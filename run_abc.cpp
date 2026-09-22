// Written by the Swansea Centre for Research in Digital Railways
// 
// in collaboration with Siemens Mobility UK
//
// September 2026
//
// Version 1
//
//-----------------------------
// no liability 
// code may be used freely
//
// File Details 
// --------------
// Filename:   AigConstructor.cs
//
// File Description
// ------------------
// Description:  This file defines a C++ utility that runs ABC's PDR algorithm to generate inductive invariants from an AIG model, extracts 
// and translates the resulting invariant into SMT-LIB format, and writes invariant assertions for use in subsequent SMT-based verification.
//
// ------------------
//
// GPL-3.0 license
//

#include <iostream>
#include <cstdio>
#include <string>
#include <fstream>
#include <sstream>
#include <vector>

std::string buildSMTExpr(
    const std::vector<std::string>& expressions,
    const std::string& suffix)
{
    std::vector<std::string> smtCubes;

    for (const std::string& cube : expressions) {
        std::stringstream ss(cube);
        std::string token;

        std::vector<std::string> smtTerms;

        while (ss >> token) {
            if (token == "∧")
                continue;

            bool negated = false;

            if (token.rfind("¬", 0) == 0) {
                negated = true;
                token = token.substr(std::string("¬").size());
            }

            token += suffix;

            if (negated) {
                smtTerms.push_back("(not " + token + ")");
            }
            else {
                smtTerms.push_back(token);
            }
        }

        std::string smtCube;

        if (smtTerms.empty()) {
            smtCube = "true";
        }
        else if (smtTerms.size() == 1) {
            smtCube = smtTerms[0];
        }
        else {
            smtCube = "(and";

            for (const auto& term : smtTerms) {
                smtCube += " " + term;
            }

            smtCube += ")";
        }

        smtCubes.push_back(smtCube);
    }

    if (smtCubes.empty())
        return "false";

    if (smtCubes.size() == 1)
        return smtCubes[0];

    std::string smtExpr = "(or";

    for (const auto& cube : smtCubes) {
        smtExpr += " " + cube;
    }

    smtExpr += ")";

    return smtExpr;
}

bool writeInvariantFile(
    const std::string& filename,
    const std::string& smtExpr)
{
    std::ofstream file(filename);

    if (!file.is_open()) {
        return false;
    }

    file << "(assert (not " << smtExpr << "))";

    return true;
}

int main(int argc, char* argv[]) {
    if (argc < 2) {
        std::cerr << "Usage: " << argv[0] << " <aiger_file>\n";
        return 1;
    }

    std::string aigFile = argv[1];

    // Create .inv filename
    std::string invFile = aigFile;
    size_t posExt = invFile.rfind(".aig");
    if (posExt != std::string::npos) {
        invFile.replace(posExt, 4, ".inv");
    } else {
        invFile += ".inv";
    }

    // Build ABC command
    std::string cmd =
        "echo \"read_aiger " + aigFile +
        "; print_stats; pdr -d -w -e -I " + invFile +
        "; quit\" | ./abc/abc";

    // Run ABC and capture output
    FILE* pipe = popen(cmd.c_str(), "r");
    if (!pipe) {
        std::cerr << "Failed to run abc\n";
        return 1;
    }

    char buffer[512];
    std::string output;

    std::cout << "=== ABC OUTPUT ===\n";

    while (fgets(buffer, sizeof(buffer), pipe)) {
        std::cout << buffer;
        output += buffer;
    }

    pclose(pipe);

    // === Parse stats ===
    int inputs = -1;
    int outputs = -1;
    int latches = -1;

    size_t pos = output.find("i/o =");
    if (pos != std::string::npos) {
        sscanf(output.c_str() + pos,
               "i/o = %d/%d  lat = %d",
               &inputs, &outputs, &latches);
    }

    std::cout << "\n=== PARSED STATS ===\n";
    std::cout << "Inputs:  " << inputs << "\n";
    std::cout << "Outputs: " << outputs << "\n";
    std::cout << "Latches: " << latches << "\n";

    // === READ + PRINT .inv FILE ===
    std::ifstream file(invFile);

    if (!file) {
        std::cerr << "\nFailed to open " << invFile << "\n";
        return 1;
    }

    std::vector<std::string> vars;
    std::vector<std::string> cubes;
    std::string line;

    std::cout << "\n=== RAW INVARIANT FILE ===\n";

    while (std::getline(file, line)) {
        // Print raw content
        std::cout << line << "\n";

        // Extract variable names
        if (line.rfind(".ilb", 0) == 0) {
            std::stringstream ss(line.substr(5));
            std::string var;
            while (ss >> var) {
                vars.push_back(var);
            }
        }
        // Skip metadata
        else if (line.empty() || line[0] == '.' || line[0] == '#') {
            continue;
        }
        // Cube lines
        else {
            cubes.push_back(line);
        }
    }

    file.close();

    std::vector<std::string> expressions;

    for (const std::string& cubeLine : cubes) {
        std::stringstream ss(cubeLine);

        std::string pattern;
        std::string outVal;

        ss >> pattern >> outVal;

        if (outVal != "1")
            continue;

        std::vector<std::string> terms;

        for (size_t i = 0; i < pattern.size() && i < vars.size(); i++) {

            if (pattern[i] == '1') {
                terms.push_back("v" + vars[i]);
            }
            else if (pattern[i] == '0') {
                terms.push_back("¬v" + vars[i]);
            }
        }

        std::string expr;

        if (terms.empty()) {
            expr = "TRUE";
        }
        else {
            expr = terms[0];

            for (size_t i = 1; i < terms.size(); i++) {
                expr += " ∧ " + terms[i];
            }
        }

        expressions.push_back(expr);
    }

    std::string smtExpr =
        buildSMTExpr(expressions, "_1");

    std::string smtExprBase =
        buildSMTExpr(expressions, "_0");

    std::cout << "\n=== INVARIANT (SMT-LIB) ===\n";
    std::cout << "(assert (not " << smtExpr << "))\n";

    std::cout << "\n=== BASE INVARIANT (SMT-LIB) ===\n";
    std::cout << "(assert (not " << smtExprBase << "))\n";

    std::string outputFile = aigFile;
    std::string baseOutputFile = aigFile;

    if (posExt != std::string::npos) {
        outputFile.replace(
            posExt, 4,
            "_invariant.smtlib");

        baseOutputFile.replace(
            posExt, 4,
            "_invariant_base.smtlib");
    }
    else {
        outputFile += "_invariant.smtlib";
        baseOutputFile += "_invariant_base.smtlib";
    }

    if (writeInvariantFile(outputFile, smtExpr)) {
        std::cout << "\nSMT-LIB written to "
                << outputFile << "\n";
    }

    if (writeInvariantFile(baseOutputFile, smtExprBase)) {
        std::cout << "\nBase SMT-LIB written to "
                << baseOutputFile << "\n";
    }

    return 0;

}
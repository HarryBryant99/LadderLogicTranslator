/* 
Written by the Swansea Centre for Research in Digital Railways

Version 1.0.0

15/09/2026

Licence: GNU General Public License v3.0

*/


//Processing the Aiger file to .aig with names

#include <cstdio>
#include <cstdlib>
#include <fstream>
#include <iostream>
#include <regex>
#include <string>
#include <vector>

struct VariableMapping
{
    std::string key;
    std::string value;
};

int main(int argc, char* argv[])
{
    if (argc != 2)
    {
        std::cerr << "Usage: " << argv[0] << " <file.aag>\n";
        return 1;
    }

    std::string aagFile = argv[1];

    // Remove .aag extension
    std::string baseName = aagFile;
    if (baseName.size() > 4 &&
        baseName.substr(baseName.size() - 4) == ".aag")
    {
        baseName = baseName.substr(0, baseName.size() - 4);
    }

    std::string detailedFile = baseName + "_detailed.txt";
    std::string tempFile = baseName + "_with_names.aag";
    std::string aigFile = baseName + ".aig";

    // Read detailed file
    std::ifstream detailed(detailedFile);
    if (!detailed)
    {
        std::cerr << "Could not open " << detailedFile << "\n";
        return 1;
    }

    std::vector<VariableMapping> vars;
    std::string line;
    bool inVarSection = false;

    std::regex pattern(R"(Key\s*=\s*(.*?),\s*Value\s*=\s*(\d+))");

    while (std::getline(detailed, line))
    {
        if (line.find("=== Decorated Var Names ===") != std::string::npos)
        {
            inVarSection = true;
            continue;
        }

        if (inVarSection &&
            line.find("===") != std::string::npos)
        {
            break;
        }

        if (inVarSection)
        {
            std::smatch match;
            if (std::regex_search(line, match, pattern))
            {
                vars.push_back({match[1], match[2]});
            }
        }
    }

    detailed.close();

    // Read original AAG
    std::ifstream aag(aagFile);
    if (!aag)
    {
        std::cerr << "Could not open " << aagFile << "\n";
        return 1;
    }

    // Create temporary AAG with names
    std::ofstream output(tempFile);
    if (!output)
    {
        std::cerr << "Could not create " << tempFile << "\n";
        return 1;
    }

    while (std::getline(aag, line))
    {
        output << line << '\n';
    }

    // Append latch names
    // Number of inputs from AAG header
    int numInputs = 0;

    // Re-open AAG file and read first line
    std::ifstream headerFile(aagFile);
    std::string headerLine;
    std::getline(headerFile, headerLine);

    std::stringstream ss(headerLine);

    std::string tag;
    int M, I, L, O, A;

    ss >> tag >> M >> I >> L >> O >> A;

    numInputs = I;

    for (const auto& var : vars)
    {
        int literal = std::stoi(var.value);

    	// Variable number in AIGER
	int varNum = literal / 2;

    	if (varNum <= numInputs)
    	{
     	    output << "i" << (varNum - 1)
     	          << " " << var.key << '\n';
    	}
	    else
	    {
        	output << "l" << (varNum - numInputs - 1)
	               << " " << var.key << '\n';
    	}
    }

    output.close();
    aag.close();

    // Convert to binary AIG
    std::string command =
        "./aiger-1.9.4/aigtoaig " + tempFile + " " + aigFile;

    std::cout << "Running: " << command << std::endl;

    int result = system(command.c_str());

    if (result != 0)
    {
        std::cerr << "Error: aigtoaig failed." << std::endl;
        return 1;
    }

    // Remove temporary file
    std::remove(tempFile.c_str());

    std::cout << "Created: " << aigFile << std::endl;

    return 0;
}

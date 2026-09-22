#include <array>
#include <cstdio>
#include <cstdlib>
#include <filesystem>
#include <iostream>
#include <string>
#include <sstream>
#include <fstream>

namespace fs = std::filesystem;

std::string runCommand(const std::string& command)
{
    std::array<char, 4096> buffer;
    std::string output;

    FILE* pipe = popen(command.c_str(), "r");

    if (!pipe)
    {
        throw std::runtime_error("Failed to start command");
    }

    while (fgets(buffer.data(), buffer.size(), pipe) != nullptr)
    {
        output += buffer.data();
    }

    pclose(pipe);

    return output;
}

void addVPrefixToTraceFile(const fs::path& filename)
{
    std::ifstream in(filename);

    if (!in)
    {
        throw std::runtime_error("Could not open counterexample file");
    }

    std::stringstream buffer;
    buffer << in.rdbuf();
    in.close();

    std::stringstream ss(buffer.str());
    std::string token;
    std::string result;

    while (ss >> token)
    {
        if (token != "#" &&
            token != "DONE" &&
            token.find('@') != std::string::npos)
        {
            token.insert(0, "v");
        }

        result += token + " ";
    }

    std::ofstream out(filename);
    out << result;
}

int main(int argc, char* argv[])
{
    if (argc != 2)
    {
        std::cerr << "Usage: " << argv[0] << " <folder>\n";
        return 1;
    }

    fs::path folder(argv[1]);

    if (!fs::exists(folder) || !fs::is_directory(folder))
    {
        std::cerr << "Error: Invalid folder '" << folder << "'\n";
        return 1;
    }

    fs::path aigFile;

    for (const auto& entry : fs::directory_iterator(folder))
    {
        if (entry.is_regular_file() &&
            entry.path().extension() == ".aig")
        {
            aigFile = entry.path();
            break;
        }
    }

    if (aigFile.empty())
    {
        std::cerr << "Error: No .aig file found in "
                  << folder << "\n";
        return 1;
    }

    fs::path cexFile = folder / "counterexample.cex";

    std::cout << "Using AIG file: "
              << aigFile << "\n";

    std::string pdrCommand =
        "abc/abc -c \""
        "read_aiger " + aigFile.string() +
        "; pdr; quit\" 2>&1";

    std::string output;

    try
    {
        output = runCommand(pdrCommand);
        //variable formatting to match tptp
    }
    catch (const std::exception& e)
    {
        std::cerr << e.what() << '\n';
        return 1;
    }

    std::cout << output << std::endl;

    bool counterExampleFound =
        output.find("Output 0") != std::string::npos;

    if (counterExampleFound)
    {
        std::cout << "Counterexample found.\n";

        std::string writeCommand =
            "abc/abc -c \""
            "read_aiger " + aigFile.string() +
            "; pdr; write_cex -f " +
            cexFile.string() +
            "; quit\"";

        int rc = std::system(writeCommand.c_str());

        if (rc != 0)
        {
            std::cerr << "Failed to write counterexample.\n";
            return 1;
        }

        std::cout << "Counterexample written to:\n"
                  << cexFile << '\n';
        addVPrefixToTraceFile(cexFile);
    }
    else
    {
        std::cout << "No counterexample found.\n";
    }

    return 0;
}

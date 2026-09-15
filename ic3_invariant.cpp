/* 
Written by the Swansea Centre for Research in Digital Railways

Version 1.0.0

15/09/2026

Licence: GNU General Public License v3.0

*/

#include <cstdlib>
#include <iostream>
#include <string>

int main(int argc, char* argv[])
{
    if (argc != 2)
    {
        std::cerr << "Usage: " << argv[0] << " <name>\n";
        return 1;
    }

    std::string name = argv[1];

    std::string abcCommand = "./run_abc " + name + ".aig";
    std::string smtCommand = "./smtlib_with_invariant " + name;

    std::cout << "\nRunning: " << abcCommand << "\n";

    int result = std::system(abcCommand.c_str());
    if (result != 0)
    {
        std::cerr << "run_abc failed with code " << result << "\n";
        return result;
    }

    std::cout << "\nRunning: " << smtCommand << "\n";

    result = std::system(smtCommand.c_str());
    if (result != 0)
    {
        std::cerr << "smtlib_with_invariant failed with code "
                  << result << "\n";
        return result;
    }

    std::cout << "Successfully completed both steps.\n";
    return 0;
}
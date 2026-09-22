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
// Description:  This file defines a C++ driver program that automates the verification workflow by running ABC-based invariant generation on 
// an .aig model and then invoking the SMT-based invariant checker on the generated results.
//
// ------------------
//
// GPL-3.0 license
//

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
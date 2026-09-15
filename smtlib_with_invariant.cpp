/* 
Written by the Swansea Centre for Research in Digital Railways

Version 1.0.0

15/09/2026

Licence: GNU General Public License v3.0

*/

#include <fstream>
#include <iostream>
#include <sstream>
#include <string>
#include <vector>
#include <stdexcept>

std::string readFile(const std::string& filename)
{
    std::ifstream file(filename);

    if (!file)
    {
        throw std::runtime_error("Cannot open file: " + filename);
    }

    std::stringstream buffer;
    buffer << file.rdbuf();

    return buffer.str();
}

struct InsertFile
{
    std::string filename;
    bool negate;
};

std::string maybeNegateAssertion(
    const std::string& content,
    bool negate)
{
    if (!negate)
    {
        return content;
    }

    const std::string prefix = "(assert ";

    if (content.rfind(prefix, 0) == 0 &&
        content.back() == ')')
    {
        std::string body =
            content.substr(prefix.size(),
                           content.size() - prefix.size() - 1);

        return "(assert (not " + body + "))";
    }

    throw std::runtime_error(
        "Expected SMT content of form '(assert ...)'");
}

void insertFilesIntoSMT(
    const std::string& targetFile,
    const std::vector<InsertFile>& insertFiles)
{
    std::string targetContent = readFile(targetFile);

    std::size_t pos = targetContent.find("(check-sat)");

    if (pos == std::string::npos)
    {
        throw std::runtime_error(
            "No (check-sat) found in " + targetFile);
    }

    std::string additions;

    for (const auto& fileInfo : insertFiles)
    {
        std::string content = readFile(fileInfo.filename);

        if (fileInfo.negate)
        {
            const std::string prefix = "(assert ";

            if (content.rfind(prefix, 0) != 0 ||
                content.empty() ||
                content.back() != ')')
            {
                throw std::runtime_error(
                    "Expected SMT file to contain a single assertion: "
                    + fileInfo.filename);
            }

            std::string body =
                content.substr(
                    prefix.size(),
                    content.size() - prefix.size() - 1);

            content = "(assert (not " + body + "))";
        }

        additions += content;
        additions += "\n";
    }

    targetContent.insert(pos, additions);

    std::ofstream output(targetFile);

    if (!output)
    {
        throw std::runtime_error(
            "Cannot write to " + targetFile);
    }

    output << targetContent;

    std::cout
        << "Inserted "
        << insertFiles.size()
        << " file(s) into "
        << targetFile
        << "\n";

    for (const auto& fileInfo : insertFiles)
    {
        std::cout << "  - "
                  << fileInfo.filename;

        if (fileInfo.negate)
        {
            std::cout << " (negated)";
        }

        std::cout << "\n";
    }
}

int main(int argc, char* argv[])
{
    if (argc != 2)
    {
        std::cerr
            << "Usage: "
            << argv[0]
            << " <name>\n";
        return 1;
    }

    std::string name = argv[1];

    try
    {
        insertFilesIntoSMT(
            name + "_step.smt",
            {
                {name + "_invariant.smtlib",false}
            });


        insertFilesIntoSMT(
            name + "_inv_base.smt",
            {
                {name + "_invariant_base.smtlib",true}
            });

        insertFilesIntoSMT(
            name + "_inv_step.smt",
            {
                {name + "_invariant_base.smtlib",false},
                {name + "_invariant.smtlib",true}
            });

        return 0;
    }

    catch (const std::exception& e)
    {
        std::cerr << e.what() << "\n";
        return 1;
    }
}
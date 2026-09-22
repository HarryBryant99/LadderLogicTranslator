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
// Description:  This file defines the ConsistencyCsvFileGenerator class, which reads CSV files containing verification results, checks 
// consistency between different verification methods (IV, BMC, and IC3), and generates CSV reports summarising the consistency results.
//
// ------------------
//
// GPL-3.0 license
//

using System.Text;

namespace SwanLLVerifier.Utils
{
    public class ConsistencyCsvFileGenerator
    {
        private readonly string inputFilePath;
        private readonly string outputFilePath;
        public ConsistencyCsvFileGenerator(string ipFilePath, string opFilePath) {
            inputFilePath = ipFilePath;
            outputFilePath = opFilePath;
        }

        public void ParseInputAndGenerateOutputIvVsBmc()
        {
            StreamReader? reader = null;
            if (File.Exists(inputFilePath))
            {
                StringBuilder csv = new();
                reader = new StreamReader(File.OpenRead(inputFilePath));
                List<string> listA = new();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        continue;

                    var values = line.Split(',');
                    bool originalIvOutput = values[1].Trim() == "yes";
                    bool originalBmcOutput = values[2].Trim() == "yes";

                    bool isConsistent = ConsistencyChecker.CalculateConsistency(ConsistencyChecker.VersusType.IvVsBmc, originalIvOutput, originalBmcOutput);

                    var newLine = string.Format("{0},{1},{2},{3}", values[0], originalIvOutput ? "yes" : "no", originalBmcOutput ? "yes" : "no", isConsistent ? "yes" : "no");
                    _ = csv.AppendLine(newLine);
                }
                reader.Close();
                File.WriteAllText(outputFilePath, csv.ToString());
            }
            else
            {
                throw new Exception($"File not found: {inputFilePath}");
            }
        }

        public void ParseInputAndGenerateOutputIvAndBmcVsIc3()
        {
            StreamReader? reader = null;
            string ivAndBmcVsIc3InputFilepath = "iv_and_bmc_vs_ic3_results.csv"; 
            string ivAndBmcVsIc3OutputFilepath = "iv_and_bmc_vs_ic3_consistency_output.csv";
            if (File.Exists(ivAndBmcVsIc3InputFilepath))
            {
                StringBuilder csv = new();
                reader = new StreamReader(File.OpenRead(ivAndBmcVsIc3InputFilepath));
                List<string> listA = new();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        continue;

                    var values = line.Split(',');
                    bool originalIvOutput = values[1].Trim() == "yes";
                    bool originalBmcOutput = values[2].Trim() == "yes";
                    string originalIc3OutputText = values[5].Trim();

                    // if not a yes or a no, then it is an exception row so skip this line for now
                    if ((originalIc3OutputText != "yes") && (originalIc3OutputText != "no"))
                    {
                        var replacementLine = string.Format("{0},{1},{2},{3},{4},{5},{6}", values[0], values[1], values[2], values[3], values[5], "N/A", "N/A");
                        _ = csv.AppendLine(replacementLine);
                        continue;
                    }
                    else
                    {
                        bool originalIc3Output = originalIc3OutputText == "yes";
                        bool isConsistentForIvVsIc3 = ConsistencyChecker.CalculateConsistency(ConsistencyChecker.VersusType.IvVsIc3, originalIvOutput, originalIc3Output);
                        bool isConsistentForBmcVsIc3 = ConsistencyChecker.CalculateConsistency(ConsistencyChecker.VersusType.BmcVsIc3, originalBmcOutput, originalIc3Output);

                        var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6}", values[0], values[1], values[2], values[3], values[5], isConsistentForIvVsIc3 ? "yes" : "no", isConsistentForBmcVsIc3 ? "yes" : "no");
                        _ = csv.AppendLine(newLine);
                    }   
                }
                reader.Close();
                File.WriteAllText(ivAndBmcVsIc3OutputFilepath, csv.ToString());
            }
            else
            {
                throw new Exception($"File not found: {ivAndBmcVsIc3InputFilepath}");
            }
        }

    }
}

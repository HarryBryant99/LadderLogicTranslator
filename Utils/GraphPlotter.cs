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
// Description:  This file defines the GraphPlotter class, which processes verification-result CSV files, computes pass rates and consistency 
// statistics, and generates summary CSV data for charts, progress analysis, and comparison of verification methods.
//
// ------------------
//
// GPL-3.0 license
//

using Microsoft.VisualBasic.FileIO;
using System.Text;

namespace SwanLLVerifier.Utils
{
    public class GraphPlotter
    {
        public void CanvaBarChartCSVGenerator()
        {
            string inputFileName = "v8_iv.csv";
            string inputFilePath = Path.Combine("SiemensData", "Lostwithiel", "CSV_Data", inputFileName);
            string outputFilePath = Path.Combine("SiemensData", "Lostwithiel", inputFileName);

            IDictionary<string, int> safetyChapterNameVsNumOfPassedChapters = new Dictionary<string, int>();
            IDictionary<string, int> safetyChapterNameVsNumOfTotalChapters = new Dictionary<string, int>();

            File.WriteAllText(outputFilePath, "");

            var csv = new StringBuilder();

            using (TextFieldParser parser = new(inputFilePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                string chapterName = "";
                string safetyGroupName = "";

                while (!parser.EndOfData)
                {
                    //Process row
                    string[]? fields = parser.ReadFields();

                    if (fields == null || fields.Length == 0)
                    {
                        continue; // or return, depending on your logic
                    }

                    safetyGroupName = fields[0].Split("_").Last();
                    chapterName = fields[0].Replace($"_{safetyGroupName}", "");
                    bool passed = fields[3] == "yes";
                    //bool passed = fields[1] == "CounterSatisfiable"; // BMC

                    if (safetyChapterNameVsNumOfPassedChapters.Keys.Contains(chapterName))
                    {
                        if (passed)
                            safetyChapterNameVsNumOfPassedChapters[chapterName] += 1;
                    }
                    else
                        safetyChapterNameVsNumOfPassedChapters.Add(chapterName, passed ? 1 : 0);

                    if (safetyChapterNameVsNumOfTotalChapters.Keys.Contains(chapterName))
                        safetyChapterNameVsNumOfTotalChapters[chapterName] += 1;
                    else
                        safetyChapterNameVsNumOfTotalChapters.Add(chapterName, 1);
                }
            }

            foreach (string chapterName in safetyChapterNameVsNumOfPassedChapters.Keys)
            {
                int totalPassed = safetyChapterNameVsNumOfPassedChapters[chapterName];
                int totalFields = safetyChapterNameVsNumOfTotalChapters[chapterName];

                double passPercent = Math.Round(((double)totalPassed / (double)totalFields) * 100, 0);

                string first = chapterName;
                string second = totalPassed.ToString();
                string third = totalFields.ToString();
                string newLine = string.Format("{0},{1},{2},{3}", first, second, third, passPercent);
                _ = csv.AppendLine(newLine);
            }

            File.AppendAllText(outputFilePath, csv.ToString());
        }

        public void GenerateMostynProgressStatusTable()
        {
            string inputFileName = "mostyn_iv_bmc_detailed_table.csv";

            List<string> ivV5V11IncrementChapters = new();
            List<string> ivV5V11DecrementChapters = new();
            List<string> ivV5V11ConstantChapters = new();

            List<string> ivV5V12IncrementChapters = new();
            List<string> ivV5V12DecrementChapters = new();
            List<string> ivV5V12ConstantChapters = new();

            using (TextFieldParser parser = new(inputFileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                while (!parser.EndOfData)
                {
                    string[]? fields = parser.ReadFields();

                    if (fields == null || fields.Length == 0)
                    {
                        continue;
                    }

                    string chapterName = fields[0];

                    int v5IVPassPercent = int.Parse(fields[1]);
                    int v12IVPassPercent = int.Parse(fields[3]);
                    int v11IVPassPercent = int.Parse(fields[5]);

                    int v5BMCFailPercent = Math.Abs(int.Parse(fields[2]));
                    int v12BMCFailPercent = Math.Abs(int.Parse(fields[4]));
                    int v11BMCFailPercent = Math.Abs(int.Parse(fields[6]));


                    if (v12BMCFailPercent > v5BMCFailPercent)
                        ivV5V12IncrementChapters.Add(chapterName);
                    else if (v12BMCFailPercent < v5BMCFailPercent)
                        ivV5V12DecrementChapters.Add(chapterName);
                    else
                        ivV5V12ConstantChapters.Add(chapterName);

                }
            }

            Console.WriteLine(ivV5V12IncrementChapters.Count());
            Console.WriteLine(ivV5V12DecrementChapters.Count());
            Console.WriteLine(ivV5V12ConstantChapters.Count());

            Console.WriteLine(string.Join(", ", ivV5V12IncrementChapters));
            Console.WriteLine("==================");
            Console.WriteLine(string.Join(", ", ivV5V12DecrementChapters));
            Console.WriteLine("==================");
            Console.WriteLine(string.Join(", ", ivV5V12ConstantChapters));

        }

        public void ProcessMostynCSV()
        {
            var csv = new StringBuilder();
            string sourceCsvPath = Path.Combine("SiemensData", "mostyn_output_v12.csv");
            string processedCsvPath = "mostyn_output_v12_processed.csv";

            File.WriteAllText(processedCsvPath, "");

            using TextFieldParser parser = new(sourceCsvPath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            bool ivVsBmcConsistency = false;

            while (!parser.EndOfData)
            {
                //Process row
                string[]? fields = parser.ReadFields();
                if (fields == null)
                {
                    continue; // or return
                }

                string chapterAndCondFilename = fields[0];
                bool ivResult = fields[1].Trim() == "yes";
                bool bmcResult = fields[2].Trim() == "yes";
                ivVsBmcConsistency = ConsistencyChecker.CalculateConsistency(ConsistencyChecker.VersusType.IvVsBmc, ivResult, bmcResult);

                var newLine = string.Format("{0},{1},{2},{3}", chapterAndCondFilename, fields[1].Trim(), fields[2].Trim(), ivVsBmcConsistency ? "yes" : "no");
                _ = csv.AppendLine(newLine);
            }
            File.AppendAllText(processedCsvPath, csv.ToString());
        }

        // theorem - the property has been proven to hold
        // countersatisfiable - the property has not been proven to hold

        public void ProcessLochnessCSV()
        {
            var csv = new StringBuilder();
            string sourceCsvPath = Path.Combine("iv_and_bmc_vs_ic3_consistency_output_lochness.csv");
            string processedCsvPath = "iv_and_bmc_vs_ic3_consistency_output_lochness_processed1.csv";

            File.WriteAllText(processedCsvPath, "");

            using TextFieldParser parser = new(sourceCsvPath);
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");

            bool ivVsBmcConsistency = false;
            bool ivVsIc3Consistency = false;
            bool bmcVsIc3Consistency = false;

            while (!parser.EndOfData)
            {
                //Process row
                string[]? fields = parser.ReadFields();

                if (fields == null || fields.Length == 0)
                {
                // handle end of file or invalid record
                return;
                }

                string chapterAndCondFilename = fields[0];
                string origIvBase = fields[6].Trim();
                string tfIvBase = fields[9].Trim();
                string origIvStep = fields[7].Trim();
                string tfIvStep = fields[10].Trim();
                string origBMC = fields[8].Trim();
                string tfBMC = fields[12].Trim();


                bool ivResult = fields[2].Trim() == "yes";
                bool bmcResult = fields[4].Trim() == "yes";
                bool ic3Result = fields[12].Trim() == "yes";
                ivVsBmcConsistency = ConsistencyChecker.CalculateConsistency(ConsistencyChecker.VersusType.IvVsBmc, ivResult, bmcResult);
                ivVsIc3Consistency = ConsistencyChecker.CalculateConsistency(ConsistencyChecker.VersusType.IvVsIc3, ivResult, ic3Result);
                bmcVsIc3Consistency = ConsistencyChecker.CalculateConsistency(ConsistencyChecker.VersusType.BmcVsIc3, bmcResult, ic3Result);


                var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6}", chapterAndCondFilename, ivVsBmcConsistency ? "yes" : "no", ivVsIc3Consistency ? "yes" : "no", bmcVsIc3Consistency ? "yes" : "no", origIvBase == tfIvBase ? "yes" : "no", origIvStep == tfIvStep ? "yes" : "no", origBMC == tfBMC ? "yes" : "no");
                _ = csv.AppendLine(newLine);
            }
            File.AppendAllText(processedCsvPath, csv.ToString());
        }

    }
}

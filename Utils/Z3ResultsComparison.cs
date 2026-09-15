using SwanLLVerifier.AIG;
//using SwanLLVerifier.ETCSDC_Properties;
using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;
using SwanLLVerifier.LadderLogic;
using SwanLLVerifier.SafetyProperty;
using SwanLLVerifier.SMTLib;
using System.Diagnostics;
using System.Text;
using System.Xml;
using static SwanLLVerifier.PropositionalLogic.PropositionalFormulaBuilder;

namespace SwanLLVerifier.Utils
{
    public class Z3ResultsComparison
    {
        public Z3ResultsComparison()
        {
            GenerateSmtAndRunZ3ForLochNess();
        }

        public void GenerateSmtAndRunZ3ForLochNess()
        {
            string sourceRootPath = @"SiemensData";
            string lochnessSafetyDirPath = Path.Combine(sourceRootPath, "Additional_LochNess_Properties");
            string lochnessTrackPlanXmlPath = Path.Combine(sourceRootPath, "LochNess-810", "810.xml");
            string outputFilepath = "lochness_new_smt_z3_run_phi_and_not_phi_22_may.csv";

            // empty the output file content on every new run
            File.WriteAllText(outputFilepath, "");

            string[] allLochnessCondDirectories = Directory.GetDirectories(lochnessSafetyDirPath);

            foreach (string lochnessCondDirectory in allLochnessCondDirectories)
            {
                if (lochnessCondDirectory.Contains("822"))
                    continue;

                Console.WriteLine(lochnessCondDirectory);
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

                string[] lochnessCondFileEntries = Directory.GetFiles(lochnessCondDirectory);

                var csv = new StringBuilder();

                foreach (string lochnessCondFile in lochnessCondFileEntries)
                {
                    XmlDocument doc = new();

                    doc.Load(lochnessTrackPlanXmlPath);

                    Ladder ladder = LadderLogicXmlParser.ParseXML(doc);
                    Model modelForOrgLadder = new(ladder);
                    modelForOrgLadder.InitialiseModel();

                    // the safety condition is always on the 5th line of the .cond file
                    string safetyPropertyString = File.ReadLines(lochnessCondFile).Skip(4).Take(1).First();
                    AbstractFirstOrderFormula safetyCondition = SafetyPropertyParser.Parse(safetyPropertyString);
                    AbstractFirstOrderFormula negatedSafety = MakeNegation(safetyCondition);

                    AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(safetyCondition);
                    AbstractFirstOrderFormula transformedNegSafety = TransformToAig.Transform(negatedSafety);

                    var watch = new Stopwatch();
                    double elapsedTime = 0;
                    string safetyOriginalBase = "";
                    string safetyOriginalStep = "";
                    string negSafetyOriginalBase = "";
                    string negSafetyOriginalStep = "";
                    string safetyTransformedBase = "";
                    string safetyTransformedStep = "";
                    string negSafetyTransformedBase = "";
                    string negSafetyTransformedStep = "";

                    try
                    {
                        SMTLibUtil.ToSMTLibInductive(ladder, safetyCondition, "safety_original");
                        SMTLibUtil.ToSMTLibInductive(ladder, negatedSafety, "neg_safety_original");

                        SMTLibUtil.ToSMTLibInductive(ladder, transformedSafety, "safety_transformed");
                        SMTLibUtil.ToSMTLibInductive(ladder, transformedNegSafety, "neg_safety_transformed");

                        watch.Start();
                        string[] z3Results = ExecuteZ3InShell();
                        watch.Stop();

                        elapsedTime = watch.Elapsed.TotalSeconds;

                        safetyOriginalBase = z3Results[0].Replace("\n", string.Empty);
                        safetyOriginalStep = z3Results[1].Replace("\n", string.Empty);
                        negSafetyOriginalBase = z3Results[2].Replace("\n", string.Empty);
                        negSafetyOriginalStep = z3Results[3].Replace("\n", string.Empty);
                        safetyTransformedBase = z3Results[4].Replace("\n", string.Empty);
                        safetyTransformedStep = z3Results[5].Replace("\n", string.Empty);
                        negSafetyTransformedBase = z3Results[6].Replace("\n", string.Empty);
                        negSafetyTransformedStep = z3Results[7].Replace("\n", string.Empty);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($">>>>>>>>>>>>>>> EXCEPTION: {ex.Message}");
                    }

                    var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", lochnessCondFile, safetyOriginalBase, safetyOriginalStep, negSafetyOriginalBase, negSafetyOriginalStep, safetyTransformedBase, safetyTransformedStep, negSafetyTransformedBase, negSafetyTransformedStep, elapsedTime);

                    _ = csv.AppendLine(newLine);
                }

                File.AppendAllText(outputFilepath, csv.ToString());
            }
        }

        public string[] ExecuteZ3InShell()
        {
            string[] fileNames = new string[] { "safety_original_base.smt", "safety_original_step.smt", "neg_safety_original_base.smt", "neg_safety_original_step.smt", "safety_transformed_base.smt", "safety_transformed_step.smt", "neg_safety_transformed_base.smt", "neg_safety_transformed_step.smt" };

            string[] z3outputs = new string[] { "n/a", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a", "n/a" };

            for (int i = 0; i < fileNames.Length; i++)
            {
                string argument = $"z3 ~/swansea-uni/SwanLLVerifier/SwanLLVerifier/bin/Debug/net6.0/{fileNames[i]}";

                ProcessStartInfo startInfo = new()

                {
                    FileName = "/bin/bash",
                    Arguments = " -c \"" + argument + " \"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                string output = "n/a";
                // Start the process
                using Process process = new();
                process.StartInfo = startInfo;
                _ = process.Start();

                // Read the output
                output = process.StandardOutput.ReadToEnd();

                // Wait for the process to exit
                process.WaitForExit();

                // Print output and error
                Console.WriteLine($">>>>>>>>> Z3 Output {fileNames[i]}:");
                Console.WriteLine(output);

                z3outputs[i] = output;
            }

            return z3outputs;
        }

    }
}

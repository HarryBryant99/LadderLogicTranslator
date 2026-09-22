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
// Filename:   RunForSiemensTestbed.cs
//
// File Description
// ------------------
// Description:  A class to run the LLT on the Ladder Logic and Safety Property files.
//
// ------------------
//
// GPL-3.0 license
//

using System.Diagnostics;
using System.Text;
using System.Text.Json;
using SwanLLVerifier.AIG;
//using SwanLLVerifier.ETCSDC_Properties;
using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;
using SwanLLVerifier.LadderLogic;
using SwanLLVerifier.TptpParser;
using static SwanLLVerifier.PropositionalLogic.PropositionalFormulaBuilder;
using SwanLLVerifier.PropositionalLogic;
using SwanLLVerifier.SMTLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SwanLLVerifier.Utils
{
    public class RunForSiemensTestbed
    {
        public RunForSiemensTestbed()
        {
            string relativePath = Environment.CurrentDirectory.EndsWith("net6.0")
                ? "../../../"
                : "";

            string sourceRootPath = relativePath + "Examples";

            string TptpPath = Path.Combine(
                sourceRootPath,
                "CounterExample_Example",
                "Ladder.tptp"
            );
            
            string SafetyDirPath = Path.Combine(
                sourceRootPath,
                "CounterExample_Example/SafetyProperties"
            );

            string outputFilepath = "output.csv";

             RunForLadderLogic(
             relativePath,
             sourceRootPath,
             TptpPath,
             SafetyDirPath,
             outputFilepath
             );
        }

        public static void RunForLadderLogic(string relativePath, string sourceRootPath, string TptpPath, string TptpSafetyDirPath, 
            string outputFilepath, IDictionary<string, bool>? initialisedLatches = null)
        {

            Ladder ladder;
            // *** the tptp way ***
            using (
                FileStream fileStream = new(
                    TptpPath,
                    FileMode.Open,
                    FileAccess.Read
                )
            )
            {
                ladder = LadderParser.ParseLadder(fileStream);
            }

            string[] SafetyFileEntries = Directory.GetFiles(TptpSafetyDirPath);

            var csv = new StringBuilder();

            foreach (string SafetyFile in SafetyFileEntries)
            {
                string condFileName = SafetyFile.Split("/").Last();

                //int ic3Output;
                var watch = new Stopwatch();
                //string chapterName = "";
                string doesPropHoldInIC3 = "";
                //string originalIVInitResult = "";
                //string originalIVStepResult = "";
                //string originalBMCResult = "";
                //string transformedIVInitResult = "";
                //string transformedIVStepResult = "";
                //string transformedBMCResult = "";
                double elapsedTime = 0;
                //string ivResult = "";
                //string bmcResult = "";

                //Verifcation Condition Folder and File Name
                String ladderFileName = Path.GetFileName(Path.GetDirectoryName(TptpPath)!);
                String condFileNameWithoutExt = Path.GetFileNameWithoutExtension(condFileName);
                String verificationCondtion = ladderFileName + "_" + condFileNameWithoutExt;

                foreach (Rung r in ladder.Rungs)
                {
                    if (initialisedLatches != null &&
                        initialisedLatches.TryGetValue(r.output, out bool initialised))
                    {
                        r.Initialised = initialised;
                    }
                    else
                    {
                        r.Initialised = false;
                    }
                }

                //Console.WriteLine(verificationCondtion);

                Console.WriteLine(SafetyFile);
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

                Model modelForOrgLadder = new(ladder);
                modelForOrgLadder.InitialiseModel();

                Ladder transformedLadder = TransformToAig.TransformLadder(ladder);
                Model modelForTfLadder = new(transformedLadder);
                modelForTfLadder.InitialiseModel();

                AbstractFirstOrderFormula safetyCondition = MakeVar("null");
                // parse safety from TPTP format
                try
                {
                    safetyCondition = ConditionParser.ParseTptpSafety(SafetyFile);
                    PrettyPrinter.PrettyPrint(safetyCondition);
                }
                catch (Exception ex)
                {
                    var exceptionLine = string.Format("{0},{1}", SafetyFile, ex.Message);
                    _ = csv.AppendLine(exceptionLine);
                    continue; // log the exception and move on
                }
                AbstractFirstOrderFormula negatedSafety = MakeNegation(safetyCondition);
                AbstractFirstOrderFormula transformedNegSafety = TransformToAig.Transform(
                    negatedSafety
                );

                // create Directory of fileName if it doesn't exist
                string? directoryName = ladderFileName + "/" + verificationCondtion;
                //if (!string.IsNullOrWhiteSpace(directoryName))
                //{
                //    Directory.CreateDirectory(directoryName);
                //} else
                //{
                //    Directory.CreateDirectory("Test");
                //}

                try
                {
                    AigConstructor aigConstructor = new(
                        transformedLadder,
                        transformedNegSafety,
                        modelForTfLadder.LatchNamesAndValues
                    );
                    aigConstructor.Decorate();
                    aigConstructor.ConstructAigerFile(directoryName + "/" + verificationCondtion + ".aag");

                    Thread.Sleep(100);

                    //watch.Start();
                    //ic3Output = Program.RunIC3();
                    //watch.Stop();

                    //Console.WriteLine($">>> Processing .cond file {condFileName}.");
                    Console.WriteLine($">>> Processing .tptp file {condFileName}.");

                    // ========== in your loop
                    //chapterName = $"{dirName}_{condFileName}";
                    //doesPropHoldInIC3 = (ic3Output == 1) ? "no" : "yes";
                    //elapsedTime = (watch.Elapsed.TotalSeconds);
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    if (
                        ex.Message.Contains(
                            "Expected predicate keys to be pre-included in a dictionary."
                        )
                    )
                    {
                        //chapterName = $"{dirName}_{condFileName}";
                        doesPropHoldInIC3 = $"N/A => {ex.Message}";
                        elapsedTime = (watch.Elapsed.TotalSeconds);
                    }
                }

                try
                {
                    SMTLibUtil.ToSMTLibInductive(ladder, negatedSafety, "results/" + directoryName + "/" + verificationCondtion);

                //    ivResult = ((originalIVInitResult == "unsat") && (originalIVStepResult == "unsat")) ? "yes" : "no";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($">>>>>>>>>>>>>>> EXCEPTION: {ex.Message}");
                }

                var newLine = string.Format(
                    "{0},{1},{2}",
                    SafetyFile,
                    doesPropHoldInIC3,
                    elapsedTime
                );

                _ = csv.AppendLine(newLine);
            }
        }

        public static string FormatFormulaOutput(string formulaOutput)
        {
            // remove /r and /n at the end of the formulaOutput
            formulaOutput = formulaOutput.TrimEnd('\r', '\n');
            // replace /\\ with &
            formulaOutput = formulaOutput.Replace("/\\", "&");
            return formulaOutput;
        }

        public static void VerifyBL(
            AbstractFirstOrderFormula request,
            AbstractFirstOrderFormula response,
            string ladderPath,
            string bltptpPath,
            string pdFileName,
            string ladderFileName,
            int steps
        )
        {
            // get all variables from request and response
            //TODO: use PropositionalFormulaUtils.AllVariables()
            ISet<string> responseVars = PropositionalFormulaUtils.AllVariablesFromFormula(response);
            ISet<string> requestVars = PropositionalFormulaUtils.AllVariablesFromFormula(request);

            Ladder ladder;
            using (FileStream fileStream = new(ladderPath, FileMode.Open, FileAccess.Read))
            {
                ladder = LadderParser.ParseLadder(fileStream);
            }
            ISet<string> ladderVars = ladder.AllVariables();

            // check that all requestVars and responseVars are in ladderVars
            foreach (var v in requestVars)
            {
                if (!ladderVars.Contains(v))
                {
                    throw new Exception(
                        $"Request variable {v} is not present in ladder variables."
                    );
                }

                if (!v.EndsWith("_0"))
                {
                    throw new Exception($"Request variable {v} does not end with _0 as expected.");
                }
            }
            foreach (var v in responseVars)
            {
                if (!ladderVars.Contains(v))
                {
                    throw new Exception(
                        $"Response variable {v} is not present in ladder variables."
                    );
                }
            }

            // pretty print request and response
            string requestOutput = PrettyPrinter.CaptureConsoleOutput(() =>
            {
                PrettyPrinter.PrettyPrint(request);
            });

            // remove /r and /n at the end of the requestOutput
            requestOutput = FormatFormulaOutput(requestOutput);

            string responseOutput = PrettyPrinter.CaptureConsoleOutput(() =>
            {
                PrettyPrinter.PrettyPrint(response);
            });

            responseOutput = FormatFormulaOutput(responseOutput);

            // vS100_RU_1 | vS100_RU_2 | vS100_RU_3 | vS100_RU_4
            // based on steps, create responseOutput with _1, _2, ..., _steps
            List<string> responseOutputs = new();
            for (int i = 0; i <= steps; i++)
            {
                string stepOutput = responseOutput;
                foreach (var v in responseVars)
                {
                    // vS100_RU_0 to vS100_RU_i
                    string newVar = v.Substring(0, v.Length - 1) + i.ToString();
                    stepOutput = stepOutput.Replace(v, newVar);
                }
                responseOutputs.Add($"({stepOutput})");
            }

            // TODO: Check that all response outputs are in ladderVars with correct step suffix

            // write to bltptpPath
            using StreamWriter writer = new(bltptpPath);

            writer.WriteLine($"include('{pdFileName}').");
            writer.WriteLine($"include('{ladderFileName}').");
            writer.WriteLine(
                $"fof(ax,conjecture, ({requestOutput}) => ({string.Join(" | ", responseOutputs)}))."
            );

            // close the writer
            writer.Close();

            //@".\Z3\z3_tptp.exe"
            string argument = ".\\Z3\\z3_tptp.exe" + " " + Path.GetFullPath(bltptpPath);

            // run .\Z3\z3_tptp.exe on bltptpPath
            ProcessStartInfo startInfo = new()
            {
                FileName = "powershell.exe",
                Arguments = argument,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            // console the command being run
            Console.WriteLine($"Run command: {startInfo.FileName} {startInfo.Arguments}");

            using Process process = new() { StartInfo = startInfo };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            Console.WriteLine("Z3 Output:");
            Console.WriteLine(output);
        }

        public static void VerifyPD(
            string relativePath,
            string PDpath,
            string ladderPath,
            string TpTpFileName,
            string fileName,
            List<string> ExcludePdKeys,
            IDictionary<string, int>? addedPDict = null,
            IDictionary<string, bool>? initialisedLatches = null
        )
        {


            Ladder ladder;

            using (
                FileStream fileStream = new(
                    $"{relativePath}{ladderPath}",
                    FileMode.Open,
                    FileAccess.Read
                )
            )
            {
                ladder = LadderParser.ParseLadder(fileStream);
            }

            // initialise all ladder rungs to false
            // foreach (var rung in ladder.Rungs)
            // {
            //     rung.Initialised = false;
            // }

            if (initialisedLatches != null)
            {
                // iterate through the dictionary and set the Initialised property of the corresponding rungs
                foreach (var kv in initialisedLatches)
                {
                    Rung? rung = ladder.Rungs.FirstOrDefault(r => r.output == kv.Key);
                    if (rung != null)
                    {
                        rung.Initialised = kv.Value;
                    }
                    else
                    {
                        throw new Exception(
                            $"Ladder does not contain a rung with output variable '{kv.Key}'"
                        );
                    }

                    // Rung? rung = ladder.Rungs.FirstOrDefault(r => r.output == AigConstructor.FormatVarName(kv.Key));
                    // if (rung != null)
                    // {
                    //     rung.Initialised = kv.Value;
                    // }
                    // else
                    // {
                    //     throw new Exception($"Ladder does not contain a rung with output variable '{kv.Key}'");
                    // }
                }
            }

            Model modelForOrgLadder = new(ladder);
            modelForOrgLadder.InitialiseModel();

            Ladder transformedLadder = TransformToAig.TransformLadder(ladder);
            Model modelForTfLadder = new(transformedLadder);
            modelForTfLadder.InitialiseModel();

            List<string> TruthValuesPdStates = new();
            List<string> FalseValuesPdStates = new();

            ISet<string> outputvars = transformedLadder.AllOutputVariables();

            // check that all output vars only end with _0 or _1
            foreach (var v in outputvars)
            {
                if (!v.EndsWith("_0") && !v.EndsWith("_1"))
                {
                    throw new Exception(
                        $"Output variable {v} does not end with _0 or _1 as expected."
                    );
                }
            }

            ISet<string> InputsAndLatches = transformedLadder.AllVariables();
            // all inputs
            ISet<string> Inputs = transformedLadder.AllInputs();

            List<string> formatedAllInputsAndLatches = new();
            foreach (var v in InputsAndLatches)
            {
                string formattedVar = AigConstructor.FormatVarName(v);
                formatedAllInputsAndLatches.Add(formattedVar);
            }

            List<string> formatedOutputVariables = new();
            foreach (var v in outputvars)
            {
                string formattedVar = AigConstructor.FormatVarName(v);
                formatedOutputVariables.Add(formattedVar);
            }

            List<string> formatedAllInputs = new();
            foreach (var v in Inputs)
            {
                string formattedVar = AigConstructor.FormatVarName(v);
                formatedAllInputs.Add(formattedVar);
            }

            // write the variable to a file
            // File.WriteAllLines(varsfileName, new[] { "Inputs: \n" + string.Join(", \n", allInputs) + "\nOutputVariable: \n"
            //     + string.Join(", \n", outputVariables) });

            // Check if any varss contains 'JS'
            var jsVars = formatedOutputVariables.Where(v => v.EndsWith("JS")).ToList();
            var jrVars = formatedAllInputs.Where(v => v.EndsWith("JR")).ToList();

            // exclude jsVars from varss
            ISet<string> ouputVariablesWithoutJS = formatedOutputVariables
                .Except(jsVars)
                .ToHashSet();

            string PDjson = File.ReadAllText(relativePath + PDpath);
            var PDdoc = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(PDjson);

            if (PDdoc is null)
            {
            throw new InvalidOperationException("Failed to deserialize PDjson.");
            }

            // normalize keys
            var PDdict = new Dictionary<string, JsonElement>();
            foreach (var kv in PDdoc)
            {
                string key = kv.Key.Trim();
                if (key.StartsWith("!"))
                {
                    continue;
                }
                PDdict[key] = kv.Value;
            }

            var formattedAddedPD = new Dictionary<string, string>();
            if (addedPDict != null)
            {
                foreach (var kv in addedPDict)
                {
                    var formattedKey = AigConstructor.FormatVarName(kv.Key);
                    if (!formatedAllInputsAndLatches.Contains(formattedKey))
                    {
                        throw new Exception(
                            $"In added PDdict. You expected a key to be in the variables but it's not. It's missing. The missing key: {kv.Key}"
                        );
                    }
                    formattedAddedPD[formattedKey] = kv.Value.ToString();
                }
            }
            // addedPDict = formattedAddedPD;

            // write the PDdict keys and values as json file
            File.WriteAllText(
                relativePath + PDpath + "_Formatted.json",
                JsonSerializer.Serialize(PDdict, new JsonSerializerOptions { WriteIndented = true })
            );

            List<string> pdStatesWithoutJSandInputs = new();
            List<string> jrVarswithTruthValues = new();
            List<string> JSVarswithTruthValues = new();
            List<string> falsePdStatesWithoutJSandInputs = new();
            List<string> pdStatesAndInputsWithTruthValues = new();

            foreach (var kv in PDdict)
            {
                string theKey = AigConstructor.FormatVarName(kv.Key);
                // File.AppendAllText(pdFileName, $"{theKey}:{kv.Value}\n");
                var theValue = kv.Value;
                // if the key is in addExcludePdKeys, replace it#
                if (addedPDict != null && addedPDict.ContainsKey(theKey))
                {
                    theValue = JsonDocument.Parse(formattedAddedPD[theKey]).RootElement;
                }

                if (ExcludePdKeys.Contains(kv.Key))
                {
                    continue;
                }
                // string theKey = kv.Key;
                if (!formatedAllInputsAndLatches.Contains(theKey))
                {
                    // Console all variables missing
                    List<string> missingVars = new();
                    foreach (var kv2 in PDdict)
                    {
                        string formattedKey2 = AigConstructor.FormatVarName(kv2.Key);
                        if (!formatedAllInputsAndLatches.Contains(formattedKey2))
                        {
                            missingVars.Add(kv2.Key);
                        }
                    }
                    Console.WriteLine(
                        $"Missing variables: {{ {string.Join(", ", missingVars.Select(v => $"\"{v}\""))} }}"
                    );
                    throw new Exception(
                        $"You expected paradise state key to be in the variables but it's not. It's missing. The missing key: {kv.Key}"
                    );
                    // continue;
                }
                // if value is 1, add to pdStates
                else if (
                    (theValue.ValueKind == JsonValueKind.Number && theValue.GetInt32() == 1)
                    || (theValue.ValueKind == JsonValueKind.String && theValue.GetString() == "1")
                )
                {
                    if (formatedOutputVariables.Contains(theKey))
                        TruthValuesPdStates.Add(theKey);
                    // if (addedPDdict.ContainsKey(kv.Key))
                    //     TruthValuesPdStates.Add(theKey);

                    // if it's input skip it
                    if (ouputVariablesWithoutJS.Contains(theKey))
                        pdStatesWithoutJSandInputs.Add(theKey);
                    else if (jrVars.Contains(theKey))
                        jrVarswithTruthValues.Add(theKey);
                    else if (jsVars.Contains(theKey))
                        JSVarswithTruthValues.Add(theKey);
                    pdStatesAndInputsWithTruthValues.Add(theKey);
                }
                else if (
                    (theValue.ValueKind == JsonValueKind.Number && theValue.GetInt32() == 0)
                    || (theValue.ValueKind == JsonValueKind.String && theValue.GetString() == "0")
                )
                {
                    if (formatedOutputVariables.Contains(theKey))
                        FalseValuesPdStates.Add(theKey);
                    // if it's input skip it
                    if (ouputVariablesWithoutJS.Contains(theKey))
                        falsePdStatesWithoutJSandInputs.Add(theKey);
                    // pdStatesAndInputsWithTruthValues.Add(theKey);
                }
            }

            // formatted output variables

            // //EXCLUDE JS AND JR VARS from pd states
            // TruthValuesPdStates = TruthValuesPdStates
            //     .Where(v => !v.EndsWith("JS") && !v.EndsWith("JR"))
            //     .ToList();

            List<string> FalseValuesNonPdStates = formatedOutputVariables
                .Where(v => !TruthValuesPdStates.Contains(v))
                .ToList();

            // //EXCLUDE JS AND JR VARS from pd states
            // FalseValuesNonPdStates = FalseValuesNonPdStates
            //     .Where(v => !v.EndsWith("JS") && !v.EndsWith("JR"))
            //     .ToList();

            List<string> BeTrueVars = TruthValuesPdStates;
            List<string> BeNegatedVars = FalseValuesPdStates;

            // append _0 to all vars in toBeTrueVars and toBeNegatedVars
            // check if they are in ladder variables with _0 suffix, if not throw an exception
            List<string> finalToBeTrueVars = new();
            List<string> finalToBeNegatedVars = new();
            foreach (var v in BeTrueVars)
            {
                string varWithSuffix = $"v{v}_0";
                if (!InputsAndLatches.Contains(varWithSuffix))
                {
                    // check with _1
                    varWithSuffix = $"v{v}_1";
                    if (!InputsAndLatches.Contains(varWithSuffix))
                    {
                        throw new Exception(
                           $"Variable {varWithSuffix} is not present in ladder variables with _0 or _1 suffix."
                       );
                    }
                }
                finalToBeTrueVars.Add(varWithSuffix);
            }

            foreach (var v in BeNegatedVars)
            {
                string varWithSuffix = $"v{v}_0";
                if (!InputsAndLatches.Contains(varWithSuffix))
                {
                    // check with _1
                    varWithSuffix = $"v{v}_1";
                    if (!InputsAndLatches.Contains(varWithSuffix))
                    {
                        throw new Exception(
                            $"Variable {varWithSuffix} is not present in ladder variables with _0 or _1 suffix."
                        );
                    }
                }
                finalToBeNegatedVars.Add(varWithSuffix);
            }

            using (StreamWriter writer = new(relativePath + TpTpFileName))
            {
                foreach (var state in finalToBeTrueVars)
                {
                    writer.WriteLine($"fof(ax,axiom, {state}).");
                }
                foreach (var state in finalToBeNegatedVars)
                {
                    writer.WriteLine($"fof(ax,axiom, ~{state}).");
                }
            }

            AbstractFirstOrderFormula safetyCondition = MakeNegation(
                MakeAnd(
                    finalToBeTrueVars
                        .Select(ps => MakeVar(ps))
                        .ToList()
                        .Concat(
                            finalToBeNegatedVars
                                .Select(nps =>
                                    (AbstractFirstOrderFormula)MakeNegation(MakeVar(nps))
                                )
                                .ToList()
                        )
                        .ToList()
                )
            );

            /// INTERNAL WORKINGS - DO NOT TOUCH
            AbstractFirstOrderFormula negatedSafety = MakeNegation(safetyCondition);
            Console.WriteLine("Negated safety condition:");
            PrettyPrinter.PrettyPrintWithDelay(negatedSafety);
            AbstractFirstOrderFormula transformedNegSafety;

            Thread thread = new(
                () =>
                {
                    transformedNegSafety = TransformToAig.Transform(negatedSafety);

                    //int ic3Output = 0;

                    AigConstructor aigConstructor = new(
                        transformedLadder,
                        transformedNegSafety,
                        modelForTfLadder.LatchNamesAndValues
                    );
                    aigConstructor.Decorate();
                    aigConstructor.ConstructAigerFile(fileName);

                    Program.RunIC3(fileName);
                },
                16 * 1024 * 1024
            ); // 16 MB stack size
            thread.Start();
            thread.Join(10000); // 10 seconds timeout
        }

        private void RunClausegenInductiveVerification()
        {
            string sourceRootPath = @"Examples";
            string clausegenExeDirName = "clausegen-exe-by-harry";

            string outputFileName = "old_tptp_verifier_iv_results.csv"; //this directory needs to be precreated
            string safetyDirPath = Path.Combine(sourceRootPath, "Ladder\\safety-cond-files");
            string command =
                           $".\\clausegen.exe -l ..\\Ladder.wt2 -s safety_original.cond --proofstrategy=inductive -g yes";


            string[] allCondDirectories = Directory.GetDirectories(safetyDirPath);

            // empty the output file content on every new run
            File.WriteAllText(outputFileName, "Safety Property,IV Base, IV Step, IV final, Time_Taken\n");

            var csv = new StringBuilder();

            foreach (string condDirectory in allCondDirectories)
            {
                Console.WriteLine(condDirectory);
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

                string[] condFileEntries = Directory.GetFiles(condDirectory);


                foreach (string safetyFile in condFileEntries)
                {
                    string inductiveOutput = "n/a";
                    string[] safetyFileNameParts = safetyFile.Split("\\");
                    // string originalChapterAndCondFilename =
                    //     $"{safetyFileNameParts[2]}_{safetyFileNameParts[3]}";
                    string originalChapterAndCondFilename =
                                        $"{safetyFileNameParts[3]}/{safetyFileNameParts[4]}";
                    var watch = new Stopwatch();

                    File.Copy(
                        safetyFile,
                        Path.Combine(sourceRootPath, clausegenExeDirName, "safety_original.cond"),
                        true
                    ); // overwrite any other safety_original.cond file if present

                    Console.WriteLine(
                        $">>> Running clausegen for {originalChapterAndCondFilename}"
                    );
                    watch.Start();

                    // specify the correct wt2 in ExecuteClausegenInCmd() manually before running this function
                    inductiveOutput = ExecuteClausegenInCmd(
                        Path.Combine(sourceRootPath, clausegenExeDirName), command
                    );
                    watch.Stop();
                    //Thread.Sleep(500);

                    var newLine = string.Format(
                        "{0},{1},{2}",
                        originalChapterAndCondFilename,
                        inductiveOutput,
                        watch.Elapsed.TotalSeconds
                    );

                    //  // TODO: the inductiveOutput comes in 2 lines. As of now, this is usually formatted manually in the CSV file later. the formatting can be done automatically in CSV generation.

                    // _ = csv.AppendLine(newLine);

                    // TODO: the inductiveOutput comes in 2 lines. As of now, this is usually formatted manually in the CSV file later. the formatting can be done automatically in CSV generation.
                    // inductive output example:
                    //% SZS status Theorem
                    //% SZS status CounterSatisfiable

                    // split inductiveOuput into base and step results
                    string[] inductiveOutputLines = inductiveOutput.Split("\n");
                    string ivBaseResult = inductiveOutputLines[0].Trim();
                    string ivStepResult = inductiveOutputLines.Length > 1 ? inductiveOutputLines[1].Trim() : "n/a";
                    // string ivFinalResult = inductiveOutputLines.Length > 2 ? inductiveOutputLines[2].Trim() : "n/a";
                    // final result is yes if both base and step results are Theorem, no otherwise
                    string ivFinalResult = (ivBaseResult == "% SZS status Theorem" && ivStepResult == "% SZS status Theorem") ? "yes" : "no";
                    string formattedNewLine = $"{originalChapterAndCondFilename},{ivBaseResult},{ivStepResult},{ivFinalResult},{watch.Elapsed.TotalSeconds}";
                    _ = csv.AppendLine(formattedNewLine);
                    // write header
                    File.WriteAllText(outputFileName, "Safety Property,IV Base, IV Step, IV final, Time_Taken\n");

                    File.AppendAllText(outputFileName, csv.ToString());

                }
                File.WriteAllText(outputFileName, "Safety Property,IV Base, IV Step, IV final, Time_Taken\n");

                File.AppendAllText(outputFileName, csv.ToString());
            }
        }

        private void GenerateTptpFilesForBMC()
        {
            string sourceRootPath = @"Examples";
            string clausegenExeDirName = "clausegen-exe-by-harry";

            string outputDir = "bmc_tptp_files"; //this directory needs to be precreated
            string safetyDirPath = Path.Combine(sourceRootPath, "safety-cond-files");
            string command_bmc = $".\\clausegen.exe -l ..\\Ladder.wt2 -s safety_original.cond --proofstrategy=bmc -b=100 -g yes";

            string[] allCondDirectories = Directory.GetDirectories(safetyDirPath);

            foreach (string condDirectory in allCondDirectories)
            {

                Console.WriteLine(condDirectory);
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

                string[] safetyFileEntries = Directory.GetFiles(condDirectory);

                foreach (string safetyFile in safetyFileEntries)
                {
                    string[] safetyFileNameParts = safetyFile.Split("\\");
                    string originalChapterAndCondFilename = $"{safetyFileNameParts[3]}_{safetyFileNameParts[4]}";
                    string initialFilename = $"{originalChapterAndCondFilename.Split(".").First()}_initial.tptp";
                    string newSafetyFilename = $"{originalChapterAndCondFilename.Split(".").First()}_safety.tptp";

                    File.Copy(safetyFile, Path.Combine(sourceRootPath, clausegenExeDirName, "safety_original.cond"), true); // overwrite any other safety_original.cond file if present

                    Console.WriteLine($">>> Running clausegen for {originalChapterAndCondFilename}");

                    ExecuteClausegenInCmd(Path.Combine(sourceRootPath, clausegenExeDirName), command_bmc);
                    //Thread.Sleep(500);

                    Console.WriteLine($">>> Copying initial.tptp, safety.tptp and BMC.tptp in the output directory for: {originalChapterAndCondFilename}");
                    File.Copy(Path.Combine(sourceRootPath, clausegenExeDirName, "Initial.tptp"), Path.Combine(outputDir, initialFilename), true);
                    //Thread.Sleep(500);
                    File.Copy(Path.Combine(sourceRootPath, clausegenExeDirName, "Safety.tptp"), Path.Combine(outputDir, newSafetyFilename), true);
                    //Thread.Sleep(500);
                    File.Copy(Path.Combine(sourceRootPath, clausegenExeDirName, "BMC.tptp"), Path.Combine(outputDir, originalChapterAndCondFilename.Replace(".cond", ".tptp")), true);
                    //Thread.Sleep(500);
                    Console.WriteLine("============================");

                    // NOTE: to generate and copy inductive verification files, change the clausegen run to inductive in ExecuteClausegenInCmd() manually
                    // and copy over the SafetyStep.tptp file only in place of copying the Initial.tptp and Safety.tptp for BMC.
                }
            }

            Console.WriteLine(">>> Copying ladder.tptp as a final step.");
            //copy the ladder.tptp into the same output directory in the end
            File.Copy(Path.Combine(sourceRootPath, clausegenExeDirName, "Ladder.tptp"), Path.Combine(outputDir, "Ladder.tptp"), true);
        }


        private void GenerateTptpFilesForIV()
        {
            string sourceRootPath = @"Examples";
            string clausegenExeDirName = "clausegen-exe-by-harry";

            string outputDir = "output_tptp_files"; //this directory needs to be precreated
            string safetyDirPath = Path.Combine(sourceRootPath, "safety-cond-files");

            string command =
               $".\\clausegen.exe -l ..\\ladder.wt2 -s safety_original.cond --proofstrategy=inductive -g yes";

            string[] allCondDirectories = Directory.GetDirectories(safetyDirPath);

            foreach (string condDirectory in allCondDirectories)
            {

                Console.WriteLine(condDirectory);
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

                string[] safetyFileEntries = Directory.GetFiles(condDirectory);

                foreach (string safetyFile in safetyFileEntries)
                {
                    string[] safetyFileNameParts = safetyFile.Split("\\");
                    string originalChapterAndCondFilename =
                        $"{safetyFileNameParts[2]}_{safetyFileNameParts[3]}";
                    string initialFilename =
                        $"{originalChapterAndCondFilename.Split(".").First()}_initial.tptp";
                    string newSafetyFilename =
                        $"{originalChapterAndCondFilename.Split(".").First()}_safety.tptp";

                    File.Copy(
                        safetyFile,
                        Path.Combine(sourceRootPath, clausegenExeDirName, "safety_original.cond"),
                        true
                    ); // overwrite any other safety_original.cond file if present

                    Console.WriteLine(
                        $">>> Running clausegen for {originalChapterAndCondFilename}"
                    );
                    _ = ExecuteClausegenInCmd(Path.Combine(sourceRootPath, clausegenExeDirName), command);
                    //Thread.Sleep(500);

                    // Console.WriteLine(
                    //     $">>> Copying initial.tptp, safety.tptp and BMC.tptp in the output directory for: {originalChapterAndCondFilename}"
                    // );
                    // File.Copy(
                    //     Path.Combine(sourceRootPath, clausegenExeDirName, "Initial.tptp"),
                    //     Path.Combine(outputDir, initialFilename),
                    //     true
                    // );
                    // //Thread.Sleep(500);
                    // File.Copy(
                    //     Path.Combine(sourceRootPath, clausegenExeDirName, "Safety.tptp"),
                    //     Path.Combine(outputDir, newSafetyFilename),
                    //     true
                    // );
                    // //Thread.Sleep(500);
                    // File.Copy(
                    //     Path.Combine(sourceRootPath, clausegenExeDirName, "BMC.tptp"),
                    //     Path.Combine(
                    //         outputDir,
                    //         originalChapterAndCondFilename.Replace(".cond", ".tptp")
                    //     ),
                    //     true
                    // );
                    //Thread.Sleep(500);
                    Console.WriteLine("============================");

                    // NOTE: to generate and copy inductive verification files, change the clausegen run to inductive in ExecuteClausegenInCmd() manually
                    // and copy over the SafetyStep.tptp file only in place of copying the Initial.tptp and Safety.tptp for BMC.
                    // Console.WriteLine(originalChapterAndCondFilename);
                    // Console.WriteLine("============================");
                    // Console.WriteLine(newSafetyFilename);
                    Console.WriteLine($"Safety file {safetyFile}");
                    // // safetyFileNameParts
                    // Console.WriteLine("============================");
                    // safetyFileNameParts.ToList().ForEach(part => Console.WriteLine(part));
                    Thread.Sleep(500);
                    File.Copy(
                        Path.Combine(sourceRootPath, clausegenExeDirName, "SafetyStep.tptp"),
                             // Path.Combine(sourceRootPath + outputDir, newSafetyFilename),
                             Path.Combine(
                           sourceRootPath + "\\" + outputDir,
                            // join each safetyFileNameParts with _
                            string.Join("_", safetyFileNameParts).Replace(".cond", "_safetystep.tptp")),
                        true
                    );
                }
            }

            Console.WriteLine(">>> Copying ladder.tptp as a final step.");
            //copy the ladder.tptp into the same output directory in the end
            File.Copy(
                Path.Combine(sourceRootPath, clausegenExeDirName, "Ladder.tptp"),
                Path.Combine(outputDir, "Ladder.tptp"),
                true
            );
        }



    

        private static string[] ExecuteZ3InShell()
        {
            string[] fileNames = new string[]
            {
                "mostyn_original_base.smt",
                "mostyn_original_step.smt",
            };

            string[] z3outputs = new string[] { "n/a", "n/a" };

            for (int i = 0; i < fileNames.Length; i++)
            {
                string argument =
                    $"z3 ~/swansea-uni/SwanLLVerifierForMac/SwanLLVerifierForMac/bin/Debug/net6.0/{fileNames[i]}";

                ProcessStartInfo startInfo = new()
                {
                    FileName = "/bin/bash",
                    Arguments = " -c \"" + argument + " \"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
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

        private static string ExecuteClausegenInCmd(string workingdir, string command)
        {

            System.Diagnostics.ProcessStartInfo startInfo = new()
            {
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                WorkingDirectory = workingdir,
                FileName = "cmd.exe",
                Arguments = "/C " + command,
                RedirectStandardOutput = true,
            };

            string output = "n/a";

            using (Process process = new())
            {
                process.StartInfo = startInfo;
                _ = process.Start();

                output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                Console.WriteLine("**** Clausegen Output ****");
                Console.WriteLine(output);
            }

            return output;
        }
    }
}

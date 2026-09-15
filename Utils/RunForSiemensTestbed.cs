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

            /*
           simple Experiment
           */
            // VerifyPD(
            //     relativePath,
            //     "simpleExample/simpleexamplePD.json",
            //     "simpleExample/program-liveness-example.tptp",
            //     "simpleExample/simpleParadise.tptp",
            //     "simpleExample/simplePD.aag",
            //     new() { },
            //     null,
            //     new Dictionary<string, bool>
            //     {
            //         // initialise latches to false
            //         { "vA_1", false },
            //         { "vB_1", false },
            //     }
            // );

            // VerifyBL(
            //     // S100_QS_1 & vS104_QD_1
            //     MakeVar("vreqB_0"),
            //     MakeVar("vB_1"),
            //     $"{relativePath}simpleExample/program-liveness-example.tptp",
            //     $"{relativePath}simpleExample/simpleBL.tptp",
            //     "simpleParadise.tptp",
            //     "program-liveness-example.tptp",
            //     1
            // );
            /*
            822 Experiment
            */
            // VerifyPD(
            //     relativePath,
            //     "822/PD-down.json",
            //     "822/ladder-file-formats/Ladder.tptp",
            //     "822Experiment/822Paradise.tptp",
            //     "822/PD822.aag",
            //     new()
            // //  {
            // //         "ACTIVE1PRI",
            // //         "ACTIVE2PRI",
            // //         "ACTIVE3PRI",
            // //         "ACTIVE4PRI",
            // //         "ACTIVE5PRI",
            // //         "ACTIVE6PRI",
            // //         "ACTIVE7PRI",
            // //         "ACTIVE8PRI",
            // //         "ACTIVE9PRI",
            // //         "ACTIVE10PRI",
            // //         "ACTIVE11PRI",
            // //         "OPCRFLT",
            // //         "APPDEL",
            // //         "P1602.RHJEN",
            // //         "LP1602.LHK(M)",
            // //         "P1602.RHJEX",
            // //         "LP1602.RHK(M)",
            // //         "ACENSET",
            // //     }
            //     { "ACENSET", "S100.ECRESETEN", "S102.ECRESETEN", "S104.ECRESETEN", "S106.ECRESETEN", "S110.ECRESETEN", "ACTIVE1PRI", "ACTIVE2PRI", "ACTIVE3PRI", "ACTIVE4PRI", "ACTIVE5PRI", "ACTIVE6PRI", "ACTIVE7PRI", "ACTIVE8PRI", "ACTIVE9PRI", "ACTIVE10PRI", "ACTIVE11PRI", "OPCRFLT", "APPDEL", "S100.EC(ASPSIMHI)JEN", "S102.EC(ASPSIMHI)JEN", "S104.EC(ASPSIMHI)JEN", "S106.(SUPPLO)JEN", "S106.EC(ASPSIMHI)JEN", "S106.(SUBLO)JEN", "S106.(UECLO)JEN", "S106.(SILO)JEN", "S106.(MILO)JEN", "S110.EC(ASPSIMHI)JEN", "CYCLEONE", "S100.(DGECBRK1)LO", "S100.(ECBRK1)LO", "S100.(RGECBRK1)LO", "S100.EC(ASPSIM)", "S100.ECRESETEN1(0)", "S100.REC", "S102.(DGECBRK1)LO", "S102.(ECBRK1)LO", "S102.(HGECBRK1)LO", "S102.EC(ASPSIM)", "S102.ECRESETEN1(0)", "S102.HEC", "S104.(DGECBRK1)LO", "S104.(ECBRK1)LO", "S104.(HGECBRK1)LO", "S104.(RGECBRK1)LO", "S104.EC(ASPSIM)", "S104.ECRESETEN1(0)", "S104.REC", "S106.((MI)UECBRK1)LO", "S106.((SI)UECBRK1)LO", "S106.((SUB)ECBRK1)LO", "S106.(DGECBRK1)LO", "S106.(ECBRK1)LO", "S106.(HGECBRK1)LO", "S106.(RGECBRK1)LO", "S106.(TPWSEN1)LO", "S106.(UECBRK1)LO", "S106.EC(ASPSIM)", "S106.ECRESETEN1(0)", "S106.REC", "S106.TSSVC.BRKLO", "S110.(ECBRK1)LO", "S110.(HGECBRK1)LO", "S110.(RGECBRK1)LO", "S110.EC(ASPSIM)", "S110.ECRESETEN1(0)", "S110.REC", "TAA.(CEBRK1)LO", "TAA.(CLRBRK1)LO", "TAA.(LCOBRK1)LO", "TAA.(OCCBRK1)LO", "TAA.(RARBRK1)LO", "TAA.(RBRBRK1)LO", "TAA.(RRBRK1)LO", "TAA.CLR(0)", "TAA.CLRBRK", "TAA.CLRBRKHI(0)", "TAA.CLRBRKLO(0)", "TAA.LCO(0)", "TAA.LCOBRK", "TAA.OCCBRKHI(0)", "TAA.OCCBRKLO(0)", "TAB.(CEBRK1)LO", "TAB.(CLRBRK1)LO", "TAB.(LCOBRK1)LO", "TAB.(OCCBRK1)LO", "TAB.(RARBRK1)LO", "TAB.(RBRBRK1)LO", "TAB.(RRBRK1)LO", "TAB.CLR(0)", "TAB.CLRBRK", "TAB.CLRBRKHI(0)", "TAB.CLRBRKLO(0)", "TAB.LCO(0)", "TAB.LCOBRK", "TAB.OCCBRKHI(0)", "TAB.OCCBRKLO(0)", "TAC.(CEBRK1)LO", "TAC.(CLRBRK1)LO", "TAC.(LCOBRK1)LO", "TAC.(OCCBRK1)LO", "TAC.(RARBRK1)LO", "TAC.(RBRBRK1)LO", "TAC.(RRBRK1)LO", "TAC.CLR(0)", "TAC.CLRBRK", "TAC.CLRBRKHI(0)", "TAC.CLRBRKLO(0)", "TAC.LCO(0)", "TAC.LCOBRK", "TAC.OCCBRKHI(0)", "TAC.OCCBRKLO(0)", "TAD.(CEBRK1)LO", "TAD.(CLRBRK1)LO", "TAD.(LCOBRK1)LO", "TAD.(OCCBRK1)LO", "TAD.(RARBRK1)LO", "TAD.(RBRBRK1)LO", "TAD.(RRBRK1)LO", "TAD.CLR(0)", "TAD.CLRBRK", "TAD.CLRBRKHI(0)", "TAD.CLRBRKLO(0)", "TAD.LCO(0)", "TAD.LCOBRK", "TAD.OCCBRKHI(0)", "TAD.OCCBRKLO(0)", "TAE.(CEBRK1)LO", "TAE.(CLRBRK1)LO", "TAE.(LCOBRK1)LO", "TAE.(OCCBRK1)LO", "TAE.(RARBRK1)LO", "TAE.(RBRBRK1)LO", "TAE.(RRBRK1)LO", "TAE.CLR(0)", "TAE.CLRBRK", "TAE.CLRBRKHI(0)", "TAE.CLRBRKLO(0)", "TAE.LCO(0)", "TAE.LCOBRK", "TAE.OCCBRKHI(0)", "TAE.OCCBRKLO(0)", "TAF.(CEBRK1)LO", "TAF.(CLRBRK1)LO", "TAF.(LCOBRK1)LO", "TAF.(OCCBRK1)LO", "TAF.(RARBRK1)LO", "TAF.(RBRBRK1)LO", "TAF.(RRBRK1)LO", "TAF.CLR(0)", "TAF.CLRBRK", "TAF.CLRBRKHI(0)", "TAF.CLRBRKLO(0)", "TAF.LCO(0)", "TAF.LCOBRK", "TAF.OCCBRKHI(0)", "TAF.OCCBRKLO(0)", "TAG.(CEBRK1)LO", "TAG.(CLRBRK1)LO", "TAG.(LCOBRK1)LO", "TAG.(OCCBRK1)LO", "TAG.(RARBRK1)LO", "TAG.(RBRBRK1)LO", "TAG.(RRBRK1)LO", "TAG.CLR(0)", "TAG.CLRBRK", "TAG.CLRBRKHI(0)", "TAG.CLRBRKLO(0)", "TAG.LCO(0)", "TAG.LCOBRK", "TAG.OCCBRKHI(0)", "TAG.OCCBRKLO(0)", "TBA.(CEBRK1)LO", "TBA.(CLRBRK1)LO", "TBA.(LCOBRK1)LO", "TBA.(OCCBRK1)LO", "TBA.(RARBRK1)LO", "TBA.(RBRBRK1)LO", "TBA.(RRBRK1)LO", "TBA.CLR(0)", "TBA.CLRBRK", "TBA.CLRBRKHI(0)", "TBA.CLRBRKLO(0)", "TBA.LCO(0)", "TBA.LCOBRK", "TBA.OCCBRKHI(0)", "TBA.OCCBRKLO(0)", "TCA.(CEBRK1)LO", "TCA.(CLRBRK1)LO", "TCA.(LCOBRK1)LO", "TCA.(OCCBRK1)LO", "TCA.(RARBRK1)LO", "TCA.(RBRBRK1)LO", "TCA.(RRBRK1)LO", "TCA.CLR(0)", "TCA.CLRBRK", "TCA.CLRBRKHI(0)", "TCA.CLRBRKLO(0)", "TCA.LCO(0)", "TCA.LCOBRK", "TCA.OCCBRKHI(0)", "TCA.OCCBRKLO(0)", "TDA.(CEBRK1)LO", "TDA.(CLRBRK1)LO", "TDA.(LCOBRK1)LO", "TDA.(OCCBRK1)LO", "TDA.(RARBRK1)LO", "TDA.(RBRBRK1)LO", "TDA.(RRBRK1)LO", "TDA.CLR(0)", "TDA.CLRBRK", "TDA.CLRBRKHI(0)", "TDA.CLRBRKLO(0)", "TDA.LCO(0)", "TDA.LCOBRK", "TDA.OCCBRKHI(0)", "TDA.OCCBRKLO(0)", "TEA.(CEBRK1)LO", "TEA.(CLRBRK1)LO", "TEA.(LCOBRK1)LO", "TEA.(OCCBRK1)LO", "TEA.(RARBRK1)LO", "TEA.(RBRBRK1)LO", "TEA.(RRBRK1)LO", "TEA.CLR(0)", "TEA.CLRBRK", "TEA.CLRBRKHI(0)", "TEA.CLRBRKLO(0)", "TEA.LCO(0)", "TEA.LCOBRK", "TEA.OCCBRKHI(0)", "TEA.OCCBRKLO(0)", "TEST(FADC).CLRBRKHI(0)", "TEST(FADC).CLRBRKLO(0)", "TEST(FADC).OCCBRKHI(0)", "TEST(FADC).OCCBRKLO(0)", "TFA.(CEBRK1)LO", "TFA.(CLRBRK1)LO", "TFA.(LCOBRK1)LO", "TFA.(OCCBRK1)LO", "TFA.(RARBRK1)LO", "TFA.(RBRBRK1)LO", "TFA.(RRBRK1)LO", "TFA.CLR(0)", "TFA.CLRBRK", "TFA.CLRBRKHI(0)", "TFA.CLRBRKLO(0)", "TFA.LCO(0)", "TFA.LCOBRK", "TFA.OCCBRKHI(0)", "TFA.OCCBRKLO(0)", "TGA.(CEBRK1)LO", "TGA.(CLRBRK1)LO", "TGA.(LCOBRK1)LO", "TGA.(OCCBRK1)LO", "TGA.(RARBRK1)LO", "TGA.(RBRBRK1)LO", "TGA.(RRBRK1)LO", "TGA.CLR(0)", "TGA.CLRBRK", "TGA.CLRBRKHI(0)", "TGA.CLRBRKLO(0)", "TGA.LCO(0)", "TGA.LCOBRK", "TGA.OCCBRKHI(0)", "TGA.OCCBRKLO(0)", "THA.(CEBRK1)LO", "THA.(CLRBRK1)LO", "THA.(LCOBRK1)LO", "THA.(OCCBRK1)LO", "THA.(RARBRK1)LO", "THA.(RBRBRK1)LO", "THA.(RRBRK1)LO", "THA.CLR(0)", "THA.CLRBRK", "THA.CLRBRKHI(0)", "THA.CLRBRKLO(0)", "THA.LCO(0)", "THA.LCOBRK", "THA.OCCBRKHI(0)", "THA.OCCBRKLO(0)", "TAA.CLR(0-1)", "TAB.CLR(0-1)", "TAC.CLR(0-1)", "TAD.CLR(0-1)", "TAE.CLR(0-1)", "TAF.CLR(0-1)", "TAG.CLR(0-1)", "TBA.CLR(0-1)", "TCA.CLR(0-1)", "TDA.CLR(0-1)", "TEA.CLR(0-1)", "TFA.CLR(0-1)", "TGA.CLR(0-1)", "THA.CLR(0-1)", "S100.EC(ASPSIMHI)JEX", "S102.EC(ASPSIMHI)JEX", "S104.EC(ASPSIMHI)JEX", "S106.(SUPPLO)JEX", "S106.EC(ASPSIMHI)JEX", "S106.(SUBLO)JEX", "S106.(UECLO)JEX", "S106.(SILO)JEX", "S106.(MILO)JEX", "S110.EC(ASPSIMHI)JEX", "P1602.LHJEN", "P1604.RHJEN", "P1606.RHJEN", "P1608.RHJEN", "P1620.RHJEN", "P1622.LHJEN", "P1624.LHJEN", "P1602.LHJEX", "P1604.RHJEX", "P1606.RHJEX", "P1608.RHJEX", "P1620.RHJEX", "P1622.LHJEX", "P1624.LHJEX", "LP1602.LHK(M)", "LP1604.RHK(M)", "LP1606.RHK(M)", "LP1608.RHK(M)", "LP1620.RHK(M)", "LP1622.LHK(M)", "LP1624.LHK(M)" },
            // new Dictionary<string, int>() {
            //     {"TAC.UK", 1},
            //     {"P1602.NUK", 1},
            //     {"P1620.NUK", 1},
            //     {"TAF.UK"   , 1},
            //     {"TAG.UK"   , 1}
            // }
            // );

            // VerifyBL(
            //     // S100_QS_1 & vS104_QD_1
            //     MakeAnd(MakeVar("vS100_QS_0"), MakeVar("vS104_QD_0")),
            //     MakeVar("vS100_RU_1"),
            //     $"{relativePath}822Experiment/Ladder822.tptp",
            //     $"{relativePath}822Experiment/S100_AM_RU.tptp",
            //     "822Paradise.tptp",
            //     "Ladder822.tptp",
            //     22
            // );
            // RunForLochNess();
            // RunForMostyn();
            // GenerateTptpFiles();
            // RunClausegenInductiveVerification();


            string sourceRootPath = relativePath + "SiemensData";
            // string lochnessTrackPlanXmlPath = Path.Combine(
            //     sourceRootPath,
            //     "LochNess-810",
            //     "810.xml"
            // );
            // string lochnessTrackPlanTptpPath = Path.Combine(
            //     sourceRootPath,
            //     "LochNess-810",
            //     "Ladder_810.tptp"
            // );
            string lochnessTrackPlanTptpPath = Path.Combine(
                sourceRootPath,
                "810",
                "Ladder.tptp"
            );

            string TestTptpPath = Path.Combine(
                sourceRootPath,
                "TestInterlocking",
                "Ladder.tptp"
            );

            string LevelCrossingTptpPath = Path.Combine(
                sourceRootPath,
                "LevelCrossing",
                "LevelCrossing.tptp"
            );

            string CounterExampleTptpPath = Path.Combine(
                sourceRootPath,
                "CounterExample",
                "Ladder.tptp"
            );

            // string lochnessSafetyDirPath = Path.Combine(
            //     sourceRootPath,
            //     "Additional_LochNess_Properties"
            // );
            string lochnessTptpSafetyDirPath = Path.Combine(
                sourceRootPath,
                "810/1_Property"
            );

            string TestSafetyDirPath = Path.Combine(
                sourceRootPath,
                "TestInterlocking/SafetyProperties"
            );

            string LevelCrossingSafetyDirPath = Path.Combine(
                sourceRootPath,
                "LevelCrossing/SafetyProperties"
            );

            
            string CounterExampleSafetyDirPath = Path.Combine(
                sourceRootPath,
                "CounterExample/SafetyProperties"
            );

            string WADT810TptpPath = Path.Combine(
                sourceRootPath,
                "810",
                "810.tptp"
            );

            string WADT810SafetyDirPath = Path.Combine(
                sourceRootPath,
                "810/WADT_A"
            );

            // string outputFilepath = "lochness810_tptp_ic3_output_26_july_2.csv";
            string outputFilepath = "810_A.csv";

             RunForLochNess(
             relativePath,
             sourceRootPath,
             WADT810TptpPath,
             WADT810SafetyDirPath,
             outputFilepath
             );

            // turned off 27/07
            // RunClausegenInductiveVerification();

            // GenerateTptpFilesForBMC();
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
            //include('822Paradise.tptp') .
            // include('Ladder822.tptp') .
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

        public static void RunFor822()
        {
            //string[] allMostynCondDirectories = Directory.GetDirectories(mostynSafetyDirPath);

            //XmlDocument doc = new XmlDocument();
            //doc.Load(mostynTrackPlanXmlPath);

            //Ladder ladder = LadderLogicXmlParser.ParseXML(doc);
            Ladder ladder;

            Console.WriteLine(Environment.CurrentDirectory);

            string relativePath = Environment.CurrentDirectory.EndsWith("net6.0")
                ? "../../../"
                : "";
            using (
                FileStream fileStream = new(
                    $"{relativePath}822/ladder-file-formats/Ladder.tptp",
                    FileMode.Open,
                    FileAccess.Read
                )
            )
            {
                ladder = LadderParser.ParseLadder(fileStream);
            }

            Model modelForOrgLadder = new(ladder);
            modelForOrgLadder.InitialiseModel();

            Ladder transformedLadder = TransformToAig.TransformLadder(ladder);
            Model modelForTfLadder = new(transformedLadder);
            modelForTfLadder.InitialiseModel();

            string PDpath = $"{relativePath}822/PD-down.json";
            string fileName = $"{relativePath}822/PD822.aag";
            string TpTpFileName = $"{relativePath}822/TpTp/822Paradise.tptp";
            string pdFileName = $"{relativePath}822_PD_states.txt";
            string jsonFileName = $"{relativePath}822/PD_normalized-down.json";
            string varsfileName = $"{relativePath}822/all_vars.txt";

            var addedPDict = new Dictionary<string, string>
            {
                // manually add to addedPDdict
                // ["TAC__OCCEXT_JS_0"] = "0",
                // ["vP1602_NLZ_0"] = "1",
                // ["vP1602_KN_0"] = "1"
            };

            // Console.WriteLine("Ladder parsed and transformed.");
            // Console ladder inputs
            ISet<string> outputVariables = transformedLadder.AllOutputVariables();
            ISet<string> allInputsAndLatches = transformedLadder.AllVariables();
            // all inputs
            ISet<string> allInputs = transformedLadder.AllInputs();
            // write the variable to a file
            File.WriteAllLines(
                varsfileName,
                new[]
                {
                    "Inputs: \n"
                        + string.Join(", \n", allInputs)
                        + "\nOutputVariable: \n"
                        + string.Join(", \n", outputVariables),
                }
            );

            // Check if any varss contains 'JS'
            var jsVars = outputVariables.Where(v => v.EndsWith("JS")).ToList();
            var jrVars = allInputs.Where(v => v.EndsWith("JR")).ToList();
            // exclude jsVars from varss
            ISet<string> ouputVariablesWithoutJS = outputVariables.Except(jsVars).ToHashSet();

            // addedPDdict["TAC_UK"] = "1";
            string PDjson = File.ReadAllText(PDpath);
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
                // key = AigConstructor.FormatVarName(key);
                // if key already exists in PDdict, console a warning and skip it
                // if (!PDdict.ContainsKey(key)) // if the PD was copied in top bottom manner - it should be replaced by the latest one
                // {
                //     // Console.WriteLine($"Warning: Duplicate key {key} found in PD.json. Skipping.");
                //     PDdict[key] = kv.Value;

                // }
                PDdict[key] = kv.Value;
            }

            // format all kesys in addedPDdict
            // replace keys in addedPDdict with formatted keys
            var formattedAddedPD = new Dictionary<string, string>();
            foreach (var kv in addedPDict)
            {
                var formattedKey = AigConstructor.FormatVarName(kv.Key);
                if (!allInputsAndLatches.Contains(formattedKey))
                {
                    throw new Exception(
                        $"In added PDdict. You expected a key to be in the variables but it's not. It's missing. The missing key: {kv.Key}"
                    );
                }
                formattedAddedPD[formattedKey] = kv.Value;
            }
            addedPDict = formattedAddedPD;

            // write the PDdict keys and values as json file
            File.WriteAllText(
                jsonFileName,
                JsonSerializer.Serialize(PDdict, new JsonSerializerOptions { WriteIndented = true })
            );

            Console.WriteLine("PD states:");

            List<string> pdStatesWithoutJSandInputs = new();
            List<string> TruthValuesPdStates = new();
            List<string> falsePdStatesWithoutJSandInputs = new();
            List<string> pdStatesAndInputsWithTruthValues = new();
            List<string> jrVarswithTruthValues = new();
            List<string> JSVarswithTruthValues = new();

            List<string> ExcludePdKeys = new()
            {
                "ACTIVE1PRI",
                "ACTIVE2PRI",
                "ACTIVE3PRI",
                "ACTIVE4PRI",
                "ACTIVE5PRI",
                "ACTIVE6PRI",
                "ACTIVE7PRI",
                "ACTIVE8PRI",
                "ACTIVE9PRI",
                "ACTIVE10PRI",
                "ACTIVE11PRI",
                "OPCRFLT",
                "APPDEL",
                "P1602.RHJEN",
                "LP1602.LHK(M)",
                "P1602.RHJEX",
                "LP1602.RHK(M)",
                "ACENSET",
            };

            // Now check if all PDdict keys are present in allInputsAndLatches
            File.WriteAllText(pdFileName, ""); // empty the file first

            foreach (var kv in PDdict)
            {
                string theKey = AigConstructor.FormatVarName(kv.Key);
                File.AppendAllText(pdFileName, $"{theKey}:{kv.Value}\n");
                var theValue = kv.Value;
                // if the key is in addExcludePdKeys, replace it
                if (addedPDict.ContainsKey(theKey))
                {
                    theValue = JsonDocument.Parse(addedPDict[theKey]).RootElement;
                }

                if (ExcludePdKeys.Contains(kv.Key))
                {
                    continue;
                }
                // string theKey = kv.Key;
                if (!allInputsAndLatches.Contains(theKey))
                {
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
                    if (outputVariables.Contains(theKey))
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
                    // if it's input skip it
                    if (ouputVariablesWithoutJS.Contains(theKey))
                        falsePdStatesWithoutJSandInputs.Add(theKey);
                    // pdStatesAndInputsWithTruthValues.Add(theKey);
                }
            }

            // extract variables that are not in pdStates
            List<string> nonPdStatesWithoutJSandInputs = ouputVariablesWithoutJS
                .Where(v => !pdStatesWithoutJSandInputs.Contains(v))
                .ToList();
            List<string> FalseValuesNonPdStates = outputVariables
                .Where(v => !TruthValuesPdStates.Contains(v))
                .ToList();
            List<string> JRnonPdStates = jrVars
                .Where(v => !jrVarswithTruthValues.Contains(v))
                .ToList();
            List<string> JSnonPdStates = jsVars
                .Where(v => !JSVarswithTruthValues.Contains(v))
                .ToList();

            using (StreamWriter writer = new(TpTpFileName))
            {
                foreach (var state in TruthValuesPdStates)
                {
                    writer.WriteLine($"fof(ax,axiom, v{state}_0).");
                }
                foreach (var state in FalseValuesNonPdStates)
                {
                    writer.WriteLine($"fof(ax,axiom, ~v{state}_0).");
                }
            }

            // safety condition is Negation of (MakeAnd of all pdStates and MakeAnd of negation of all nonPdStates)
            // AbstractFirstOrderFormula safetyCondition = MakeNegation(MakeAnd(
            //     MakeAnd(pdStates.Select(ps => MakeVar(ps)).ToList()),
            //     MakeAnd(nonPdStates.Select(nps => (AbstractFirstOrderFormula)MakeNegation(MakeVar(nps))).ToList())
            //     ));

            //using single MakeAnd
            AbstractFirstOrderFormula safetyCondition = MakeNegation(
                MakeAnd(
                    TruthValuesPdStates
                        .Select(ps => MakeVar(ps))
                        .ToList()
                        .Concat(
                            FalseValuesNonPdStates
                                .Select(nps =>
                                    (AbstractFirstOrderFormula)MakeNegation(MakeVar(nps))
                                )
                                .ToList()
                        )
                        .ToList()
                // .Concat(
                // // create a new list
                // new List<AbstractFirstOrderFormula>()
                // {
                //     MakeNegation(MakeVar("vTAC__OCCEXT_JR_0")) // just to avoid empty list issue
                // }
                // ).ToList()
                )
            );

            // other way round
            //  AbstractFirstOrderFormula safetyCondition = MakeNegation(MakeAnd(
            //     MakeAnd(pdStates.Select(ps => (AbstractFirstOrderFormula)MakeNegation(MakeVar(ps))).ToList()),
            //     MakeAnd(nonPdStates.Select(nps => MakeVar(nps)).ToList())
            //     ));

            // only pd states
            // AbstractFirstOrderFormula safetyCondition = MakeNegation(
            //     MakeAnd(pdStates.Select(ps => MakeVar(ps)).ToList())
            // );

            // All var 0
            // AbstractFirstOrderFormula safetyCondition = MakeNegation(MakeAnd(
            //  varss.Select(ps => (AbstractFirstOrderFormula)MakeNegation(MakeVar(ps))).ToList()
            //  ));

            /// INTERNAL WORKINGS - DO NOT TOUCH
            AbstractFirstOrderFormula negatedSafety = MakeNegation(safetyCondition);
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
                    // print all rungs with their outputs
                    foreach (var rung in transformedLadder.Rungs)
                    {
                        //IL822__EN_JS
                        if (rung.output == "IL822__EN_JS")
                        {
                            Console.WriteLine("Found the rung with output IL822__EN_JS");
                            PrettyPrinter.PrettyPrintRung(rung);
                            // int literal = aigConstructor.DecorateFormulaTree(rung.formula, "");
                            // Console.WriteLine($"Literal for IL822__EN_JS formula: {literal}");
                        }
                        // Console.WriteLine($"Rung output: {rung.output}");
                    }
                    aigConstructor.ConstructAigerFile(fileName);

                    Program.RunIC3(fileName);
                },
                16 * 1024 * 1024
            ); // 16 MB stack size
            thread.Start();
            thread.Join(10000); // 10 seconds timeout
        }

        //}

        public static void RunForMostyn()
        {
            string sourceRootPath = @"SiemensData";
            //string mostynTrackPlanXmlPath = Path.Combine(sourceRootPath, "Mostyn_946_Data", "xml_versions", "946_v5.xml");
            string mostynTrackPlanTptpPath = Path.Combine(sourceRootPath, "Mostyn_946_Data", "Ladder_946_v11.tptp");

            // string mostynSafetyDirPath = Path.Combine(
            //     sourceRootPath,
            //     "Additional_Mostyn_Properties"
            // );
            string mostynTptpSafetyDirPath = Path.Combine(
                sourceRootPath,
                "Mostyn_946_Data",
                "mostyn_iv_tptp_files"
            ); // this pre-existing directory should have all safety_step.tptp files
            string mostynOutputFilepath = "mostyn_tptp_iv_output_31_jul.csv";

            // empty the output file content on every new run
            File.WriteAllText(mostynOutputFilepath, "");

            //string[] allMostynCondDirectories = Directory.GetDirectories(mostynSafetyDirPath);

            //XmlDocument doc = new XmlDocument();
            //doc.Load(mostynTrackPlanXmlPath);

            //Ladder ladder = LadderLogicXmlParser.ParseXML(doc);
            Ladder ladder;

            using (
                FileStream fileStream = new(mostynTrackPlanTptpPath, FileMode.Open, FileAccess.Read)
            )
            {
                ladder = LadderParser.ParseLadder(fileStream);
            }

            Model modelForOrgLadder = new(ladder);
            modelForOrgLadder.InitialiseModel();

            Ladder transformedLadder = TransformToAig.TransformLadder(ladder);
            Model modelForTfLadder = new(transformedLadder);
            modelForTfLadder.InitialiseModel();

            //foreach (string mostynCondDirectory in allMostynCondDirectories)
            //{
            //    string dirName = mostynCondDirectory.Split("/").Last();

            //    Console.WriteLine(mostynCondDirectory);
            //    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

            string[] mostynCondFileEntries = Directory.GetFiles(mostynTptpSafetyDirPath);

            var csv = new StringBuilder();

            foreach (string mostynSafetyFile in mostynCondFileEntries)
            {
                string condFileName = mostynSafetyFile.Split("/").Last();

                // the safety condition is always on the 5th line of the .cond file
                //string safetyPropertyString = File.ReadLines(mostynSafetyFile).Skip(4).Take(1).First();
                //AbstractFirstOrderFormula safetyCondition = SafetyPropertyParser.Parse(safetyPropertyString);

                AbstractFirstOrderFormula safetyCondition = MakeVar("null");
                // parse safety from TPTP format
                try
                {
                    safetyCondition = ConditionParser.ParseTptpSafety(mostynSafetyFile);
                }
                catch (Exception ex)
                {
                    var exceptionLine = string.Format("{0},{1}", mostynSafetyFile, ex.Message);
                    _ = csv.AppendLine(exceptionLine);
                    continue; // log the exception and move on
                }

                AbstractFirstOrderFormula negatedSafety = MakeNegation(safetyCondition);
                AbstractFirstOrderFormula transformedNegSafety = TransformToAig.Transform(
                    negatedSafety
                );

                int ic3Output = 0;
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

                try
                {
                    AigConstructor aigConstructor = new(
                        transformedLadder,
                        transformedNegSafety,
                        modelForTfLadder.LatchNamesAndValues
                    );
                    aigConstructor.Decorate();
                    aigConstructor.ConstructAigerFile();

                    Thread.Sleep(100);

                    watch.Start();
                    ic3Output = Program.RunIC3();
                    watch.Stop();

                    //Console.WriteLine($">>> Processing .cond file {condFileName}.");
                    Console.WriteLine($">>> Processing .tptp file {condFileName}.");

                    // ========== in your loop
                    //chapterName = $"{dirName}_{condFileName}";
                    doesPropHoldInIC3 = (ic3Output == 1) ? "no" : "yes";
                    elapsedTime = (watch.Elapsed.TotalSeconds);
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

                //try
                //{
                //    SMTLibUtil.ToSMTLibInductive(ladder, negatedSafety, "mostyn_original");

                //    watch.Start();
                //    string[] z3Results = ExecuteZ3InShell();
                //    watch.Stop();

                //    elapsedTime = watch.Elapsed.TotalSeconds;

                //    originalIVInitResult = z3Results[0].Replace("\n", String.Empty);
                //    originalIVStepResult = z3Results[1].Replace("\n", String.Empty);

                //    ivResult = ((originalIVInitResult == "unsat") && (originalIVStepResult == "unsat")) ? "yes" : "no";
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine($">>>>>>>>>>>>>>> EXCEPTION: {ex.Message}");
                //}

                var newLine = string.Format(
                    "{0},{1},{2}",
                    mostynSafetyFile,
                    doesPropHoldInIC3,
                    elapsedTime
                );
                //var newLine = string.Format("{0},{1},{2},{3}", mostynCondFile, originalIVInitResult, originalIVStepResult, ivResult);

                _ = csv.AppendLine(newLine);


                File.AppendAllText(mostynOutputFilepath, csv.ToString());
            }

            File.WriteAllText(mostynOutputFilepath, ""); // empty the file first

            File.AppendAllText(mostynOutputFilepath, csv.ToString());
            //}
        }

        public static void RunForLochNess(string relativePath, string sourceRootPath, string lochnessTrackPlanTptpPath, string lochnessTptpSafetyDirPath, 
            string outputFilepath, IDictionary<string, bool>? initialisedLatches = null)
        {

            // empty the output file content on every new run
            //File.WriteAllText(outputFilepath, "");

            // string[] allLochnessCondDirectories = Directory.GetDirectories(
            //     lochnessTptpSafetyDirPath
            // );

            Ladder ladder;
            // *** the tptp way ***
            using (
                FileStream fileStream = new(
                    lochnessTrackPlanTptpPath,
                    FileMode.Open,
                    FileAccess.Read
                )
            )
            {
                ladder = LadderParser.ParseLadder(fileStream);
            }

            // *** the xml way ***
            //XmlDocument doc = new XmlDocument;
            //doc.Load(lochnessTrackPlanXmlPath);
            //Ladder ladder = LadderLogicXmlParser.ParseXML(doc);

            //foreach (string lochnessCondDirectory in allLochnessCondDirectories)
            //{
            //    if (lochnessCondDirectory.Contains("822"))
            //        continue;

            //    Console.WriteLine(lochnessCondDirectory);
            //    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

            string[] lochnessSafetyFileEntries = Directory.GetFiles(lochnessTptpSafetyDirPath);

            var csv = new StringBuilder();

            foreach (string lochnessSafetyFile in lochnessSafetyFileEntries)
            {
                string condFileName = lochnessSafetyFile.Split("/").Last();

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
                String ladderFileName = Path.GetFileName(Path.GetDirectoryName(lochnessTrackPlanTptpPath)!);
                String condFileNameWithoutExt = Path.GetFileNameWithoutExtension(condFileName);
                String verificationCondtion = ladderFileName + "_" + condFileNameWithoutExt;

                // if (initialisedLatches != null)
                // {
                //     // iterate through the dictionary and set the Initialised property of the corresponding rungs
                //     foreach (var kv in initialisedLatches)
                //     {
                //         Rung? rung = ladder.Rungs.FirstOrDefault(r => r.output == kv.Key);
                //         if (rung != null)
                //         {
                //             rung.Initialised = kv.Value;
                //         }
                //         else
                //         {
                //             throw new Exception(
                //                 $"Ladder does not contain a rung with output variable '{kv.Key}'"
                //             );
                //         }
                //     }
                // } else
                // {
                //     foreach (Rung r in ladder.Rungs)
                //     {
                //         if (r != null)
                //         {
                //             r.Initialised = false;
                //         }
                //     }
                // }

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

                Console.WriteLine(lochnessSafetyFile);
                Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>");

                Model modelForOrgLadder = new(ladder);
                modelForOrgLadder.InitialiseModel();

                Ladder transformedLadder = TransformToAig.TransformLadder(ladder);
                Model modelForTfLadder = new(transformedLadder);
                modelForTfLadder.InitialiseModel();

                // the safety condition is always on the 5th line of the .cond file
                //string safetyPropertyString = File.ReadLines(lochnessSafetyFile).Skip(4).Take(1).First();
                //AbstractFirstOrderFormula safetyCondition = SafetyPropertyParser.Parse(safetyPropertyString);

                AbstractFirstOrderFormula safetyCondition = MakeVar("null");
                // parse safety from TPTP format
                try
                {
                    safetyCondition = ConditionParser.ParseTptpSafety(lochnessSafetyFile);
                    PrettyPrinter.PrettyPrint(safetyCondition);
                }
                catch (Exception ex)
                {
                    var exceptionLine = string.Format("{0},{1}", lochnessSafetyFile, ex.Message);
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
                    //SMTLibUtil.ToSMTLibInductive(ladder, transformedNegSafety, "lochness_transformed");
                    //SMTLibUtil.ToSMTLibInductive(transformedLadder, transformedNegSafety, "mostyn_transformed");
                    //SMTLibUtil.ToSMTLibBoundedModelChecking(ladder, negatedSafety, 100, "lochness810_transformed");

                //    string[] z3Results = ExecuteZ3InShell();

                //    originalIVInitResult = z3Results[0].Replace("\n", String.Empty);
                //    originalIVStepResult = z3Results[1].Replace("\n", String.Empty);
                //    //transformedIVInitResult = z3Results[2].Replace("\n", String.Empty);
                //    //transformedIVStepResult = z3Results[3].Replace("\n", String.Empty);

                //    ivResult = ((originalIVInitResult == "unsat") && (originalIVStepResult == "unsat")) ? "yes" : "no";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($">>>>>>>>>>>>>>> EXCEPTION: {ex.Message}");
                }

                var newLine = string.Format(
                    "{0},{1},{2}",
                    lochnessSafetyFile,
                    doesPropHoldInIC3,
                    elapsedTime
                );

                _ = csv.AppendLine(newLine);
                // append to output file after every safety property processed to avoid data loss in case of any unexpected crash before the end of the loop
                //File.WriteAllText(outputFilepath, csv.ToString());
            }

            //File.WriteAllText(outputFilepath, ""); // empty the file first before writing the final results

            //File.AppendAllText(outputFilepath, csv.ToString());
            //}
        }

        private void RunClausegenInductiveVerification()
        {
            string sourceRootPath = @"SiemensData";
            string clausegenExeDirName = "clausegen-exe-by-harry";

            // string outputFileName = "old_tptp_verifier_iv_results.csv"; //this directory needs to be precreated
            // string safetyDirPath = Path.Combine(sourceRootPath, "LochNess-822\\safety-cond-files");
            // string command =
            //                $".\\clausegen.exe -l ..\\LochNess-822\\ladder822_Old.wt2 -s safety_original.cond --proofstrategy=inductive -g yes";

            string outputFileName = "810_old_tptp_verifier_iv_results.csv"; //this directory needs to be precreated
            string safetyDirPath = Path.Combine(sourceRootPath, "LochNess-810\\safety-cond-files");
            string command =
                           $".\\clausegen.exe -l ..\\LochNess-810\\810.wt2 -s safety_original.cond --proofstrategy=inductive -g yes";


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
            string sourceRootPath = @"SiemensData";
            string clausegenExeDirName = "clausegen-exe-by-harry";

            // string outputDir = "SiemensData\\Mostyn_946_Data\\mostyn_v11_bmc_tptp_files"; //this directory needs to be precreated
            // string safetyDirPath = Path.Combine(sourceRootPath, "Mostyn_946_Data\\Additional_Mostyn_Properties");
            // string command_bmc = $".\\clausegen.exe -l ..\\Mostyn_946_Data\\946_v11.wt2 -s safety_original.cond --proofstrategy=bmc -b=100 -g yes";

            // string outputDir = "SiemensData\\LochNess-822\\lochness_822_bmc_tptp_files"; //this directory needs to be precreated
            // string safetyDirPath = Path.Combine(sourceRootPath, "LochNess-822\\safety-cond-files");
            // string command_bmc = $".\\clausegen.exe -l ..\\LochNess-822\\ladder822_Old.wt2 -s safety_original.cond --proofstrategy=bmc -b=100 -g yes";

            string outputDir = "SiemensData\\LochNess-810\\lochness_810_bmc_tptp_files"; //this directory needs to be precreated
            string safetyDirPath = Path.Combine(sourceRootPath, "LochNess-810\\safety-cond-files");
            string command_bmc = $".\\clausegen.exe -l ..\\LochNess-810\\810.wt2 -s safety_original.cond --proofstrategy=bmc -b=100 -g yes";

            string[] allCondDirectories = Directory.GetDirectories(safetyDirPath);

            foreach (string condDirectory in allCondDirectories)
            {
                //if (condDirectory.Contains("822"))
                //    continue;

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
                    // ..\\LochNess-822\\ladder822_Old.wt2

                    // string command_bmc = $".\\clausegen.exe -l 946_v11.wt2 -s safety_original.cond --proofstrategy=bmc -b=100 -g yes";

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
            string sourceRootPath = @"SiemensData";
            string clausegenExeDirName = "clausegen-exe-by-harry";

            string outputDir = "LochNess-822\\822_tptp_files"; //this directory needs to be precreated
            string safetyDirPath = Path.Combine(sourceRootPath, "LochNess-822\\safety-cond-files");

            string command =
               $".\\clausegen.exe -l ..\\LochNess-822\\ladder822_Old.wt2 -s safety_original.cond --proofstrategy=inductive -g yes";
            //string command_bmc = $".\\clausegen.exe -l 946_v11.wt2 -s safety_original.cond --proofstrategy=bmc -b=100 -g yes";



            string[] allCondDirectories = Directory.GetDirectories(safetyDirPath);

            foreach (string condDirectory in allCondDirectories)
            {
                //if (condDirectory.Contains("822"))
                //    continue;

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
            //string[] fileNames = new string[] { "lochness810_original_base.smt", "lochness810_original_step.smt" };

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
            // string command =
            //     $".\\clausegen.exe -l ..\\LochNess-822\\ladder822_Old.wt2 -s safety_original.cond --proofstrategy=inductive -g yes";
            //string command_bmc = $".\\clausegen.exe -l 946_v11.wt2 -s safety_original.cond --proofstrategy=bmc -b=100 -g yes";

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

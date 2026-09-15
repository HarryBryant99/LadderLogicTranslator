using System.Diagnostics;
using System.Text.Json;
using System.Xml;
using SwanLLVerifier.AIG;
//using SwanLLVerifier.ETCSDC_Properties;
using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;
using SwanLLVerifier.LadderLogic;
using SwanLLVerifier.SafetyProperty;
using SwanLLVerifier.SMTLib;
using SwanLLVerifier.TptpParser;
using SwanLLVerifier.Utils;
using static SwanLLVerifier.PropositionalLogic.PropositionalFormulaBuilder;

public static class Program
{
    public static int RunIC3(string fileName = "test.aag")
    {
        // measure time taken by the following block
        Stopwatch stopwatch = Stopwatch.StartNew();
        string currentDirectory = Environment.CurrentDirectory;
        Console.WriteLine("Current Directory: " + currentDirectory);
        if (!Directory.Exists("results"))
        {
            Directory.CreateDirectory("results");
        }
        fileName = Path.Combine("results", fileName);
        string argument = Path.Combine(currentDirectory, fileName);

        ProcessStartInfo startInfo = new()
        {
            FileName = @"C:\Windows\system32\wsl.exe",
            Arguments =
                $"-d Ubuntu-24.04 -- /home/micheal/ic3/IC3 < {argument.Replace('\\', '/').Replace("C:", "/mnt/c")}",
            // Arguments = "-d Ubuntu-24.04 -- /home/micheal/ic3/IC3 < /home/micheal/ic3/test.aag",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };

        Console.WriteLine(
            "Executing command: File: " + startInfo.FileName + " Arguments: " + startInfo.Arguments
        );

        Process proc = new() { StartInfo = startInfo };
        _ = proc.Start();

        string output = proc.StandardOutput.ReadToEnd();
        string error = proc.StandardError.ReadToEnd();

        proc.WaitForExit();

        Console.WriteLine(
            "########################### IC3 OUTPUT FROM FILE: "
                + fileName
                + " ###########################"
        );
        // Console.WriteLine("Exit Code: " + proc.ExitCode);
        Console.WriteLine("Output:");
        Console.WriteLine(output);
        if (proc.ExitCode != 0)
        {
            Console.WriteLine("Error:");
            Console.WriteLine(error);
            // Optionally exit the program if desired:
            // Environment.Exit(proc.ExitCode);
        }
        stopwatch.Stop();
        Console.WriteLine($"Time taken by IC3: {stopwatch.ElapsedMilliseconds} ms");
        if (output.Contains('0'))
            return 0;
        else if (output.Contains('1'))
            return 1;
        else
        {
            throw new Exception("Unexpected output from IC3: " + output);
        }
    }

    public static void Main(string[] args)
    {
        // TestExample1();
        //TestExample2();
        //TestExample3();
        //TestExample4();

        // TestExample1Mic();
        // QTestExample1Mic();

        // TestExample2Mic();
        // TestExample2MofifiedMic();

        // TestExample3Mic();
        // QTestExample3Mic();

        // HypoPExample1();

        // HypoTestExample3Mic();
        // HyPoQTestExample3Mic();

        // TestExample4Mic();
        // QTestExample4Mic();

        // TestExample5Mic();
        // QTestExample5Mic();

        // TestExample5Pelican();
        // TestExample5Pelican2();
        // QTestExample5Pelican();

        // PragunExample1();

        string relativePath = Environment.CurrentDirectory.EndsWith("net6.0") ? "../../../" : "";

        //AbstractFirstOrderFormula generatedFormula = RandomFormulaGenerator.Generate(100);
        //PrettyPrinter.PrettyPrint(generatedFormula);

        RunForSiemensTestbed rfstb = new();
        // RunMultipleTest();
        // applyAigmoveAndAigResetThenRunIC3("results/Examples/exHarry-p0.aag");
        // applyAigmoveAndAigResetThenRunIC3("results/Examples/exHarry-p1.aag");
        // applyAigmoveAndAigResetThenRunIC3("results/Examples/exHarry-p2.aag");

        // TestSimpleExampleMicTpTp(
        //     relativePath: relativePath,
        //     fileName: "simpleExample/PDExampleReachability.aag",
        //     ladderPath: "simpleExample/program-liveness-example.tptp",
        //     safetyPropertyString: "~(\"A_0\" & \"B_1\")",
             // safetyPropertyString: "~(~(\"A_1\"))",
        //     initialisedLatches: new Dictionary<string, bool>
        //     {
                 // initialise latches to false
        //         { "vA_1", false },
        //         { "vB_1", false },
        //     }
        // );

        // RunMultipleTest();

        // TestExampleSafetyMic();
        //CheckConsistency();
        //TestSMTLibProduction();

        //new GraphPlotter().CanvaBarChartCSVGenerator();
        //new GraphPlotter().GenerateMostynProgressStatusTable();

        //VerifyTransformation();
        //new Z3ResultsComparison();

        // TestExample3PD1();
        // TestExample3PD2();
    }

    public static void RunMultipleTest()
    {
        // create new list

        //     string safetyPropertyString = "~(\"A_1\" & \"B_1\")";
        // create a data structure of this form
        // [
        // {
        //     string relativePath = Environment.CurrentDirectory.EndsWith("net6.0") ? "../../../" : "";
        //     string PDpath = $"{relativePath}simpleExample/PD-down-example.json";
        //     string TpTpFileName = $"{relativePath}simpleExample/ExampleParadise.tptp";
        //     string fileName = $"{relativePath}simpleExample/PDExample.aag";
        //     string ladderPath = $"{relativePath}simpleExample/program-liveness-example.tptp";
        //     // ~(A_1 & B_1 )
        // }, ....
        // ]
        string relativePath = Environment.CurrentDirectory.EndsWith("net6.0") ? "../../../" : "";

        // // Example 2 P
        // TestSimpleExampleMicTpTp(
        //     relativePath,
        //     "Examples/ex2-p.aag",
        //     "Examples/ex2.tptp",
        //     // ¬ x ∨ x
        //     "(~(\"X_0\") | (\"X_0\"))",
        //     new Dictionary<string, bool> { { "vX_1", false } }
        // );
        // // Example 2 Q,
        TestSimpleExampleMicTpTp(
            relativePath,
            "Examples/ex2.tptp",
            // x
            "(\"X_0\")",
            "Examples/ex2-q.aag",
            new Dictionary<string, bool> { { "vX_1", false } }
        );
        Console.WriteLine($"Relative Path: {Environment.CurrentDirectory} => {relativePath}");
        List<(
            string PDpath,
            string TpTpFileName,
            string fileName,
            string ladderPath,
            string safetyPropertyString,
            IDictionary<string, bool>? initialisedLatches
        )> testCases = new()
        {
            // (
            //     "simpleExample/PD-down-example.json",
            //     "simpleExample/ExampleParadise.tptp",
            //     "simpleExample/PDExample.aag",
            //     "simpleExample/program-liveness-example.tptp",
            //     "~(\"A_0\" & \"B_0\")",
            //     new Dictionary<string, bool>
            //     {
            //         // initialise latches to false
            //         { "vA_1", false },
            //         { "vB_1", false },
            //     }
            // ),
            // (
            //     "simpleExample/PD-down-example.json",
            //     "simpleExample/ExampleParadise.tptp",
            //     "simpleExample/PDExamplev2.aag",
            //     "simpleExample/program-liveness-examplev2.tptp",
            //     "~(\"A_0\" & \"B_0\")",
            //     new Dictionary<string, bool>
            //     {
            //         // initialise latches to false
            //         { "vA_1", false },
            //         { "vB_1", false },
            //     }
            // ),
            // //  x ∨ y
            // (
            //     "simpleExample/PD-down-example.json",
            //     "Examples/Example1/ExampleParadise.tptp",
            //     "Examples/Example1/ex1.aag",
            //     "Examples/Example1/ex1.tptp",
            //     "(\"X_0\" | \"Y_0\")",
            //     new Dictionary<string, bool>
            //     {
            //         // initialise latches to false
            //         { "vX_1", true },
            //         { "vY_1", true },
            //     }
            // ),
            // (
            //     "simpleExample/PD-down-example.json",
            //     "Examples/Example1/ExampleParadise.tptp",
            //     "Examples/Example1/ex1-q.aag",
            //     "Examples/Example1/ex1.tptp",
            //     // ¬x ∧ ¬y
            //     "(~(\"X_0\") & ~(\"Y_0\"))",
            //     new Dictionary<string, bool>
            //     {
            //         // initialise latches to false
            //         { "vX_1", true },
            //         { "vY_1", true },
            //     }
            // ),
            // // Example 3 - P
            // (
            //     "simpleExample/PD-down-example.json",
            //     "Examples/Example1/ExampleParadise.tptp",
            //     "Examples/ex3-p.aag",
            //     "Examples/ex3.tptp",
            //     //¬ b ∨ s,
            //     "(~(\"B_0\") | (\"S_0\"))",
            //     new Dictionary<string, bool> { { "vB_1", false }, { "vS_1", true } }
            // ),
            // // Example 3 - Q
            // (
            //     "simpleExample/PD-down-example.json",
            //     "Examples/Example1/ExampleParadise.tptp",
            //     "Examples/ex3-q.aag",
            //     "Examples/ex3.tptp",
            //     // ¬ b ∧ ¬ s
            //     "(~(\"B_0\") & ~(\"S_0\"))",
            //     new Dictionary<string, bool> { { "vB_1", false }, { "vS_1", true } }
            // ),
            // // Example 4 P,
            // (
            //     "simpleExample/PD-down-example.json",
            //     "Examples/Example1/ExampleParadise.tptp",
            //     "Examples/ex4-p.aag",
            //     "Examples/ex4.tptp",
            //     // x
            //     "((\"X_0\") | ~(\"X_0\"))",
            //     new Dictionary<string, bool> { { "vX_1", false } }
            // ),
            // (
            //     "simpleExample/PD-down-example.json",
            //     "Examples/Example1/ExampleParadise.tptp",
            //     "Examples/ex4-q.aag",
            //     "Examples/ex4.tptp",
            //     // x
            //     "~(\"X_0\")",
            //     new Dictionary<string, bool>
            //     {
            //         // { "vX_1" , false }
            //     }
            // ),
            // // Example Harry - Q
            // (
            //     "simpleExample/PD-down-example.json",
            //     "Examples/Example1/ExampleParadise.tptp",
            //     "Examples/exHarry-q.aag",
            //     "Examples/exHarry.tptp",
            //     //x ∨ y,
            //     "(~(\"X_0\") & ~(\"Y_0\"))",
            //     new Dictionary<string, bool> { { "vX_1", true }, { "vY_1", true } }
            // ),
            // Example Harry - P
            (
                "simpleExample/PD-down-example.json",
                "Examples/Example1/ExampleParadise.tptp",
                "Examples/exHarry-p0.aag",
                "Examples/exHarry.tptp",
                //x ∨ y,
                "((\"X_0\") | (\"Y_0\"))",
                new Dictionary<string, bool> { { "vX_1", true }, { "vY_1", true } }
            ),
            // Example Harry - P1
            (
                "simpleExample/PD-down-example.json",
                "Examples/Example1/ExampleParadise.tptp",
                "Examples/exHarry-p1.aag",
                "Examples/exHarry.tptp",
                //x ∨ y,
                "(\"X_0\")",
                new Dictionary<string, bool> { { "vX_1", true }, { "vY_1", true } }
            ),
            // Example Harry - P2
            (
                "simpleExample/PD-down-example.json",
                "Examples/Example1/ExampleParadise.tptp",
                "Examples/exHarry-p2.aag",
                "Examples/exHarry.tptp",
                //x ∨ y,
                "(~(\"Y_0\"))",
                new Dictionary<string, bool> { { "vX_1", true }, { "vY_1", true } }
            ),
            // add more test cases here
        };
    }

    public static void applyAigmoveAndAigResetThenRunIC3(string fileName)
    {
        string movedFileName = fileName.Replace(".aag", "_afteraigmove.aag");

        string argumentMove = $"aigmove {fileName} > {movedFileName}";
        ProcessStartInfo startInfoMove = new()
        {
            FileName = @"C:\Windows\system32\wsl.exe",
            Arguments =
                $"-d Ubuntu-24.04 -- {argumentMove.Replace('\\', '/').Replace("C:", "/mnt/c")}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        Process procMove = new() { StartInfo = startInfoMove };
        _ = procMove.Start();
        string outputMove = procMove.StandardOutput.ReadToEnd();
        string errorMove = procMove.StandardError.ReadToEnd();
        procMove.WaitForExit();
        if (procMove.ExitCode != 0)
        {
            Console.WriteLine("Error during aigmove:");
            Console.WriteLine(errorMove);
            return;
        }
        // console the output of aigmove
        // Console.WriteLine("Output of aigmove:");
        // Console.WriteLine(outputMove);
        Console.WriteLine("Applying aigreset on the moved AIG file:");
        string resetFileName = movedFileName.Replace(
            "_afteraigmove.aag",
            "_afteraigmove_afteraigreset.aag"
        );
        string argumentReset = $"aigreset {movedFileName} > {resetFileName}";
        ProcessStartInfo startInfoReset = new()
        {
            FileName = @"C:\Windows\system32\wsl.exe",
            Arguments =
                $"-d Ubuntu-24.04 -- {argumentReset.Replace('\\', '/').Replace("C:", "/mnt/c")}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true,
        };
        Process procReset = new() { StartInfo = startInfoReset };
        _ = procReset.Start();
        string outputReset = procReset.StandardOutput.ReadToEnd();
        string errorReset = procReset.StandardError.ReadToEnd();
        procReset.WaitForExit();
        if (procReset.ExitCode != 0)
        {
            Console.WriteLine("Error during aigreset:");
            Console.WriteLine(errorReset);
            return;
        }
        // Console.WriteLine("Output of aigreset:");
        // Console.WriteLine(outputReset);
        // Now run IC3 on the reset file
        Console.WriteLine("Running IC3 on the reset AIG file:");
        // remove results/ from resetFileName
        string resetFileNameForIC3 = resetFileName.Replace("results/", "");
        RunIC3(resetFileNameForIC3);
    }

    public static List<Model> TestSimpleExampleMicTpTp(
        string relativePath,
        string ladderPath,
        string safetyPropertyString,
        string fileName,
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

        // using (StreamWriter writer = new(relativePath + TpTpFileName))
        // {
        //     foreach (var state in TruthValuesPdStates)
        //     {
        //         string formattedstate = AigConstructor.FormatVarName(state);
        //         writer.WriteLine($"fof(ax,axiom, v{formattedstate}_0).");
        //     }
        //     foreach (var state in FalseValuesNonPdStates)
        //     {
        //         string formattedstate = AigConstructor.FormatVarName(state);
        //         writer.WriteLine($"fof(ax,axiom, ~v{formattedstate}_0).");
        //     }
        // }

        // AbstractFirstOrderFormula safetyCondition = MakeNegation(
        //      MakeAnd(
        //      TruthValuesPdStates.Select(ps => MakeVar(ps)).ToList()
        //      .Concat(
        //          FalseValuesNonPdStates.Select(nps => (AbstractFirstOrderFormula)MakeNegation(MakeVar(nps))).ToList()
        //      ).ToList()
        //      // .Concat(
        //      // // create a new list
        //      // new List<AbstractFirstOrderFormula>()
        //      // {
        //      //     MakeNegation(MakeVar("vTAC__OCCEXT_JR_0")) // just to avoid empty list issue
        //      // }
        //      // ).ToList()
        //      )
        //  );

        // AbstractFirstOrderFormula safetyCondition = MakeNegation(
        //    MakeAnd(
        //        MakeVar("A_1"),
        //        MakeVar("B_1")
        //    )
        // );
        AbstractFirstOrderFormula safetyCondition = SafetyPropertyParser.Parse(
            safetyPropertyString
        );
        Console.WriteLine("Safety Condition Parsed: " + PrettyPrinter.Prettify(safetyCondition));

        /// INTERNAL WORKINGS - DO NOT TOUCH
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safetyCondition);
        PrettyPrinter.PrettyPrintWithDelay(negatedSafety);
        AbstractFirstOrderFormula transformedNegSafety;

        Thread thread = new(
            () =>
            {
                transformedNegSafety = TransformToAig.Transform(negatedSafety);

                AigConstructor aigConstructor = new(
                    transformedLadder,
                    transformedNegSafety,
                    modelForTfLadder.LatchNamesAndValues
                );
                // Print Transformed Ladder and modelForTfLadder LatchNamesAndValues
                // Console.WriteLine("Transformed Ladder Rungs:");
                foreach (var rung in transformedLadder.Rungs)
                {
                    Console.WriteLine(
                        $"Output: {rung.output}, Formula: {PrettyPrinter.Prettify(rung.formula)}"
                    );
                }
                // Console.WriteLine("Latch Names and Values:");
                foreach (var kv in modelForTfLadder.LatchNamesAndValues)
                {
                    Console.WriteLine($"{kv.Key}: {kv.Value}");
                }
                aigConstructor.Decorate();
                aigConstructor.ConstructAigerFile(fileName);

                Program.RunIC3(fileName);
            },
            16 * 1024 * 1024
        ); // 16 MB stack size
        thread.Start();
        thread.Join(10000); // 10 seconds timeout

        return new List<Model>() { modelForOrgLadder, modelForTfLadder };
    }

    public static List<Model> TestExample3PD1()
    {
        string fileName = "TESTEXAMPLE3MICPD1.aag";

        AbstractFirstOrderFormula varI = MakeVar("i");
        AbstractFirstOrderFormula varB = MakeVar("b");
        AbstractFirstOrderFormula varS = MakeVar("s");

        Ladder ladder = new();

        AbstractFirstOrderFormula form1 = MakeOr(varI, MakeAnd(MakeNegation(varS), varB)); // i' ≡ i ∨ (¬ s ∧ b)

        Rung rung1 = new() { formula = form1, output = "b" };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula form2 = varS; // s' ≡ s
        Rung rung2 = new()
        {
            formula = form2,
            output = "s",
            Initialised = true,
        };
        ladder.AddRung(rung2);

        AbstractFirstOrderFormula safety = MakeNegation(MakeAnd(varB, varS));
        // AbstractFirstOrderFormula safety = MakeNegation(varB); // safety ≡ ¬ b ∨ s
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);
        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        // extra step? could have used modelForTfLadder.InputsAndVariablesValus in AigConstructor
        //Model model = new Model(ladder);
        //model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ladder);

        Model modelForTfLadder = new(transformedLadder);
        modelForTfLadder.InitialiseModel();

        AigConstructor aigConstructor = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        aigConstructor.Decorate();
        aigConstructor.ConstructAigerFile(fileName);

        RunIC3(fileName);

        return new List<Model>() { modelForTfLadder, modelForTfLadder };
    }

    public static List<Model> TestExample3PD2()
    {
        string fileName = "TESTEXAMPLE3MICPD2.aag";

        AbstractFirstOrderFormula varI = MakeVar("i");
        AbstractFirstOrderFormula varB = MakeVar("b");
        AbstractFirstOrderFormula varS = MakeVar("s");

        Ladder ladder = new();

        AbstractFirstOrderFormula form1 = MakeOr(varI, MakeAnd(MakeNegation(varS), varB)); // i' ≡ i ∨ (¬ s ∧ b)
        Rung rung1 = new() { formula = form1, output = "b" };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula form2 = varS; // s' ≡ s
        Rung rung2 = new()
        {
            formula = form2,
            output = "s",
            Initialised = true,
        };
        ladder.AddRung(rung2);

        AbstractFirstOrderFormula safety = MakeNegation(MakeAnd(varB, MakeNegation(varS)));
        // AbstractFirstOrderFormula safety = MakeNegation(varB); // safety ≡ ¬ b ∨ s
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);
        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        // extra step? could have used modelForTfLadder.InputsAndVariablesValus in AigConstructor
        //Model model = new Model(ladder);
        //model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ladder);

        Model modelForTfLadder = new(transformedLadder);
        modelForTfLadder.InitialiseModel();

        AigConstructor aigConstructor = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        aigConstructor.Decorate();
        aigConstructor.ConstructAigerFile(fileName);

        RunIC3(fileName);

        return new List<Model>() { modelForTfLadder, modelForTfLadder };
    }

    public static void VerifyTransformation()
    {
        XmlDocument doc = new();
        //doc.Load(Path.Combine("SiemensData", "Mostyn_946_Data", "xml_versions", "946_v12.xml"));
        doc.Load(Path.Combine("SiemensData", "LochNess-810", "810.xml"));
        Ladder ladder = LadderLogicXmlParser.ParseXML(doc);

        string safetyPropertyString =
            "((~(\"S6913(DC).U_0\")) & (\"S6913(DC).U_1\")) -> (((~(\"S6913.RD_1\")) | ((\"S6913.QRDZ_0\") | (\"S6913.QRDZ_1\"))) & ((~(\"SBS4.RD_1\")) | ((\"SBS4.QRDZ_0\") | (\"SBS4.QRDZ_1\"))))";

        AbstractFirstOrderFormula safetyCondition = SafetyPropertyParser.Parse(
            safetyPropertyString
        );
        AbstractFirstOrderFormula negatedOriginalSafety = MakeNegation(safetyCondition);
        //AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(safetyCondition);
        //AbstractFirstOrderFormula transformedNegatedSafety = MakeNegation(transformedSafety);

        SMTLibUtil.ToSMTLibInductive(ladder, negatedOriginalSafety, "originalSafety");
        //SMTLibUtil.ToSMTLibInductive(ladder, transformedSafety, "transformedSafety");

        //SMTLibUtil.ToSMTLibBoundedModelChecking(ladder, negatedOriginalSafety, 100, "originalSafetyBMCOriginal");

        //Console.WriteLine("Original Safety from COND file >>>>>");
        //Console.WriteLine(safetyPropertyString);
        //Console.WriteLine();
        //Console.WriteLine("Pretty Printed Parsed Safety >>>>>");
        //Console.WriteLine(PrettyPrinter.Prettify(safetyCondition));
        //Console.WriteLine();
        //Console.WriteLine("Pretty Printed Negated Parsed Safety >>>>>");
        //Console.WriteLine(PrettyPrinter.Prettify(negatedOriginalSafety));

        //AbstractFirstOrderFormula form1 = MakeOr(MakeVar("a"), MakeVar("b"));
        //AbstractFirstOrderFormula form2 = TransformToAig.TransformAllOrs(form1);
        //PrettyPrinter.PrettyPrint(form2);
    }

    public static void TestSMTLibProduction()
    {
        AbstractFirstOrderFormula varA = MakeVar("a");
        AbstractFirstOrderFormula varX = MakeVar("x");
        AbstractFirstOrderFormula varY = MakeVar("y");

        Ladder ladder = new();

        AbstractFirstOrderFormula form1 = MakeAnd(MakeNegation(varX), varY);
        Rung rung1 = new() { formula = form1, output = "x" };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula form2 = varY;
        Rung rung2 = new() { formula = form2, output = "y" };
        ladder.AddRung(rung2);

        AbstractFirstOrderFormula form3 = MakeAnd(
            MakeNegation(MakeAnd(varA, varY)),
            MakeNegation(MakeAnd(MakeNegation(varY), MakeNegation(varA)))
        );
        Rung rung3 = new() { formula = form3, output = "z" };
        ladder.AddRung(rung3);

        AbstractFirstOrderFormula varX0 = MakeVar("x_1");
        AbstractFirstOrderFormula varY0 = MakeVar("y_1");

        AbstractFirstOrderFormula safety = MakeAnd(MakeOr(varX0, varY0), varY0);
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);
        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        // Output an inductive or bounded model checking problem in the SMTLib format
        SMTLibUtil.ToSMTLibInductive(ladder, safety, "example1");

        //SMTLibUtil.ToSMTLibBoundedModelChecking(ladder, safety, 100, "example1");
    }

    public static void CheckConsistency()
    {
        string sourceRootPath = @"wetransfer_aigerfier-and-ladder-logic\SiemensData";
        string inputFilePath = Path.Combine(sourceRootPath, "ResultCSVs", "original_iv_bmc.csv");
        string outputFilePath = Path.Combine(
            sourceRootPath,
            "ResultCSVs",
            "consistency_original_iv_bmc.csv"
        );

        ConsistencyCsvFileGenerator ccfg = new(inputFilePath, outputFilePath);
        //ccfg.ParseInputAndGenerateOutputIvVsBmc();
        ccfg.ParseInputAndGenerateOutputIvAndBmcVsIc3();
    }

    public static void TestExample1_1()
    {
        AbstractFirstOrderFormula varA = MakeVar("a");
        AbstractFirstOrderFormula varX = MakeVar("x");
        AbstractFirstOrderFormula varY = MakeVar("y");

        Ladder ladder = new();

        AbstractFirstOrderFormula form1 = MakeOr(MakeAnd(varX, varY), varA);
        Rung rung1 = new() { formula = form1, output = "x" };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula form2 = MakeOr(varX, varA);
        Rung rung2 = new() { formula = form2, output = "y" };
        ladder.AddRung(rung2);

        AbstractFirstOrderFormula safety = MakeOr(MakeVar("x_1"), MakeNegation(MakeVar("y_1")));
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);

        // Output an inductive or bounded model checking problem in the SMTLib format
        SMTLibUtil.ToSMTLibInductive(ladder, negatedSafety, "example1");
        SMTLibUtil.ToSMTLibBoundedModelChecking(ladder, negatedSafety, 100, "example1");
    }

    public static void TestExample1_1A()
    {
        AbstractFirstOrderFormula varA = MakeVar("a");
        AbstractFirstOrderFormula varX = MakeVar("x");
        AbstractFirstOrderFormula varY = MakeVar("y");

        Ladder ladder = new();

        AbstractFirstOrderFormula form1 = MakeOr(MakeAnd(varX, varY), varA);
        Rung rung1 = new() { formula = form1, output = "x" };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula form2 = MakeOr(varX, varA);
        Rung rung2 = new() { formula = form2, output = "y" };
        ladder.AddRung(rung2);

        Ladder transLadder = TransformToAig.TransformLadder(ladder);

        AbstractFirstOrderFormula safetyVarX = MakeVar("x_1");
        AbstractFirstOrderFormula safetyVarY = MakeVar("y_1");

        AbstractFirstOrderFormula safety = MakeOr(safetyVarX, safetyVarY);
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);

        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(safety);
        AbstractFirstOrderFormula transformedNegSafety = MakeNegation(transformedSafety);

        // Output an inductive or bounded model checking problem in the SMTLib format
        SMTLibUtil.ToSMTLibInductive(ladder, negatedSafety, "example1a");
        SMTLibUtil.ToSMTLibBoundedModelChecking(ladder, negatedSafety, 100, "example1a");

        // do the above with a different set of transformed ladder and a transformed safety and run both on z3.
        SMTLibUtil.ToSMTLibInductive(transLadder, transformedNegSafety, "example1a_transformed");
        SMTLibUtil.ToSMTLibBoundedModelChecking(
            transLadder,
            transformedNegSafety,
            100,
            "example1a_transformed"
        );
    }

    public static void TestExample1_2()
    {
        AbstractFirstOrderFormula varA = MakeVar("a");
        AbstractFirstOrderFormula varX = MakeVar("x");
        AbstractFirstOrderFormula varY = MakeVar("y");

        Ladder ladder = new();

        AbstractFirstOrderFormula form1 = MakeOr(MakeAnd(varX, MakeNegation(varY)), varA);
        Rung rung1 = new() { formula = form1, output = "x" };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula form2 = varY;
        Rung rung2 = new() { formula = form2, output = "y" };
        ladder.AddRung(rung2);

        AbstractFirstOrderFormula safety = MakeOr(MakeVar("x_1"), MakeNegation(MakeVar("y_1")));
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);

        // Output an inductive or bounded model checking problem in the SMTLib format
        SMTLibUtil.ToSMTLibInductive(ladder, negatedSafety, "example2");
        SMTLibUtil.ToSMTLibBoundedModelChecking(ladder, negatedSafety, 100, "example2");
    }

    public static void TestExample1_3()
    {
        AbstractFirstOrderFormula varX = MakeVar("x");

        Ladder ladder = new();
        //¬ (¬ (x ∧ x) ∧ ¬ (¬ x ∧ ¬ x))
        AbstractFirstOrderFormula form1 = MakeNegation(
            MakeAnd(
                MakeNegation(MakeAnd(varX, varX)),
                MakeNegation(MakeAnd(MakeNegation(varX), MakeNegation(varX)))
            )
        );
        Rung rung1 = new() { formula = form1, output = "x" };
        ladder.AddRung(rung1);

        //AbstractFirstOrderFormula form2 = varY;
        //Rung rung2 = new Rung();
        //rung2.formula = form2;
        //rung2.output = "y";
        //ladder.AddRung(rung2);

        AbstractFirstOrderFormula varX_1 = MakeVar("x_1");
        AbstractFirstOrderFormula safety = MakeOr(varX_1, MakeNegation(varX_1));
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);

        // Output an inductive or bounded model checking problem in the SMTLib format
        SMTLibUtil.ToSMTLibInductive(ladder, negatedSafety, "example3");
        SMTLibUtil.ToSMTLibBoundedModelChecking(ladder, negatedSafety, 100, "example3");
    }

    public static void TestExample1_4()
    {
        AbstractFirstOrderFormula varX = MakeVar("x");

        Ladder ladder = new();
        AbstractFirstOrderFormula form1 = MakeOr(varX, MakeNegation(varX));
        Rung rung1 = new() { formula = form1, output = "x" };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula form2 = MakeAnd(varX, MakeNegation(varX));
        Rung rung2 = new() { formula = form2, output = "y" };
        ladder.AddRung(rung2);

        AbstractFirstOrderFormula varX_1 = MakeVar("x_1");
        AbstractFirstOrderFormula safety = varX_1;
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);

        // Output an inductive or bounded model checking problem in the SMTLib format
        SMTLibUtil.ToSMTLibInductive(ladder, negatedSafety, "example4");
        SMTLibUtil.ToSMTLibBoundedModelChecking(ladder, negatedSafety, 100, "example4");
    }

    public static List<Model> TestExample1Mic()
    {
        AbstractFirstOrderFormula varA = MakeVar("a");
        AbstractFirstOrderFormula varX = MakeVar("x");
        AbstractFirstOrderFormula varY = MakeVar("y");

        Ladder ladder = new();

        AbstractFirstOrderFormula form1 = MakeAnd(MakeNegation(varX), varY); // x' ≡ ¬ x ∧ y
        // form1 = MakeNegation(form1); // Negate the formula to follow Markus's method
        Rung rung1 = new()
        {
            formula = form1,
            output = "x",
            Initialised = true,
        };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula form2 = varY; // y' ≡ y
        // form2 = MakeNegation(form2); // Negate the formula to follow Markus's method
        Rung rung2 = new()
        {
            formula = form2,
            output = "y",
            Initialised = true,
        };
        ladder.AddRung(rung2);

        // AbstractFirstOrderFormula form3 = MakeAnd(MakeNegation(MakeAnd(varA, varY)), MakeNegation(MakeAnd(MakeNegation(varY), MakeNegation(varA)))); // z' ≡ ¬ (a ∧ y’) ∧ ¬ (¬ a ∧ ¬ y’)

        AbstractFirstOrderFormula form3 = MakeAnd( // z' ≡ a ⊕ y'         // z' ≡ (a V y ) ^ ¬(a^y)
            MakeOr(varA, varY),
            MakeNegation(MakeAnd(varA, varY))
        );
        Rung rung3 = new() { formula = form3, output = "z" };
        ladder.AddRung(rung3);

        AbstractFirstOrderFormula safety = MakeOr(varX, varY);
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);
        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety); // Converts the negated safety property to AIG format
        Console.WriteLine("Transformed Safety: " + PrettyPrinter.Prettify(transformedSafety));

        // TODO: remove this step of initialising the model with the original ladder and only use transformed ladder
        //Model model = new Model(ladder);
        //model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ladder); // It calls the same method used for transforming the safety property to AIG format
        // TransformedLadder is a new Ladder object with the rungs transformed to AIG format
        // Console.WriteLine("Transformed Ladder: " + transformedLadder.AllVariables());

        Model modelForTfLadder = new(transformedLadder); // Just set the variable `ladder` to transformed ladder passed as a parameter
        modelForTfLadder.InitialiseModel(); // gets and set the latch names and values to the variable `latchNamesAndValues`

        // AigConstructor aigConstructor = new(transformedLadder, transformedSafety, modelForTfLadder.LatchNamesAndValues);
        // aigConstructor.Decorate();
        // aigConstructor.ConstructAigerFile();

        AigConstructor aigConstructor = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        // DOES THE FOLLOWING:
        // ladder = ldr;   // Set the transformed ladder to the variable `ladder` in the AigConstructor
        // safety = sfty; // set the transformed safety property to the variable `safety` in the AigConstructor
        // allLatchVariables = ladder.rungs.Select(r => r.output); // get all the latch (output) variables from the transformed ladder
        // latchNamesAndValues = latchNameVal; // set the latch names and values to the variable `latchNamesAndValues` same as the `latchNamesAndValues` in the Model class
        aigConstructor.Decorate();
        string fileName = "TESTEXAMPLE1MIC.aag";
        aigConstructor.ConstructAigerFile(fileName);

        //    String argument = "~/swansea-uni/IC3ref/IC3 < ~/swansea-uni/SwanLLVerifier/SwanLLVerifier/bin/Debug/net6.0/test.aag";
        // get current directory program is running in programmatically

        RunIC3(fileName);

        return new List<Model>() { modelForTfLadder, modelForTfLadder };
    }

    public static List<Model> QTestExample1Mic()
    {
        AbstractFirstOrderFormula varA = MakeVar("a");
        AbstractFirstOrderFormula varX = MakeVar("x");
        AbstractFirstOrderFormula varY = MakeVar("y");

        Ladder ladder = new();

        AbstractFirstOrderFormula form1 = MakeAnd(MakeNegation(varX), varY); // x' ≡ ¬ x ∧ y
        // form1 = MakeNegation(form1); // Negate the formula to follow Markus's method
        Rung rung1 = new()
        {
            formula = form1,
            output = "x",
            Initialised = true,
        };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula form2 = varY; // y' ≡ y
        // form2 = MakeNegation(form2); // Negate the formula to follow Markus's method
        Rung rung2 = new()
        {
            formula = form2,
            output = "y",
            Initialised = true,
        };
        ladder.AddRung(rung2);

        // AbstractFirstOrderFormula form3 = MakeAnd(MakeNegation(MakeAnd(varA, varY)), MakeNegation(MakeAnd(MakeNegation(varY), MakeNegation(varA)))); // z' ≡ ¬ (a ∧ y’) ∧ ¬ (¬ a ∧ ¬ y’)

        AbstractFirstOrderFormula form3 = MakeAnd( // z' ≡ a ⊕ y'         // z' ≡ (a V y ) ^ ¬(a^y)
            MakeOr(varA, varY),
            MakeNegation(MakeAnd(varA, varY))
        );
        Rung rung3 = new() { formula = form3, output = "z" };
        ladder.AddRung(rung3);

        AbstractFirstOrderFormula safety = MakeAnd(MakeNegation(varX), MakeNegation(varY));
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);
        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety); // Converts the negated safety property to AIG format
        Console.WriteLine("Transformed Safety: " + PrettyPrinter.Prettify(transformedSafety));

        // TODO: remove this step of initialising the model with the original ladder and only use transformed ladder
        //Model model = new Model(ladder);
        //model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ladder); // It calls the same method used for transforming the safety property to AIG format
        // TransformedLadder is a new Ladder object with the rungs transformed to AIG format
        // Console.WriteLine("Transformed Ladder: " + transformedLadder.AllVariables());

        Model modelForTfLadder = new(transformedLadder); // Just set the variable `ladder` to transformed ladder passed as a parameter
        modelForTfLadder.InitialiseModel(); // gets and set the latch names and values to the variable `latchNamesAndValues`

        // AigConstructor aigConstructor = new(transformedLadder, transformedSafety, modelForTfLadder.LatchNamesAndValues);
        // aigConstructor.Decorate();
        // aigConstructor.ConstructAigerFile();

        AigConstructor aigConstructor = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        // DOES THE FOLLOWING:
        // ladder = ldr;   // Set the transformed ladder to the variable `ladder` in the AigConstructor
        // safety = sfty; // set the transformed safety property to the variable `safety` in the AigConstructor
        // allLatchVariables = ladder.rungs.Select(r => r.output); // get all the latch (output) variables from the transformed ladder
        // latchNamesAndValues = latchNameVal; // set the latch names and values to the variable `latchNamesAndValues` same as the `latchNamesAndValues` in the Model class
        aigConstructor.Decorate();
        string fileName = "QTESTEXAMPLE1MIC.aag";
        aigConstructor.ConstructAigerFile(fileName);

        //    String argument = "~/swansea-uni/IC3ref/IC3 < ~/swansea-uni/SwanLLVerifier/SwanLLVerifier/bin/Debug/net6.0/test.aag";
        // get current directory program is running in programmatically

        RunIC3(fileName);

        return new List<Model>() { modelForTfLadder, modelForTfLadder };
    }

    public static void HypoPExample1()
    {
        string fileName = "HypoPTESTEXAMPLE1MIC.aag";

        // Ladder ladder = new();

        string a = "a";
        string x = "x";
        string y = "y";
        string z = "z"; // output variable for the last rung
        string l_x = "l_x";
        string l_y = "l_y"; // latch variable for y

        AbstractFirstOrderFormula varA = MakeVar(a);
        AbstractFirstOrderFormula varX = MakeVar(x);
        AbstractFirstOrderFormula varY = MakeVar(y);
        AbstractFirstOrderFormula varLX = MakeVar(l_x); // l_x is the latch variable for x
        AbstractFirstOrderFormula varLY = MakeVar(l_y); // l_y is the latch variable for y

        Ladder ladder = new();

        Rung rung1 = new()
        {
            // x ’ ≡ ¬  (¬(( x ^ l-x) ^ ¬( x ^ l-x))) ∧ y
            formula = MakeAnd(
                MakeNegation(
                    MakeAnd(MakeNegation(MakeAnd(varX, varLX)), MakeNegation(MakeAnd(varX, varLX)))
                ),
                varY
            ), // x' ≡ ¬ (¬(x ∧ l_x) ∧ ¬(x ∧ l_x)) ∧ y
            output = x,
        };
        ladder.AddRung(rung1);

        // y ’ ≡ ¬(( y ^ l-y) ^ ¬( y ^ l-y))
        Rung rung2 = new()
        {
            formula = MakeNegation(
                MakeAnd(MakeAnd(varY, varLY), MakeNegation(MakeAnd(varY, varLY)))
            ), // y' ≡ ¬(y ∧ l_y) ∧ ¬(y ∧ l_y)
            output = y,
        };
        ladder.AddRung(rung2);

        // z = (a V  ¬(( y ^ l-y) ^ ¬( y ^ l-y)) ) ^ ¬(a^ ¬(( y ^ l-y) ^ ¬( y ^ l-y)))
        Rung rung3 = new()
        {
            formula = MakeAnd(
                MakeOr(
                    varA,
                    MakeNegation(MakeAnd(MakeAnd(varY, varLY), MakeNegation(MakeAnd(varY, varLY))))
                ),
                MakeNegation(
                    MakeAnd(
                        varA,
                        MakeNegation(
                            MakeAnd(MakeAnd(varY, varLY), MakeNegation(MakeAnd(varY, varLY)))
                        )
                    )
                )
            ),
            output = z,
        };
        ladder.AddRung(rung3);

        // L-X = X ^  L-X
        Rung rung4 = new()
        {
            formula = MakeAnd(varX, varLX), // l_x' ≡ x ∧ l_x
            output = l_x,
        };
        ladder.AddRung(rung4);

        // L-Y = Y ^ L-Y
        Rung rung5 = new()
        {
            formula = MakeAnd(varY, varLY), // l_y' ≡ y ∧ l_y
            output = l_y,
        };
        ladder.AddRung(rung5);

        // AbstractFirstOrderFormula safety = MakeAnd(
        //     MakeNegation(varX), // ¬ x
        //     MakeNegation(varY)  // ¬ y
        // ); // safety ≡ ¬ x ∧ ¬ y
        // fileName = "HypoQTESTEXAMPLE1MIC.aag";

        AbstractFirstOrderFormula safety = MakeOr(varX, varY);
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);
        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety); // Converts the negated safety property to AIG format
        Console.WriteLine("Transformed Safety: " + PrettyPrinter.Prettify(transformedSafety));

        // TODO: remove this step of initialising the model with the original ladder and only use transformed ladder
        //Model model = new Model(ladder);
        //model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ladder); // It calls the same method used for transforming the safety property to AIG format
        // TransformedLadder is a new Ladder object with the rungs transformed to AIG format
        // Console.WriteLine("Transformed Ladder: " + transformedLadder.AllVariables());

        Model modelForTfLadder = new(transformedLadder); // Just set the variable `ladder` to transformed ladder passed as a parameter
        modelForTfLadder.InitialiseModel(); // gets and set the latch names and values to the variable `latchNamesAndValues`

        // AigConstructor aigConstructor = new(transformedLadder, transformedSafety, modelForTfLadder.LatchNamesAndValues);
        // aigConstructor.Decorate();
        // aigConstructor.ConstructAigerFile();

        AigConstructor aigConstructor = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        // DOES THE FOLLOWING:
        // ladder = ldr;   // Set the transformed ladder to the variable `ladder` in the AigConstructor
        // safety = sfty; // set the transformed safety property to the variable `safety` in the AigConstructor
        // allLatchVariables = ladder.rungs.Select(r => r.output); // get all the latch (output) variables from the transformed ladder
        // latchNamesAndValues = latchNameVal; // set the latch names and values to the variable `latchNamesAndValues` same as the `latchNamesAndValues` in the Model class
        aigConstructor.Decorate();
        aigConstructor.ConstructAigerFile(fileName);

        //    String argument = "~/swansea-uni/IC3ref/IC3 < ~/swansea-uni/SwanLLVerifier/SwanLLVerifier/bin/Debug/net6.0/test.aag";
        // get current directory program is running in programmatically

        RunIC3(fileName);
    }

    public static List<Model> TestExample3Mic()
    {
        AbstractFirstOrderFormula varI = MakeVar("i");
        AbstractFirstOrderFormula varB = MakeVar("b");
        AbstractFirstOrderFormula varS = MakeVar("s");

        Ladder ladder = new();

        AbstractFirstOrderFormula form1 = MakeOr(varI, MakeAnd(MakeNegation(varS), varB)); // i' ≡ i ∨ (¬ s ∧ b)
        Rung rung1 = new() { formula = form1, output = "b" };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula form2 = varS; // s' ≡ s
        Rung rung2 = new()
        {
            formula = form2,
            output = "s",
            Initialised = true,
        };
        ladder.AddRung(rung2);

        AbstractFirstOrderFormula safety = MakeOr(MakeNegation(varB), varS); // safety ≡ ¬ b ∨ s
        // AbstractFirstOrderFormula safety = MakeNegation(varB); // safety ≡ ¬ b ∨ s
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);
        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        // extra step? could have used modelForTfLadder.InputsAndVariablesValus in AigConstructor
        //Model model = new Model(ladder);
        //model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ladder);

        Model modelForTfLadder = new(transformedLadder);
        modelForTfLadder.InitialiseModel();

        AigConstructor aigConstructor = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        aigConstructor.Decorate();
        string fileName = "TESTEXAMPLE3MIC.aag";
        aigConstructor.ConstructAigerFile(fileName);

        RunIC3(fileName);

        return new List<Model>() { modelForTfLadder, modelForTfLadder };
    }

    public static List<Model> QTestExample3Mic()
    {
        string fileName = "QTESTEXAMPLE3MIC.aag";

        AbstractFirstOrderFormula varI = MakeVar("i");
        AbstractFirstOrderFormula varB = MakeVar("b");
        AbstractFirstOrderFormula varS = MakeVar("s");

        Ladder ladder = new();

        AbstractFirstOrderFormula form1 = MakeOr(varI, MakeAnd(MakeNegation(varS), varB)); // i' ≡ i ∨ (¬ s ∧ b)
        Rung rung1 = new() { formula = form1, output = "b" };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula form2 = varS; // s' ≡ s
        Rung rung2 = new()
        {
            formula = form2,
            output = "s",
            Initialised = true,
        };
        ladder.AddRung(rung2);

        // AbstractFirstOrderFormula safety = MakeOr(MakeNegation(varB), varS); // safety ≡ ¬ b ∨ s
        AbstractFirstOrderFormula safety = MakeAnd(MakeNegation(varB), MakeNegation(varS)); // safety ≡ ¬ b ∨ s
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);
        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        // extra step? could have used modelForTfLadder.InputsAndVariablesValus in AigConstructor
        //Model model = new Model(ladder);
        //model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ladder);

        Model modelForTfLadder = new(transformedLadder);
        modelForTfLadder.InitialiseModel();

        AigConstructor aigConstructor = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        aigConstructor.Decorate();
        aigConstructor.ConstructAigerFile(fileName);

        RunIC3(fileName);

        return new List<Model>() { modelForTfLadder, modelForTfLadder };
    }

    public static void HypoTestExample3Mic()
    {
        string fileName = "HypoTESTEXAMPLE3MIC.aag";

        AbstractFirstOrderFormula varI = MakeVar("i");
        AbstractFirstOrderFormula varB = MakeVar("b");
        AbstractFirstOrderFormula varS = MakeVar("s");
        AbstractFirstOrderFormula varX = MakeVar("x");

        Ladder ladder = new();

        // x ’ ≡ ¬ (¬ x ∧ x) ∧ i
        AbstractFirstOrderFormula form3 = MakeAnd(
            MakeNegation(MakeAnd(MakeNegation(varX), varX)),
            varI
        ); // x' ≡ ¬ (¬ x ∧ x) ∧ i
        Rung rung3 = new() { formula = form3, output = "x" };
        ladder.AddRung(rung3);

        AbstractFirstOrderFormula form1 = MakeOr(varX, MakeAnd(MakeNegation(varS), varB)); // b' ≡ x ∨ (¬ s ∧ b)
        Rung rung1 = new() { formula = form1, output = "b" };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula form2 = varS; // s' ≡ s
        Rung rung2 = new()
        {
            formula = form2,
            output = "s",
            Initialised = true,
        };
        ladder.AddRung(rung2);

        AbstractFirstOrderFormula safety = MakeOr(MakeNegation(varB), varS); // safety ≡ ¬ b ∨ s
        // AbstractFirstOrderFormula safety = MakeNegation(varB); // safety ≡ ¬ b ∨ s
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);
        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        // extra step? could have used modelForTfLadder.InputsAndVariablesValus in AigConstructor
        //Model model = new Model(ladder);
        //model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ladder);

        Model modelForTfLadder = new(transformedLadder);
        modelForTfLadder.InitialiseModel();

        AigConstructor aigConstructor = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        aigConstructor.Decorate();
        aigConstructor.ConstructAigerFile(fileName);

        RunIC3(fileName);
    }

    public static void HyPoQTestExample3Mic()
    {
        string fileName = "HyPoQTESTEXAMPLE3MIC.aag";

        AbstractFirstOrderFormula varI = MakeVar("i");
        AbstractFirstOrderFormula varB = MakeVar("b");
        AbstractFirstOrderFormula varS = MakeVar("s");
        AbstractFirstOrderFormula varX = MakeVar("x");

        Ladder ladder = new();

        // x ’ ≡ ¬ (¬ x ∧ x) ∧ i
        AbstractFirstOrderFormula form3 = MakeAnd(
            MakeNegation(MakeAnd(MakeNegation(varX), varX)),
            varI
        ); // x' ≡ ¬ (¬ x ∧ x) ∧ i
        Rung rung3 = new() { formula = form3, output = "x" };
        ladder.AddRung(rung3);

        AbstractFirstOrderFormula form1 = MakeOr(varX, MakeAnd(MakeNegation(varS), varB)); // b' ≡ x ∨ (¬ s ∧ b)
        Rung rung1 = new() { formula = form1, output = "b" };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula form2 = varS; // s' ≡ s
        Rung rung2 = new()
        {
            formula = form2,
            output = "s",
            Initialised = true,
        };
        ladder.AddRung(rung2);

        // AbstractFirstOrderFormula safety = MakeOr(MakeNegation(varB), varS); // safety ≡ ¬ b ∨ s
        AbstractFirstOrderFormula safety = MakeAnd(MakeNegation(varB), MakeNegation(varS)); // safety ≡ ¬ b ∨ s
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);
        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        // extra step? could have used modelForTfLadder.InputsAndVariablesValus in AigConstructor
        //Model model = new Model(ladder);
        //model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ladder);

        Model modelForTfLadder = new(transformedLadder);
        modelForTfLadder.InitialiseModel();

        AigConstructor aigConstructor = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        aigConstructor.Decorate();
        aigConstructor.ConstructAigerFile(fileName);

        RunIC3(fileName);
    }

    public static List<Model> TestExample4Mic()
    {
        AbstractFirstOrderFormula varX = MakeVar("x");

        Ladder ladder = new();

        AbstractFirstOrderFormula form1 = // (x V ¬x) ^ ¬(x ^ ¬x)
        MakeAnd(MakeOr(varX, MakeNegation(varX)), MakeNegation(MakeAnd(varX, MakeNegation(varX))));
        Rung rung1 = new() { formula = form1, output = "x" };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula safety = MakeOr(varX, MakeNegation(varX)); // safety ≡ x ∨ ¬ x
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);

        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        Model model = new(ladder);
        model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ladder);

        Model modelForTfLadder = new(transformedLadder);
        modelForTfLadder.InitialiseModel();

        AigConstructor aigConstructor = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        aigConstructor.Decorate();
        string fileName = "TESTEXAMPLE4MIC.aag";
        aigConstructor.ConstructAigerFile(fileName);

        RunIC3(fileName);

        return new List<Model>() { model, modelForTfLadder };
    }

    public static List<Model> TestExampleSafetyMic()
    {
        string reqA_0 = "reqA_0";
        string reqB_0 = "reqB_0";
        string A_1 = "A";
        string B_1 = "B";
        string B_0 = "B_0";

        string fileName = "TestExampleSafetyMic.aag";

        Ladder ladder = new();

        AbstractFirstOrderFormula form1 = // reqA_0 ^ ¬varB_0
        MakeAnd(MakeVar(reqA_0), MakeNegation(MakeVar(B_0)));

        AbstractFirstOrderFormula form2 = // reqB_0 ^ ¬varA_1
        MakeAnd(MakeVar(reqB_0), MakeNegation(MakeVar(A_1)));
        Rung rung1 = new() { formula = form1, output = AigConstructor.FormatVarName(A_1) };
        ladder.AddRung(rung1);

        Rung rung2 = new() { formula = form2, output = AigConstructor.FormatVarName(B_1) };
        ladder.AddRung(rung2);

        // ¬(A /\ B)
        AbstractFirstOrderFormula safety = MakeNegation(MakeAnd(MakeVar(A_1), MakeVar(B_1))); // safety ≡ ¬(A /\ B)

        // Internal workings
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);

        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        Model model = new(ladder);
        model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ladder);

        Model modelForTfLadder = new(transformedLadder);
        modelForTfLadder.InitialiseModel();

        AigConstructor aigConstructor = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        aigConstructor.Decorate();
        aigConstructor.ConstructAigerFile(fileName);

        RunIC3(fileName);

        return new List<Model>() { model, modelForTfLadder };
    }

    public static List<Model> QTestExample4Mic()
    {
        AbstractFirstOrderFormula varX = MakeVar("x");

        Ladder ladder = new();

        AbstractFirstOrderFormula form1 = // (x V ¬x) ^ ¬(x ^ ¬x)
        MakeAnd(MakeOr(varX, MakeNegation(varX)), MakeNegation(MakeAnd(varX, MakeNegation(varX))));
        Rung rung1 = new() { formula = form1, output = "x" };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula safety = MakeNegation(varX); // safety ≡ ¬ x
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);

        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        Model model = new(ladder);
        model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ladder);

        Model modelForTfLadder = new(transformedLadder);
        modelForTfLadder.InitialiseModel();

        AigConstructor aigConstructor = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        aigConstructor.Decorate();
        string fileName = "QTESTEXAMPLE4MIC.aag";
        aigConstructor.ConstructAigerFile(fileName);

        RunIC3(fileName);

        return new List<Model>() { model, modelForTfLadder };
    }

    public static List<Model> TestExample2Mic()
    {
        AbstractFirstOrderFormula varX = MakeVar("x");

        Ladder ladder = new();

        AbstractFirstOrderFormula form1 = MakeNegation(MakeOr(MakeNegation(varX), varX)); // x' ≡ ¬ x ∨ x
        // USED Markus METHOD -> Negate the only transition formula in the rung
        Rung rung1 = new() { formula = form1, output = "x_1" };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula safety = varX; // safety ≡ x
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);

        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        Model model = new(ladder);
        model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ladder);

        Model modelForTfLadder = new(transformedLadder);
        modelForTfLadder.InitialiseModel();

        AigConstructor aigConstructor = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        aigConstructor.Decorate();
        string fileName = "TESTEXAMPLE2MIC.aag";
        aigConstructor.ConstructAigerFile(fileName);

        RunIC3(fileName);

        return new List<Model>() { model, modelForTfLadder };
    }

    public static List<Model> TestExample2MofifiedMic()
    {
        AbstractFirstOrderFormula varX = MakeVar("x");

        Ladder ladder = new();

        AbstractFirstOrderFormula form1 = MakeNegation(MakeOr(MakeNegation(varX), varX)); // x' ≡ ¬ x ∨ x
        // USED Markus METHOD -> Negate the only transition formula in the rung
        Rung rung1 = new() { formula = form1, output = "x" };
        ladder.AddRung(rung1);

        AbstractFirstOrderFormula safety = MakeOr(MakeNegation(varX), varX);
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);

        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        Model model = new(ladder);
        model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ladder);

        Model modelForTfLadder = new(transformedLadder);
        modelForTfLadder.InitialiseModel();

        AigConstructor aigConstructor = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        aigConstructor.Decorate();
        string fileName = "TESTEXAMPLE2MODIFIEDMIC.aag";
        aigConstructor.ConstructAigerFile(fileName);

        RunIC3(fileName);

        return new List<Model>() { model, modelForTfLadder };
    }

    public static void TestExample5Mic()
    {
        AbstractFirstOrderFormula varS = MakeVar("s");
        AbstractFirstOrderFormula varB = MakeVar("b");
        AbstractFirstOrderFormula varI = MakeVar("i");

        Ladder ldr = new();

        AbstractFirstOrderFormula form1 = MakeOr(varI, MakeAnd(MakeNegation(varS), varB)); // i' ≡ i ∨ (¬ s ∧ b)
        // form1 = MakeNegation(form1); // Negate the formula to follow Markus's method
        Rung bRung = new() { formula = form1, output = "b" };

        ldr.AddRung(bRung);

        AbstractFirstOrderFormula form2 = varS; // s' ≡ s
        // form2 = MakeNegation(form2); // Negate the formula to follow Markus's method
        // Markus approach doesn't work here
        Rung sRung = new()
        {
            formula = form2,
            output = "s",
            Initialised = true,
        };

        ldr.AddRung(sRung);

        AbstractFirstOrderFormula safety = MakeOr(MakeNegation(varB), varS);
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);
        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        Model model = new(ldr);
        model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ldr);

        Model modelForTfLadder = new(transformedLadder);
        modelForTfLadder.InitialiseModel();

        AigConstructor aig = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        aig.Decorate();
        string fileName = "TESTEXAMPLE5MIC.aag";
        aig.ConstructAigerFile(fileName);

        RunIC3(fileName);
    }

    public static void QTestExample5Mic()
    {
        AbstractFirstOrderFormula varS = MakeVar("s");
        AbstractFirstOrderFormula varB = MakeVar("b");
        AbstractFirstOrderFormula varI = MakeVar("i");

        Ladder ldr = new();

        Rung bRung = new()
        {
            formula = MakeOr(varI, MakeAnd(MakeNegation(varS), varB)),
            output = "b",
        };

        ldr.AddRung(bRung);

        // Markus approach doesn't work here
        Rung sRung = new()
        {
            formula = varS,
            output = "s",
            Initialised = true,
        };

        ldr.AddRung(sRung);

        AbstractFirstOrderFormula safety = MakeAnd(MakeNegation(varB), MakeNegation(varS)); // safety ≡ ¬ b ∧ ¬ s
        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);
        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        Model model = new(ldr);
        model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ldr);

        Model modelForTfLadder = new(transformedLadder);
        modelForTfLadder.InitialiseModel();

        AigConstructor aig = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        aig.Decorate();
        string fileName = "QTESTEXAMPLE5MIC.aag";
        aig.ConstructAigerFile(fileName);

        RunIC3(fileName);
    }

    public static void TestExample5Pelican()
    {
        // AbstractFirstOrderFormula varS = MakeVar("s");
        // AbstractFirstOrderFormula varB = MakeVar("b");
        // AbstractFirstOrderFormula varI = MakeVar("i");

        // button, request, old sh, old sl, sh, sl, pg, pgf, pr, tg, ta, tr, taf

        const string button = "button";
        const string request = "request";
        const string old_Sh = "old_sh";
        const string old_Sl = "old_sl";
        const string sh = "sh";
        const string sl = "sl";
        const string pg = "pg";
        const string pgf = "pgf";
        const string pr = "pr";
        const string tg = "tg";
        const string ta = "ta";
        const string tr = "tr";
        const string taf = "taf";

        AbstractFirstOrderFormula varButton = MakeVar(button);
        AbstractFirstOrderFormula varRequest = MakeVar(request);
        AbstractFirstOrderFormula varOld_sh = MakeVar(old_Sh);
        AbstractFirstOrderFormula varOld_sl = MakeVar(old_Sl);
        AbstractFirstOrderFormula varSH = MakeVar(sh);
        AbstractFirstOrderFormula varSL = MakeVar(sl);
        AbstractFirstOrderFormula varPG = MakeVar(pg);
        AbstractFirstOrderFormula varPGF = MakeVar(pgf);
        AbstractFirstOrderFormula varPR = MakeVar(pr);
        AbstractFirstOrderFormula varTG = MakeVar(tg);
        AbstractFirstOrderFormula varTA = MakeVar(ta);
        AbstractFirstOrderFormula varTR = MakeVar(tr);
        AbstractFirstOrderFormula varTAF = MakeVar(taf);

        Ladder ldr = new();

        Rung oshRung = new()
        {
            formula = varSH, // old_sh' ≡ sh
            output = old_Sh,
        };
        ldr.AddRung(oshRung);

        Rung oslRung = new()
        {
            formula = varSL, // old_sl' ≡ sl
            output = old_Sl,
        };
        ldr.AddRung(oslRung);

        Rung shRung = new()
        {
            formula = MakeOr(
                MakeAnd(varOld_sh, MakeNegation(varOld_sl)),
                MakeAnd(MakeNegation(varOld_sh), varOld_sl)
            ), // sh' ≡ (old_sh ∧ ¬ old_sl) ∨ (¬ old_sh ∧ old_sl)
            output = sh,
            Initialised = false, // sh isfalse
        };
        ldr.AddRung(shRung);

        Rung slRung = new()
        {
            formula = // sl' ≡ (old_sh ∧ ¬ old_sl) ∨ ((¬ request ∧ button) ^ ¬ old_sl))
            MakeOr(
                MakeAnd(varOld_sh, MakeNegation(varOld_sl)),
                MakeAnd(MakeAnd(MakeNegation(varRequest), varButton), MakeNegation(varOld_sl))
            ),
            output = sl,
            Initialised = false, // sl is false
        };
        ldr.AddRung(slRung);

        AbstractFirstOrderFormula requestRungFormula = // request' ≡ (button ^ ¬old_sh’) V (button ^ ¬old_sl’)
                                                       //    V (request ^  ¬button ^  ¬old_sh’)
                                                       //    V (request ^ ¬button ^ ¬old_sl’)
                                                       // is equivalent to:
                                                       // (button ^ ¬old_sh’) V (button ^ ¬old_sl’) V ((request ^  ¬button) ^  ¬old_sh’) V ((request ^ ¬button) ^ ¬old_sl’)

            MakeOr(
                MakeOr(
                    MakeOr(
                        MakeAnd(varButton, MakeNegation(varOld_sh)), // (button ^ ¬old_sh’)
                        MakeAnd(varButton, MakeNegation(varOld_sl)) // (button ^ ¬old_sl’)
                    ) // (button ^ ¬old_sh’) V (button ^ ¬old_sl’)
                    ,
                    MakeAnd(MakeAnd(varRequest, MakeNegation(varButton)), MakeNegation(varOld_sh)) // ((request ^  ¬button) ^  ¬old_sh’)
                ), // (button ^ ¬old_sh’) V (button ^ ¬old_sl’) V ((request ^  ¬button) ^  ¬old_sh’)
                MakeAnd(MakeAnd(varRequest, MakeNegation(varButton)), MakeNegation(varOld_sl)) // ((request ^ ¬button) ^ ¬old_sl’)
            ); // (button ^ ¬old_sh’) V (button ^ ¬old_sl’) V ((request ^  ¬button) ^  ¬old_sh’) V ((request ^ ¬button) ^ ¬old_sl’)

        _ = PrettyPrinter.Prettify(requestRungFormula);

        Rung requestRung = new()
        {
            formula = requestRungFormula,

            output = request,
            Initialised = false, // request is false
        };
        ldr.AddRung(requestRung);

        Rung pgRung = new()
        {
            formula = MakeAnd(varOld_sh, MakeNegation(varOld_sl)), // pg' ≡ old_sh ∧ ¬ old_sl
            output = pg,
        };
        ldr.AddRung(pgRung);

        Rung pgfRung = new()
        {
            formula = MakeAnd(varOld_sh, varOld_sl), // pgf' ≡ old_sh ∧ old_sl
            output = pgf,
        };
        ldr.AddRung(pgfRung);

        Rung prRung = new()
        {
            formula = MakeNegation(varOld_sh), // pr' ≡ ¬ old_sh
            output = pr,
            Initialised = true, // pr is initialised to true
        };
        ldr.AddRung(prRung);

        Rung tgRung = new()
        {
            formula = MakeOr(
                MakeAnd(MakeNegation(varOld_sh), MakeNegation(varOld_sl)), // tg' ≡ ¬ old_sh ∧ ¬ old_sl
                MakeAnd(MakeNegation(varButton), MakeNegation(varRequest)) // V (¬ button ∧ ¬ request)
            ), // tg' ≡ ¬ old_sh ∧ ¬ old_sl V (¬ button ∧ ¬ request)
            output = tg,
            Initialised = true, // tg is initialised to true
        };
        ldr.AddRung(tgRung);

        Rung taRung = new()
        {
            formula = MakeAnd(MakeNegation(varOld_sh), varOld_sl), // ta' ≡ ¬ old_sh ∧ old_sl
            output = ta,
        };
        ldr.AddRung(taRung);

        Rung trRung = new()
        {
            formula = MakeAnd(varOld_sh, MakeNegation(varOld_sl)), // tr' ≡ old_sh ∧ ¬ old_sl
            output = tr,
        };
        ldr.AddRung(trRung);

        Rung tafRung = new()
        {
            formula = MakeAnd(varOld_sh, varOld_sl), // taf' ≡ old_sh ∧ old_sl
            output = taf,
        };
        ldr.AddRung(tafRung);

        // SAFETY PROPERTY
        // // For tra c lights, exclusively one out of tg tatr taf shall be true:
        // AbstractFirstOrderFormula safety = MakeAnd(
        //     MakeNegation(varTG),
        //     MakeNegation(varTA),
        //     MakeNegation(varTAF)
        // ); // safety ≡ ¬ tg ∧ ¬ ta ∧ ¬ taf

        // for pedestrian lights, exclusively one out of pgpgfpr shall be true:
        // safety ≡ (pg ^ ¬ pgf ^ ¬ pr) V (¬ pg ^ pgf ^ ¬ pr) V (¬ pg ^ ¬ pgf ^ pr)
        // is equivalent to:
        // (((pg ^ ¬ pgf) ^ ¬ pr) V ((¬ pg ^ pgf) ^ ¬ pr)) V ((¬ pg ^ ¬ pgf )^ pr)
        AbstractFirstOrderFormula safety = MakeOr(
            MakeOr(
                MakeAnd(MakeAnd(varPG, MakeNegation(varPGF)), MakeNegation(varPR)), // (pg ^ ¬ pgf) ^ ¬ pr
                MakeAnd(MakeAnd(MakeNegation(varPG), varPGF), MakeNegation(varPR)) // (¬ pg ^ pgf) ^ ¬ pr
            ), // ((pg ^ ¬ pgf) ^ ¬ pr) V ((¬ pg ^ pgf) ^ ¬ pr)
            MakeAnd(MakeAnd(MakeNegation(varPG), MakeNegation(varPGF)), varPR) // ((¬ pg ^ ¬ pgf )^ pr)
        ); // (((pg ^ ¬ pgf) ^ ¬ pr) V ((¬ pg ^ pgf) ^ ¬ pr)) V ((¬ pg ^ ¬ pgf )^ pr)

        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);
        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        Model model = new(ldr);
        model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ldr);

        Model modelForTfLadder = new(transformedLadder);
        modelForTfLadder.InitialiseModel();

        AigConstructor aig = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        aig.Decorate();
        string fileName = "TESTEXAMPLE5PELICANMIC.aag";
        aig.ConstructAigerFile(fileName);

        RunIC3(fileName);
    }

    public static void QTestExample5Pelican()
    {
        // AbstractFirstOrderFormula varS = MakeVar("s");
        // AbstractFirstOrderFormula varB = MakeVar("b");
        // AbstractFirstOrderFormula varI = MakeVar("i");

        // button, request, old sh, old sl, sh, sl, pg, pgf, pr, tg, ta, tr, taf

        const string button = "button";
        const string request = "request";
        const string old_Sh = "old_sh";
        const string old_Sl = "old_sl";
        const string sh = "sh";
        const string sl = "sl";
        const string pg = "pg";
        const string pgf = "pgf";
        const string pr = "pr";
        const string tg = "tg";
        const string ta = "ta";
        const string tr = "tr";
        const string taf = "taf";

        AbstractFirstOrderFormula varButton = MakeVar(button);
        AbstractFirstOrderFormula varRequest = MakeVar(request);
        AbstractFirstOrderFormula varOld_sh = MakeVar(old_Sh);
        AbstractFirstOrderFormula varOld_sl = MakeVar(old_Sl);
        AbstractFirstOrderFormula varSH = MakeVar(sh);
        AbstractFirstOrderFormula varSL = MakeVar(sl);
        AbstractFirstOrderFormula varPG = MakeVar(pg);
        AbstractFirstOrderFormula varPGF = MakeVar(pgf);
        AbstractFirstOrderFormula varPR = MakeVar(pr);
        AbstractFirstOrderFormula varTG = MakeVar(tg);
        AbstractFirstOrderFormula varTA = MakeVar(ta);
        AbstractFirstOrderFormula varTR = MakeVar(tr);
        AbstractFirstOrderFormula varTAF = MakeVar(taf);

        Ladder ldr = new();

        Rung oshRung = new()
        {
            formula = varSH, // old_sh' ≡ sh
            output = old_Sh,
        };
        ldr.AddRung(oshRung);

        Rung oslRung = new()
        {
            formula = varSL, // old_sl' ≡ sl
            output = old_Sl,
        };
        ldr.AddRung(oslRung);

        Rung shRung = new()
        {
            formula = MakeOr(
                MakeAnd(varOld_sh, MakeNegation(varOld_sl)),
                MakeAnd(MakeNegation(varOld_sh), varOld_sl)
            ), // sh' ≡ (old_sh ∧ ¬ old_sl) ∨ (¬ old_sh ∧ old_sl)
            output = sh,
            Initialised = false, // sh isfalse
        };
        ldr.AddRung(shRung);

        Rung slRung = new()
        {
            formula = // sl' ≡ (old_sh ∧ ¬ old_sl) ∨ ((¬ request ∧ button) ^ ¬ old_sl))
            MakeOr(
                MakeAnd(varOld_sh, MakeNegation(varOld_sl)),
                MakeAnd(MakeAnd(MakeNegation(varRequest), varButton), MakeNegation(varOld_sl))
            ),
            output = sl,
            Initialised = false, // sl is false
        };
        ldr.AddRung(slRung);

        AbstractFirstOrderFormula requestRungFormula = // request' ≡ (button ^ ¬old_sh’) V (button ^ ¬old_sl’)
                                                       //    V (request ^  ¬button ^  ¬old_sh’)
                                                       //    V (request ^ ¬button ^ ¬old_sl’)
                                                       // is equivalent to:
                                                       // (button ^ ¬old_sh’) V (button ^ ¬old_sl’) V ((request ^  ¬button) ^  ¬old_sh’) V ((request ^ ¬button) ^ ¬old_sl’)

            MakeOr(
                MakeOr(
                    MakeOr(
                        MakeAnd(varButton, MakeNegation(varOld_sh)), // (button ^ ¬old_sh’)
                        MakeAnd(varButton, MakeNegation(varOld_sl)) // (button ^ ¬old_sl’)
                    ) // (button ^ ¬old_sh’) V (button ^ ¬old_sl’)
                    ,
                    MakeAnd(MakeAnd(varRequest, MakeNegation(varButton)), MakeNegation(varOld_sh)) // ((request ^  ¬button) ^  ¬old_sh’)
                ), // (button ^ ¬old_sh’) V (button ^ ¬old_sl’) V ((request ^  ¬button) ^  ¬old_sh’)
                MakeAnd(MakeAnd(varRequest, MakeNegation(varButton)), MakeNegation(varOld_sl)) // ((request ^ ¬button) ^ ¬old_sl’)
            ); // (button ^ ¬old_sh’) V (button ^ ¬old_sl’) V ((request ^  ¬button) ^  ¬old_sh’) V ((request ^ ¬button) ^ ¬old_sl’)

        _ = PrettyPrinter.Prettify(requestRungFormula);

        Rung requestRung = new()
        {
            formula = requestRungFormula,

            output = request,
            Initialised = false, // request is false
        };
        ldr.AddRung(requestRung);

        Rung pgRung = new()
        {
            formula = MakeAnd(varOld_sh, MakeNegation(varOld_sl)), // pg' ≡ old_sh ∧ ¬ old_sl
            output = pg,
        };
        ldr.AddRung(pgRung);

        Rung pgfRung = new()
        {
            formula = MakeAnd(varOld_sh, varOld_sl), // pgf' ≡ old_sh ∧ old_sl
            output = pgf,
        };
        ldr.AddRung(pgfRung);

        Rung prRung = new()
        {
            formula = MakeNegation(varOld_sh), // pr' ≡ ¬ old_sh
            output = pr,
            Initialised = true, // pr is initialised to true
        };
        ldr.AddRung(prRung);

        Rung tgRung = new()
        {
            formula = MakeOr(
                MakeAnd(MakeNegation(varOld_sh), MakeNegation(varOld_sl)), // tg' ≡ ¬ old_sh ∧ ¬ old_sl
                MakeAnd(MakeNegation(varButton), MakeNegation(varRequest)) // V (¬ button ∧ ¬ request)
            ), // tg' ≡ ¬ old_sh ∧ ¬ old_sl V (¬ button ∧ ¬ request)
            output = tg,
            Initialised = true, // tg is initialised to true
        };
        ldr.AddRung(tgRung);

        Rung taRung = new()
        {
            formula = MakeAnd(MakeNegation(varOld_sh), varOld_sl), // ta' ≡ ¬ old_sh ∧ old_sl
            output = ta,
        };
        ldr.AddRung(taRung);

        Rung trRung = new()
        {
            formula = MakeAnd(varOld_sh, MakeNegation(varOld_sl)), // tr' ≡ old_sh ∧ ¬ old_sl
            output = tr,
        };
        ldr.AddRung(trRung);

        Rung tafRung = new()
        {
            formula = MakeAnd(varOld_sh, varOld_sl), // taf' ≡ old_sh ∧ old_sl
            output = taf,
        };
        ldr.AddRung(tafRung);

        // SAFETY PROPERTY
        // // For tra c lights, exclusively one out of tg tatr taf shall be true:
        // AbstractFirstOrderFormula safety = MakeAnd(
        //     MakeNegation(varTG),
        //     MakeNegation(varTA),
        //     MakeNegation(varTAF)
        // ); // safety ≡ ¬ tg ∧ ¬ ta ∧ ¬ taf

        // for pedestrian lights, exclusively one out of pgpgfpr shall be true:
        // safety ≡ (pg ^ ¬ pgf ^ ¬ pr) V (¬ pg ^ pgf ^ ¬ pr) V (¬ pg ^ ¬ pgf ^ pr)
        // is equivalent to:
        // (((pg ^ ¬ pgf) ^ ¬ pr) V ((¬ pg ^ pgf) ^ ¬ pr)) V ((¬ pg ^ ¬ pgf )^ pr)
        AbstractFirstOrderFormula safety = MakeAnd(varPG, varTG);

        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);
        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        Model model = new(ldr);
        model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ldr);

        Model modelForTfLadder = new(transformedLadder);
        modelForTfLadder.InitialiseModel();

        AigConstructor aig = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        aig.Decorate();
        string fileName = "QTESTEXAMPLE5PELICANMIC.aag";
        aig.ConstructAigerFile(fileName);

        RunIC3(fileName);
    }

    public static void TestExample5Pelican2()
    {
        // AbstractFirstOrderFormula varS = MakeVar("s");
        // AbstractFirstOrderFormula varB = MakeVar("b");
        // AbstractFirstOrderFormula varI = MakeVar("i");

        // button, request, old sh, old sl, sh, sl, pg, pgf, pr, tg, ta, tr, taf

        const string button = "button";
        const string request = "request";
        const string old_Sh = "old_sh";
        const string old_Sl = "old_sl";
        const string sh = "sh";
        const string sl = "sl";
        const string pg = "pg";
        const string pgf = "pgf";
        const string pr = "pr";
        const string tg = "tg";
        const string ta = "ta";
        const string tr = "tr";
        const string taf = "taf";

        AbstractFirstOrderFormula varButton = MakeVar(button);
        AbstractFirstOrderFormula varRequest = MakeVar(request);
        AbstractFirstOrderFormula varOld_sh = MakeVar(old_Sh);
        AbstractFirstOrderFormula varOld_sl = MakeVar(old_Sl);
        AbstractFirstOrderFormula varSH = MakeVar(sh);
        AbstractFirstOrderFormula varSL = MakeVar(sl);
        AbstractFirstOrderFormula varPG = MakeVar(pg);
        AbstractFirstOrderFormula varPGF = MakeVar(pgf);
        AbstractFirstOrderFormula varPR = MakeVar(pr);
        AbstractFirstOrderFormula varTG = MakeVar(tg);
        AbstractFirstOrderFormula varTA = MakeVar(ta);
        AbstractFirstOrderFormula varTR = MakeVar(tr);
        AbstractFirstOrderFormula varTAF = MakeVar(taf);

        Ladder ldr = new();

        Rung oshRung = new()
        {
            formula = varSH, // old_sh' ≡ sh
            output = old_Sh,
        };
        ldr.AddRung(oshRung);

        Rung oslRung = new()
        {
            formula = varSL, // old_sl' ≡ sl
            output = old_Sl,
        };
        ldr.AddRung(oslRung);

        Rung shRung = new()
        {
            formula = MakeOr(
                MakeAnd(varOld_sh, MakeNegation(varOld_sl)),
                MakeAnd(MakeNegation(varOld_sh), varOld_sl)
            ), // sh' ≡ (old_sh ∧ ¬ old_sl) ∨ (¬ old_sh ∧ old_sl)
            output = sh,
            Initialised = false, // sh isfalse
        };
        ldr.AddRung(shRung);

        Rung slRung = new()
        {
            formula = // sl' ≡ (old_sh ∧ ¬ old_sl) ∨ ((¬ request ∧ button) ^ ¬ old_sl))
            MakeOr(
                MakeAnd(varOld_sh, MakeNegation(varOld_sl)),
                MakeAnd(MakeAnd(MakeNegation(varRequest), varButton), MakeNegation(varOld_sl))
            ),
            output = sl,
            Initialised = false, // sl is false
        };
        ldr.AddRung(slRung);

        AbstractFirstOrderFormula requestRungFormula = // request' ≡ (button ^ ¬old_sh’) V (button ^ ¬old_sl’)
                                                       //    V (request ^  ¬button ^  ¬old_sh’)
                                                       //    V (request ^ ¬button ^ ¬old_sl’)
                                                       // is equivalent to:
                                                       // (button ^ ¬old_sh’) V (button ^ ¬old_sl’) V ((request ^  ¬button) ^  ¬old_sh’) V ((request ^ ¬button) ^ ¬old_sl’)

            MakeOr(
                MakeOr(
                    MakeOr(
                        MakeAnd(varButton, MakeNegation(varOld_sh)), // (button ^ ¬old_sh’)
                        MakeAnd(varButton, MakeNegation(varOld_sl)) // (button ^ ¬old_sl’)
                    ) // (button ^ ¬old_sh’) V (button ^ ¬old_sl’)
                    ,
                    MakeAnd(MakeAnd(varRequest, MakeNegation(varButton)), MakeNegation(varOld_sh)) // ((request ^  ¬button) ^  ¬old_sh’)
                ), // (button ^ ¬old_sh’) V (button ^ ¬old_sl’) V ((request ^  ¬button) ^  ¬old_sh’)
                MakeAnd(MakeAnd(varRequest, MakeNegation(varButton)), MakeNegation(varOld_sl)) // ((request ^ ¬button) ^ ¬old_sl’)
            ); // (button ^ ¬old_sh’) V (button ^ ¬old_sl’) V ((request ^  ¬button) ^  ¬old_sh’) V ((request ^ ¬button) ^ ¬old_sl’)

        _ = PrettyPrinter.Prettify(requestRungFormula);

        Rung requestRung = new()
        {
            formula = requestRungFormula,

            output = request,
            Initialised = false, // request is false
        };
        ldr.AddRung(requestRung);

        Rung pgRung = new()
        {
            formula = MakeAnd(varOld_sh, MakeNegation(varOld_sl)), // pg' ≡ old_sh ∧ ¬ old_sl
            output = pg,
        };
        ldr.AddRung(pgRung);

        Rung pgfRung = new()
        {
            formula = MakeAnd(varOld_sh, varOld_sl), // pgf' ≡ old_sh ∧ old_sl
            output = pgf,
        };
        ldr.AddRung(pgfRung);

        Rung prRung = new()
        {
            formula = MakeNegation(varOld_sh), // pr' ≡ ¬ old_sh
            output = pr,
            Initialised = true, // pr is initialised to true
        };
        ldr.AddRung(prRung);

        Rung tgRung = new()
        {
            formula = MakeOr(
                MakeAnd(MakeNegation(varOld_sh), MakeNegation(varOld_sl)), // tg' ≡ ¬ old_sh ∧ ¬ old_sl
                MakeAnd(MakeNegation(varButton), MakeNegation(varRequest)) // V (¬ button ∧ ¬ request)
            ), // tg' ≡ ¬ old_sh ∧ ¬ old_sl V (¬ button ∧ ¬ request)
            output = tg,
            Initialised = true, // tg is initialised to true
        };
        ldr.AddRung(tgRung);

        Rung taRung = new()
        {
            formula = MakeAnd(MakeNegation(varOld_sh), varOld_sl), // ta' ≡ ¬ old_sh ∧ old_sl
            output = ta,
        };
        ldr.AddRung(taRung);

        Rung trRung = new()
        {
            formula = MakeAnd(varOld_sh, MakeNegation(varOld_sl)), // tr' ≡ old_sh ∧ ¬ old_sl
            output = tr,
        };
        ldr.AddRung(trRung);

        Rung tafRung = new()
        {
            formula = MakeAnd(varOld_sh, varOld_sl), // taf' ≡ old_sh ∧ old_sl
            output = taf,
        };
        ldr.AddRung(tafRung);

        // SAFETY PROPERTY
        // // For tra c lights, exclusively one out of tg tatr taf shall be true:
        // AbstractFirstOrderFormula safety = MakeAnd(
        //     MakeNegation(varTG),
        //     MakeNegation(varTA),
        //     MakeNegation(varTAF)
        // ); // safety ≡ ¬ tg ∧ ¬ ta ∧ ¬ taf

        // for pedestrian lights, exclusively one out of pgpgfpr shall be true:
        // safety ≡ (pg ^ ¬ pgf ^ ¬ pr) V (¬ pg ^ pgf ^ ¬ pr) V (¬ pg ^ ¬ pgf ^ pr)
        // is equivalent to:
        // (((pg ^ ¬ pgf) ^ ¬ pr) V ((¬ pg ^ pgf) ^ ¬ pr)) V ((¬ pg ^ ¬ pgf )^ pr)
        AbstractFirstOrderFormula safety = MakeOr(
            MakeOr(
                MakeAnd(
                    MakeAnd(MakeAnd(varTG, MakeNegation(varTA)), MakeNegation(varTR)),
                    MakeNegation(varTAF)
                ), // // (((tg ^ ¬ ta) ^ ¬tr )^ ¬ taf)
                MakeAnd(
                    MakeAnd(MakeAnd(MakeNegation(varTG), varTA), MakeNegation(varTR)),
                    MakeNegation(varTAF)
                ) // (¬ tg ^ ta ^ ¬ tr ^ ¬ taf)
            ), //  // (((tg ^ ¬ ta) ^ ¬tr )^ ¬ taf) V (¬ tg ^ ta ^ ¬ tr ^ ¬ taf)
            MakeOr(
                MakeAnd(
                    MakeAnd(MakeAnd(MakeNegation(varTG), MakeNegation(varTA)), varTR),
                    MakeNegation(varTAF)
                ), // (¬ tg ^ ¬ ta ^ tr ^ ¬ taf)
                MakeAnd(
                    MakeAnd(MakeAnd(MakeNegation(varTG), MakeNegation(varTA)), MakeNegation(varTR)),
                    varTAF
                ) // (¬ tg ^ ¬ ta ^ ¬ tr ^ taf)
            ) // (¬ tg ^ ¬ ta ^ tr ^ ¬ taf) V (¬ tg ^ ¬ ta ^ ¬ tr ^ taf)
        ); // (((tg ^ ¬ ta) ^ ¬tr )^ ¬ taf) V (¬ tg ^ ta ^ ¬ tr ^ ¬ taf)
        //   V (¬ tg ^ ¬ ta ^ tr ^ ¬ taf) V (¬ tg ^ ¬ ta ^ ¬ tr ^ taf)

        AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);
        AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

        Model model = new(ldr);
        model.InitialiseModel();

        Ladder transformedLadder = TransformToAig.TransformLadder(ldr);

        Model modelForTfLadder = new(transformedLadder);
        modelForTfLadder.InitialiseModel();

        AigConstructor aig = new(
            transformedLadder,
            transformedSafety,
            modelForTfLadder.LatchNamesAndValues
        );
        aig.Decorate();
        string fileName = "TESTEXAMPLE5PELICAN2MIC.aag";
        aig.ConstructAigerFile(fileName);

        RunIC3(fileName);
    }

    // public static void TestExample5Pelican2()
    // {
    //     // AbstractFirstOrderFormula varS = MakeVar("s");
    //     // AbstractFirstOrderFormula varB = MakeVar("b");
    //     // AbstractFirstOrderFormula varI = MakeVar("i");

    //     // button, request, old sh, old sl, sh, sl, pg, pgf, pr, tg, ta, tr, taf

    //     const string button = "button";
    //     const string request = "request";
    //     const string oldSh = "old_sh";
    //     const string oldSl = "old_sl";
    //     const string sh = "sh";
    //     const string sl = "sl";
    //     const string pg = "pg";
    //     const string pgf = "pgf";
    //     const string pr = "pr";
    //     const string tg = "tg";
    //     const string ta = "ta";
    //     const string tr = "tr";
    //     const string taf = "taf";

    //     AbstractFirstOrderFormula varB = MakeVar(button);
    //     AbstractFirstOrderFormula varR = MakeVar(request);
    //     AbstractFirstOrderFormula varOSH = MakeVar(oldSh);
    //     AbstractFirstOrderFormula varOSL = MakeVar(oldSl);
    //     AbstractFirstOrderFormula varSH = MakeVar(sh);
    //     AbstractFirstOrderFormula varSL = MakeVar(sl);
    //     AbstractFirstOrderFormula varPG = MakeVar(pg);
    //     AbstractFirstOrderFormula varPGF = MakeVar(pgf);
    //     AbstractFirstOrderFormula varPR = MakeVar(pr);
    //     AbstractFirstOrderFormula varTG = MakeVar(tg);
    //     AbstractFirstOrderFormula varTA = MakeVar(ta);
    //     AbstractFirstOrderFormula varTR = MakeVar(tr);
    //     AbstractFirstOrderFormula varTAF = MakeVar(taf);

    //     Ladder ldr = new();

    //     Rung oshRung = new()
    //     {
    //         formula = varSH, // old_sh' ≡ sh
    //         output = oldSh,
    //     };
    //     ldr.AddRung(oshRung);

    //     Rung oslRung = new()
    //     {
    //         formula = varSL, // old_sl' ≡ sl
    //         output = oldSl,
    //     };
    //     ldr.AddRung(oslRung);

    //     Rung shRung = new()
    //     {
    //         formula = MakeOr(
    //             MakeAnd(varOSH, MakeNegation(varOSL)),
    //             MakeAnd(MakeNegation(varOSH), varOSL)
    //         ), // sh' ≡ (old_sh ∧ ¬ old_sl) ∨ (¬ old_sh ∧ old_sl)
    //         output = sh,
    //         Initialised = false,
    //     };
    //     ldr.AddRung(shRung);

    //     Rung slRung = new()
    //     {
    //         formula =  // sl' ≡ (old_sh ∧ ¬ old_sl) ∨ ((¬ request ∧ button) ^ ¬ old_sl))
    //             MakeOr(
    //                 MakeAnd(varOSH, MakeNegation(varOSL)),
    //                 MakeAnd(MakeAnd(MakeNegation(varR), varB), MakeNegation(varOSL))
    //             ),
    //         output = sl,
    //         Initialised = false
    //     };
    //     ldr.AddRung(slRung);

    //     AbstractFirstOrderFormula requestRungFormula = // request' ≡ (button ^ ¬old_sh’) V (button ^ ¬old_sl’)
    //                                                    //    V (request ^  ¬button ^  ¬old_sh’)
    //                                                    //    V (request ^ ¬button ^ ¬old_sl’)
    //                                                    // is equivalent to:
    //                                                    // (button ^ ¬old_sh’) V (button ^ ¬old_sl’) V ((request ^  ¬button) ^  ¬old_sh’) V ((request ^ ¬button) ^ ¬old_sl’)

    //             MakeOr(MakeOr(
    //                 MakeOr(
    //                 MakeAnd(varB, MakeNegation(varOSH)), // (button ^ ¬old_sh’)
    //                 MakeAnd(varB, MakeNegation(varOSL)) // (button ^ ¬old_sl’)
    //             ) // (button ^ ¬old_sh’) V (button ^ ¬old_sl’)
    //             , MakeAnd(MakeAnd(varR, MakeNegation(varB)), varOSH) // ((request ^  ¬button) ^  ¬old_sh’)
    //             ), // (button ^ ¬old_sh’) V (button ^ ¬old_sl’) V ((request ^  ¬button) ^  ¬old_sh’)
    //             MakeAnd(MakeAnd(varR, MakeNegation(varB)), varOSL) // ((request ^ ¬button) ^ ¬old_sl’)
    //             ); // (button ^ ¬old_sh’) V (button ^ ¬old_sl’) V ((request ^  ¬button) ^  ¬old_sh’) V ((request ^ ¬button) ^ ¬old_sl’)

    //     PrettyPrinter.Prettify(requestRungFormula);

    //     Rung requestRung = new()
    //     {
    //         formula = requestRungFormula,

    //         output = request,
    //         Initialised = false // request is not initialised to true
    //     };
    //     ldr.AddRung(requestRung);

    //     Rung pgRung = new()
    //     {
    //         formula = MakeAnd(varOSH, MakeNegation(varOSL)), // pg' ≡ old_sh ∧ ¬ old_sl
    //         output = pg
    //     };
    //     ldr.AddRung(pgRung);

    //     Rung pgfRung = new()
    //     {
    //         formula = MakeAnd(varOSH, varOSL), // pgf' ≡ old_sh ∧ old_sl
    //         output = pgf
    //     };
    //     ldr.AddRung(pgfRung);

    //     Rung prRung = new()
    //     {
    //         formula = MakeNegation(varOSH), // pr' ≡ ¬ old_sh
    //         output = pr,
    //         Initialised = true // pr is initialised to true
    //     };
    //     ldr.AddRung(prRung);

    //     Rung tgRung = new()
    //     {
    //         formula = MakeOr(
    //             MakeAnd(MakeNegation(varOSH), MakeNegation(varOSL)), // tg' ≡ ¬ old_sh ∧ ¬ old_sl
    //             MakeAnd(MakeNegation(varB), MakeNegation(varR)) // V (¬ button ∧ ¬ request)
    //         ), // tg' ≡ ¬ old_sh ∧ ¬ old_sl V (¬ button ∧ ¬ request)
    //         output = tg,
    //         Initialised = true
    //     };
    //     ldr.AddRung(tgRung);

    //     Rung taRung = new()
    //     {
    //         formula = MakeAnd(MakeNegation(varOSH), varOSL), // ta' ≡ ¬ old_sh ∧ old_sl
    //         output = ta
    //     };
    //     ldr.AddRung(taRung);

    //     Rung trRung = new()
    //     {
    //         formula = MakeAnd(varOSH, MakeNegation(varOSL)), // tr' ≡ old_sh ∧ ¬ old_sl
    //         output = tr
    //     };
    //     ldr.AddRung(trRung);

    //     Rung tafRung = new()
    //     {
    //         formula = MakeAnd(varOSH, varOSL), // taf' ≡ old_sh ∧ old_sl
    //         output = taf
    //     };
    //     ldr.AddRung(tafRung);

    //     // SAFETY PROPERTY
    //     // // For traffic lights, exclusively one out of tg, ta, taf shall be true:
    //     // (tg ^ ¬ ta ^ ¬tr ^ ¬ taf) V (¬ tg ^ ta ^ ¬ tr ^ ¬ taf)
    //     //   V (¬ tg ^ ¬ ta ^ tr ^ ¬ taf) V (¬ tg ^ ¬ ta ^ ¬ tr ^ taf)
    //     // is equivalent to:
    //     // (((tg ^ ¬ ta )^ ¬tr) ^ ¬ taf) V (¬ tg ^ ta ^ ¬ tr ^ ¬ taf)
    //     //   V (¬ tg ^ ¬ ta ^ tr ^ ¬ taf) V (¬ tg ^ ¬ ta ^ ¬ tr ^ taf)

    //     AbstractFirstOrderFormula safety =
    //         MakeOr(
    //             MakeOr(
    //                 MakeAnd(MakeAnd(MakeAnd(varTG, MakeNegation(varTA)), MakeNegation(varTR)), MakeNegation(varTAF)), // // (((tg ^ ¬ ta) ^ ¬tr )^ ¬ taf)
    //                 MakeAnd(MakeAnd(MakeAnd(MakeNegation(varTG), varTA), MakeNegation(varTR)), MakeNegation(varTAF)) // (¬ tg ^ ta ^ ¬ tr ^ ¬ taf)
    //             ), //  // (((tg ^ ¬ ta) ^ ¬tr )^ ¬ taf) V (¬ tg ^ ta ^ ¬ tr ^ ¬ taf)
    //             MakeOr(
    //                 MakeAnd(MakeAnd(MakeAnd(MakeNegation(varTG), MakeNegation(varTA)), varTR), MakeNegation(varTAF)), // (¬ tg ^ ¬ ta ^ tr ^ ¬ taf)
    //                 MakeAnd(MakeAnd(MakeAnd(MakeNegation(varTG), MakeNegation(varTA)), MakeNegation(varTR)), varTAF) // (¬ tg ^ ¬ ta ^ ¬ tr ^ taf)
    //             ) // (¬ tg ^ ¬ ta ^ tr ^ ¬ taf) V (¬ tg ^ ¬ ta ^ ¬ tr ^ taf)
    //         ); // (((tg ^ ¬ ta) ^ ¬tr )^ ¬ taf) V (¬ tg ^ ta ^ ¬ tr ^ ¬ taf)
    //            //   V (¬ tg ^ ¬ ta ^ tr ^ ¬ taf) V (¬ tg ^ ¬ ta ^ ¬ tr ^ taf)

    //     AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);
    //     AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

    //     Model model = new(ldr);
    //     model.InitialiseModel();

    //     Ladder transformedLadder = TransformToAig.TransformLadder(ldr);

    //     Model modelForTfLadder = new(transformedLadder);
    //     modelForTfLadder.InitialiseModel();

    //     AigConstructor aig = new(transformedLadder, transformedSafety, modelForTfLadder.LatchNamesAndValues);
    //     aig.Decorate();
    //     string fileName = "TESTEXAMPLE5PELICAN2MIC.aag";
    //     aig.ConstructAigerFile(fileName);

    //     RunIC3(fileName);

    // }

    // public static List<Model> TestExample3()
    // {
    //    AbstractFirstOrderFormula varX = MakeVar("x");

    //    Ladder ladder = new Ladder();

    //    AbstractFirstOrderFormula form1 = MakeNegation(MakeAnd(MakeNegation(varX), varX));
    //    Rung rung1 = new Rung();
    //    rung1.formula = form1;
    //    rung1.output = "x";
    //    ladder.AddRung(rung1);

    //    AbstractFirstOrderFormula safety = varX;
    //    AbstractFirstOrderFormula negatedSafety = MakeNegation(safety);

    //    AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

    //    Model model = new Model(ladder);
    //    model.InitialiseModel();

    //    Ladder transformedLadder = TransformToAig.TransformLadder(ladder);

    //    Model modelForTfLadder = new Model(transformedLadder);
    //    modelForTfLadder.InitialiseModel();

    //    AigConstructor aigConstructor = new AigConstructor(transformedLadder, transformedSafety, model.LatchNamesAndValues);
    //    aigConstructor.Decorate();
    //    aigConstructor.ConstructAigerFile();

    //    return new List<Model>() { model, modelForTfLadder };
    // }

    //public static List<Model> TestExample4()
    //{
    //    //string ladderLogicFile = "822.xml";
    //    string ladderLogicFile = Path.Combine(@"SiemensData\LochNess-810\810.xml");

    //    // run for all 810s
    //    XmlDocument doc = new XmlDocument();
    //    doc.Load(ladderLogicFile);
    //    Ladder ladder = LadderLogicXmlParser.ParseXML(doc);

    //    //string safetyPropertyString = "\"S6915.(SPU)INHIBIT(FL)_0\" & \"S6915.(SPU)INHIBIT(FL)_1\"";
    //    //string safetyPropertyString = "\"IL822.(EN)CAN\"";
    //    string safetyPropertyString = "(\"S6909(AM).U_0\") -> (\"S6909(BM).N_0\")";

    //    //string safetyFile = Path.Combine(@"SiemensData\Additional_LochNess_Properties\LochNess810_GSP_ConflictingRoutes\group0.cond");

    //    //string safetyPropertyString = File.ReadLines(safetyFile).Skip(4).Take(1).First();

    //    AbstractFirstOrderFormula safetyCondition = SafetyPropertyParser.Parse(safetyPropertyString);
    //    //PrettyPrinter.PrettyPrint(safetyCondition);
    //    AbstractFirstOrderFormula negatedSafety = MakeNegation(safetyCondition);
    //    AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(negatedSafety);

    //    Model model = new Model(ladder);
    //    model.InitialiseModel();

    //    Ladder transformedLadder = TransformToAig.TransformLadder(ladder);

    //    Model modelForTfLadder = new Model(transformedLadder);
    //    modelForTfLadder.InitialiseModel();

    //    AigConstructor aigConstructor = new AigConstructor(transformedLadder, transformedSafety, model.LatchNamesAndValues);
    //    aigConstructor.Decorate();
    //    aigConstructor.ConstructAigerFile();

    //    //ProcessStartInfo startInfo = new ProcessStartInfo() { FileName = "wsl", Arguments = "/mnt/c/Users/LLV/source/repos/SwanLLVerifier/SwanLLVerifier/bin/Release/net6.0/test.aag" };
    //    //ProcessStartInfo startInfo = new ProcessStartInfo
    //    //{
    //    //    WindowStyle = ProcessWindowStyle.Hidden,
    //    //    FileName = "cmd.exe",
    //    //    Arguments = "~/swansea-uni/IC3ref/IC3 < "
    //    //};

    //    //Process proc = new Process() { StartInfo = startInfo, };
    //    //proc.Start();
    //    //proc.WaitForExit();
    //    //Console.WriteLine("###########################");
    //    //Console.WriteLine(proc.ExitCode);

    //    return new List<Model>() { model, modelForTfLadder };
    //}
}

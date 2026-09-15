using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;
using SwanLLVerifier.LadderLogic;
using SwanLLVerifier.Utils;

namespace SwanLLVerifier.AIG
{
    public class AigConstructor
    {
        private readonly IDictionary<string, bool> latchNamesAndValues =
            new Dictionary<string, bool>();
        private readonly IEnumerable<string> allLatchVariables;
        private readonly IDictionary<string, int> decoratedTrees = new Dictionary<string, int>();

        private readonly IDictionary<string, int> decoratedVarNames = new Dictionary<string, int>();

        // _1 (nextcycle) variables for regular (_0) varNames
        private readonly IDictionary<string, int> decoratedVarNamesNextCycle =
            new Dictionary<string, int>();
        private readonly IDictionary<string, int> decoratedLatchVariables =
            new Dictionary<string, int>();

        private readonly Ladder ladder = new();
        private readonly AbstractFirstOrderFormula safety;

        private readonly string safetyPropertyName = "SAFETY";

        private readonly List<string> aigerFileLines = new();

        private readonly List<string> detailedAigerFileLines = new();

        private int maxLiteralIndex = 0;
        public int rungIndex = 0;

        public AigConstructor(
            Ladder ldr,
            AbstractFirstOrderFormula sfty,
            IDictionary<string, bool> latchNameVal
        )
        {
            ladder = ldr;
            safety = sfty;
            allLatchVariables = ladder.Rungs.Select(r => r.output);
            latchNamesAndValues = latchNameVal;
        }

     

        public void Decorate()
        {
            AssignLiteralNumberToAllInputs();
            AssignLiteralNumberToAllOutputs();
            DecorateLatchesMic();
            DecorateSafety();
        }

        public static string FormatVarName(string varName)
        {
            string formattedVarName = varName;

            if (varName.StartsWith("v")) // ladder.tptp has all variables starting with the character v
                formattedVarName = varName.Substring(1, varName.Length - 1);

            if (varName.EndsWith("_0") || varName.EndsWith("_1")) //
                formattedVarName = formattedVarName.Substring(0, formattedVarName.Length - 2);

            formattedVarName = formattedVarName.Replace(".", "_");
            formattedVarName = formattedVarName.Replace("(", "_");
            formattedVarName = formattedVarName.Replace(")", "_");
            formattedVarName = formattedVarName.Replace("/", "_");

            return formattedVarName;
        }

      

        public void AssignLiteralNumberToAllInputs()
        {
            // Write out all input and assign their variable/literal (inputs is easily determined
            //  by variables that appears only to the left of the transition relations functions)
            ISet<string> allInputVariables = ladder.AllInputs();
            foreach (string varName in allInputVariables)
            {
                // maxLiteralIndex += 2;
                string formattedVarName = varName;

            

                formattedVarName = FormatVarName(formattedVarName);

                if (
                    !decoratedVarNames.ContainsKey(formattedVarName)
                    || !decoratedVarNamesNextCycle.ContainsKey($"{formattedVarName}_1")
                )
                {
                    maxLiteralIndex += 2;
                }

                if (!decoratedVarNames.ContainsKey(formattedVarName))
                    decoratedVarNames.Add(formattedVarName, maxLiteralIndex);

                // also decorate the next cycle variable (_1) for the current variable
                // with the same maxLiteralIndex decoration
                if (!decoratedVarNamesNextCycle.ContainsKey($"{formattedVarName}_1"))
                    decoratedVarNamesNextCycle.Add($"{formattedVarName}_1", maxLiteralIndex);
                // Console.WriteLine($"Decorated variable: {formattedVarName} with value: {maxLiteralIndex}");
            }
        }

        public void AssignLiteralNumberToAllOutputs()
        {
            Console.WriteLine("Assigning literal numbers to all outputs...");
            // Write out all output and assign their variable/literal (outputs is easily determined
            //  by variables that appears only to the right of the transition relations functions)
            ISet<string> allOutputVariables = ladder.AllOutputVariables();
            foreach (string varName in allOutputVariables)
            {
                // maxLiteralIndex += 2;
                string formattedVarName = varName;

             

                formattedVarName = FormatVarName(formattedVarName);

                if (
                    !decoratedVarNames.ContainsKey(formattedVarName)
                    || !decoratedVarNamesNextCycle.ContainsKey($"{formattedVarName}_1")
                )
                {
                    maxLiteralIndex += 2;
                }

                if (!decoratedVarNames.ContainsKey(formattedVarName))
                    decoratedVarNames.Add(formattedVarName, maxLiteralIndex);

                // also decorate the next cycle variable (_1) for the current variable
                // with the same maxLiteralIndex decoration
                if (!decoratedVarNamesNextCycle.ContainsKey($"{formattedVarName}_1"))
                    decoratedVarNamesNextCycle.Add($"{formattedVarName}_1", maxLiteralIndex);
                // Console.WriteLine($"Decorated variable: {formattedVarName} with value: {maxLiteralIndex}");
            }
        }


        public void DecorateLatchesMic()
        {
            // Console.WriteLine("No of rungs: " + ladder.Rungs.Count);
            // foreach (var rung in ladder.Rungs)
            // {
            //     Console.WriteLine($"Output: {rung.output}, Formula: {PrettyPrinter.Prettify(rung.formula)}");
            // }
            for (int i = 0; i < ladder.Rungs.Count; i++)
            {
                // Console.WriteLine($"Decorating rung {i}: {ladder.Rungs[i].output}");
                string rungOutput = ladder.Rungs[i].output;


                rungOutput = FormatVarName(rungOutput);

                string latchVariable = $"{rungOutput}_LATCH";
                // console the formula properties
                int decoratedValue = DecorateFormulaTree(ladder.Rungs[i].formula, latchVariable);

                // Console.WriteLine($"====> Complete decoratedValue {PrettyPrinter.Prettify(ladder.Rungs[i].formula)}: {decoratedValue} for latch variable: {latchVariable}", ConsoleColor.Green);

                if (decoratedValue < 2)
                    throw new ArgumentOutOfRangeException(
                        $"decoratedValue OutOfRange: {decoratedValue}"
                    );

                // Console.WriteLine($"Decorated latch variable: {latchVariable} with value: {decoratedValue}");
                if (!decoratedLatchVariables.ContainsKey(latchVariable)) // check if the latch variable is already decorated
                {
                    decoratedLatchVariables.Add(latchVariable, decoratedValue);

                    //File.AppendAllText("latches.json", $"{latchVariable}: {decoratedValue},\n");
                }
            }
        }

        public void DecorateSafety()
        {
            // Console.WriteLine("Decorating safety property..." + PrettyPrinter.Prettify(safety));
            _ = DecorateFormulaTree(safety, safetyPropertyName);
            // Ensure that the safety property is decorated correctly
            // if (!decoratedTrees.ContainsKey(safetyPropertyName))
            // {
            //     throw new Exception("Safety property decoration failed. 'SAFETY' key not found in decoratedTrees.");
            // }
            // Console.WriteLine("Safety property decorated successfully.");
        }

        
        public int DecorateFormulaTree(AbstractFirstOrderFormula formula, string parentLatchVarName) // This function is recursive
        {
            int decoratedValue = -1;
            // Console all the formulas properties

            // Console.WriteLine($"In formula: {PrettyPrinter.Prettify(formula)} with  name: {parentLatchVarName}");

            switch (formula)
            {
                case Predicate:
                    Predicate p = (Predicate)formula;
                    string formattedpredName;

                    if (p.Name.EndsWith("_1"))
                    {
                        formattedpredName = FormatVarName(p.Name);

                        if (decoratedLatchVariables.ContainsKey($"{formattedpredName}_LATCH"))
                        {
                            formattedpredName = $"{formattedpredName}_LATCH";
                        }
                        // else
                        //     throw new Exception($"Expected next cycle latch variable {formattedpredName}_LATCH to be pre-included in decoratedVarNamesNextCycle dictionary.");
                    }
                    else if (p.Name.EndsWith("_0"))
                    {
                        formattedpredName = FormatVarName(p.Name);
                    }
                    else
                    {
                        throw new Exception(
                            $"Predicate name {p.Name} does not end with _0 or _1 as expected."
                        );
                    }

                   

                    if (decoratedVarNames.ContainsKey(formattedpredName))
                        decoratedValue = decoratedVarNames[formattedpredName];
                    else if (decoratedVarNamesNextCycle.ContainsKey(formattedpredName))
                        decoratedValue = decoratedVarNamesNextCycle[formattedpredName];
                    else if (decoratedLatchVariables.ContainsKey(formattedpredName))
                        decoratedValue = decoratedLatchVariables[formattedpredName];
                    else
                    {
                        Console.WriteLine(
                            $"Predicate name {formattedpredName} not found in decoratedVarNames, decoratedVarNamesNextCycle, or decoratedLatchVariables dictionaries."
                        );
                        throw new Exception(
                            "Expected predicate keys to be pre-included in a dictionary."
                        );
                    }

                    break;

                case Negation:
                    Negation neg = (Negation)formula;

                    // decoratedValue = DecorateFormulaTree(neg.Operand, parentLatchVarName) + 1;
                    decoratedValue = DecorateFormulaTree(neg.Operand, parentLatchVarName);
                    
                    decoratedValue =
                        (decoratedValue % 2 == 0) ? decoratedValue + 1 : decoratedValue - 1;
                    if (!decoratedTrees.ContainsKey($"{parentLatchVarName}_NEG_{decoratedValue}"))
                        decoratedTrees.Add(
                            $"{parentLatchVarName}_NEG_{decoratedValue}",
                            decoratedValue
                        );
                    string correspondingLatchString = decoratedVarNames
                        .FirstOrDefault(x => x.Value == decoratedValue - 1)
                        .Key;

                    if (
                        !decoratedTrees.ContainsKey(
                            $"NEGATION_OF_{neg.Operand} ({correspondingLatchString})"
                        )
                    )
                    {
                        decoratedTrees.Add(
                            $"NEGATION_OF_{neg.Operand} ({correspondingLatchString})",
                            decoratedValue
                        );
                    }

                    break;

                case And:
                    And and = (And)formula;
          

                    int leftValue = DecorateFormulaTree(and.LeftOperand, parentLatchVarName);
                    int rightValue = DecorateFormulaTree(and.RightOperand, parentLatchVarName);

                    maxLiteralIndex += 2; // This caused same repetitive values for AND gates that were not unique (just with swapped left and right values) e.g  !x /\ x value: 4 same as x /\ !x formula: (x /\ !x) value: 6 (Consider Example 4)
                    decoratedTrees.Add(
                        $"{parentLatchVarName}_AND_{maxLiteralIndex}_{leftValue}_{rightValue}",
                        maxLiteralIndex
                    );
                    decoratedValue = maxLiteralIndex;

                    break;

                case Brackets:
                    Brackets br = (Brackets)formula;
                    decoratedValue = DecorateFormulaTree(br.Operand, parentLatchVarName);
                    break;
            }

            if (decoratedValue < 2 || decoratedValue == -1)
                throw new ArgumentOutOfRangeException($"Abnormal decoratedValue: {decoratedValue}");

            if (
                !decoratedTrees.ContainsKey(safetyPropertyName)
                && parentLatchVarName == safetyPropertyName
            )
                decoratedTrees.Add(safetyPropertyName, decoratedValue);
            // Console.WriteLine($"formula: {PrettyPrinter.Prettify(formula)} value: {decoratedValue}");

            return decoratedValue;
        } // end DecorateFormulaTree

      
        public void ConstructAigerFile(string fileName = "test.aag")
        {
            IDictionary<int, int> latchLiteralsAndLatchInputs = new Dictionary<int, int>();
            Console.WriteLine("Constructing AIGER file...");

            // are all coils latches? Probably YES (based on the examples that we've been doing). Confirm with MR later.
            // mapping latch variable names to their latch literal names and their decorated values
            foreach (string inputKey in decoratedLatchVariables.Keys)
            {
                string originalCoilName = inputKey.Replace("_LATCH", "");
                latchLiteralsAndLatchInputs.Add(
                    decoratedVarNames[originalCoilName],
                    decoratedLatchVariables[inputKey]
                );
            }

            int numberOfLatches = decoratedLatchVariables.Count;

            // if it's not a latch can we say that it's an input?
            List<string> inputsOnly = new();
            foreach (string varKey in decoratedVarNames.Keys)
            {
                if (!decoratedLatchVariables.ContainsKey($"{varKey}_LATCH"))
                    inputsOnly.Add(varKey);
            }

            // INPUTS
            foreach (string inputKey in inputsOnly)
            {
                aigerFileLines.Add($"{decoratedVarNames[inputKey]}\n");
            }
            detailedAigerFileLines.AddRange(
                inputsOnly.Select(input =>
                    $"INPUT {input} with value: {decoratedVarNames[input]}\n"
                )
            );

            int numberOfInputs = inputsOnly.Count;
            detailedAigerFileLines.Add($"Total number of inputs: {numberOfInputs}\n");

            // Connsole decorated var names in one line
            // Console.WriteLine("Decorated Var Names Dictonary: " + string.Join(", ", decoratedVarNames.Select(kvp => $"{kvp.Key}: {kvp.Value}")));
            // Console latchNamesAndValues dictionary in one line
            // Console.WriteLine("Latch Names And Values Dictonary: " + string.Join(", ", latchNamesAndValues.Select(kvp => $"{kvp.Key}: {kvp.Value}")));

            // LATCHES
            foreach (int latchLiteral in latchLiteralsAndLatchInputs.Keys)
            {
                string correspondingLatchString = decoratedVarNames
                    .FirstOrDefault(x => x.Value == latchLiteral)
                    .Key;
                // get the rung output with the corresponding latch string
                Rung? correspondingRung = ladder.Rungs.FirstOrDefault(r =>
                    FormatVarName(r.output) == correspondingLatchString
                );
                bool initialLatchValue = latchNamesAndValues[$"{correspondingLatchString}_LATCH"];
                if (correspondingRung == null)
                {
                    throw new ArgumentNullException(
                        $"Corresponding rung for latch {correspondingLatchString} not found."
                    );
                }
                // Console.WriteLine($"Not Sure yet");
                //It is essential to check for null before dereferencing to avoid runtime exceptions.
                if (correspondingRung.Initialised != null)
                {
                    initialLatchValue = (bool)correspondingRung.Initialised; // if the rung is initialised, we set the value to true
                }

                int convertedInitialLatchValue = initialLatchValue ? 1 : 0;

                aigerFileLines.Add(
                    $"{latchLiteral} {latchLiteralsAndLatchInputs[latchLiteral]} {convertedInitialLatchValue}\n"
                );
                detailedAigerFileLines.Add(
                    $"LATCH {correspondingLatchString} => {latchLiteral} with value: {latchLiteralsAndLatchInputs[latchLiteral]} and initial value: {convertedInitialLatchValue}\n"
                );
            }

            // OUTPUT (always 1 output only i.e. from safety property which is treated as a bad state literal)
            // last value of the decoratedTrees dictionary is always the output of the safety property
            aigerFileLines.Add($"{decoratedTrees.Values.Last()}\n");
            detailedAigerFileLines.Add(
                $"Output (safety property): {decoratedTrees.Values.Last()}\n"
            );

            // AND gates
            int numberOfAndGates = 0;
            foreach (string input_key in decoratedTrees.Keys)
            {
                // Console.WriteLine($"Processing input key: {input_key}");
                if (input_key.Contains("_AND_"))
                {
                    // Console.WriteLine($"Found AND gate in input key: {input_key}");
                    string[] splittedInputKey = input_key.Split('_').TakeLast(3).ToArray();
                    // Console.WriteLine($"Splitted input key: {string.Join(", ", splittedInputKey)}");
                    aigerFileLines.Add(
                        $"{splittedInputKey[0]} {splittedInputKey[1]} {splittedInputKey[2]}\n"
                    );
                    detailedAigerFileLines.Add(
                        $"AND gate: {splittedInputKey[0]} {splittedInputKey[1]} {splittedInputKey[2]} \n"
                    );
                    // add the string of the splittedInputKey[1] and splittedInputKey[2] to detailedAigerFileLines for debugging
                    // detailedAigerFileLines.Add($"  left operand literal: {decoratedTrees.FirstOrDefault(x => x.Value.ToString() == splittedInputKey[1]).Key}\n");
                    // detailedAigerFileLines.Add($"  right operand literal: {decoratedTrees.FirstOrDefault(x => x.Value.ToString() == splittedInputKey[2]).Key}\n");
                    //  add to detailedAigerFileLines the return value of PrettyPrinter.Prettify for the corresponding formula
                    // detailedAigerFileLines.Add($"Corresponding formula: {PrettyPrinter.Prettify(ladder.Rungs[rungIndex].formula)}");
                    numberOfAndGates++;
                }
            }

            // Add decoratedLatchVariables, decoratedTrees, decoratedVarNames,
            // decoratedVarNamesNextCycle,  latchNamesAndValues to detailedAigerFileLines for debugging
            detailedAigerFileLines.Add("\n=== Decorated Var Names ===\n");
            foreach (KeyValuePair<string, int> kvp in decoratedVarNames)
                detailedAigerFileLines.Add($"Key = {kvp.Key}, Value = {kvp.Value}\n");
            detailedAigerFileLines.Add("\n=== Decorated Var Names Next Cycle ===\n");
            foreach (KeyValuePair<string, int> kvp in decoratedVarNamesNextCycle)
                detailedAigerFileLines.Add($"Key = {kvp.Key}, Value = {kvp.Value}\n");
            detailedAigerFileLines.Add("\n=== Decorated Latch Variables ===\n");
            foreach (KeyValuePair<string, int> kvp in decoratedLatchVariables)
                detailedAigerFileLines.Add($"Key = {kvp.Key}, Value = {kvp.Value}\n");
            detailedAigerFileLines.Add("\n=== Decorated Trees ===\n");
            foreach (KeyValuePair<string, int> kvp in decoratedTrees)
                detailedAigerFileLines.Add($"Key = {kvp.Key}, Value = {kvp.Value}\n");
            detailedAigerFileLines.Add("\n=== Latch Names And Values ===\n");
            foreach (KeyValuePair<string, bool> kvp in latchNamesAndValues)
                detailedAigerFileLines.Add($"Key = {kvp.Key}, Value = {kvp.Value}\n");

            int maxVariableIndex = numberOfInputs + numberOfLatches + numberOfAndGates;
            // M I L O A BadStateLiteral
            string firstLine =
                $"aag {maxVariableIndex} {numberOfInputs} {numberOfLatches} 0 {numberOfAndGates} 1\n";

            Console.WriteLine("============= CREATING AIGERFILE ==============");
            // put the files in results directory
            if (!Directory.Exists("results"))
            {
                Directory.CreateDirectory("results");
            }
            fileName = Path.Combine("results", fileName);
            // create Directory of fileName if it doesn't exist
            string? directoryName = Path.GetDirectoryName(fileName);
            if (directoryName != null && !Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
            using StreamWriter writer = new(fileName);
            writer.Write(firstLine);
            foreach (string line in aigerFileLines)
                writer.Write(line);
            // Write detailed information to a separate file for debugging
            string detailedFileName = fileName.Replace(".aag", "_detailed.txt");
            using StreamWriter detailedWriter = new(detailedFileName);
            detailedWriter.Write("Detailed AIGER File Information:");
            detailedWriter.Write(firstLine);
            foreach (string detailedLine in detailedAigerFileLines)
                detailedWriter.Write(detailedLine);
        } // end ConstructAigerFile
    }
}

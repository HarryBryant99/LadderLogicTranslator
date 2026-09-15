using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;
using SwanLLVerifier.LadderLogic;
using SwanLLVerifier.AIG;

public class Model
{
    private readonly Ladder ladder = new();

    // key-value pair of variable names as string and their computed value
    private readonly IDictionary<string, bool> latchNamesAndValues = new Dictionary<string, bool>();

    // constructor
    public Model(Ladder ldr)
    {
        ladder = ldr;
    }

    public IDictionary<string, bool> LatchNamesAndValues { get { return latchNamesAndValues; } }

    public void InitialiseModel()
    {
        // initialisation to FALSE is the first thing to do as quoted by Siemens
        foreach (Rung rung in ladder.Rungs)
        {
            string rungOutput = rung.output;

            // if (rungOutput.StartsWith("v")) // ladder.tptp has all variables starting with the character v
            //     rungOutput = rungOutput.Substring(1, rungOutput.Length - 1);

            // if (rungOutput.EndsWith("_0") || rungOutput.EndsWith("_1")) //
            //     rungOutput = rungOutput.Substring(0, rungOutput.Length - 2);

            // rungOutput = rungOutput.Replace(".", "_");
            // rungOutput = rungOutput.Replace("(", "_");
            // rungOutput = rungOutput.Replace(")", "_");

            rungOutput = AigConstructor.FormatVarName(rungOutput);

            // check if the rungOutput already exists in the dictionary
            if (!latchNamesAndValues.ContainsKey($"{rungOutput}_LATCH"))
                latchNamesAndValues.Add(new KeyValuePair<string, bool>($"{rungOutput}_LATCH", ParseAndEvaluate(rung.formula)));


        }
        // Print the latch names and values for debugging
        // Console.WriteLine("Latch Names and Values:");
        // foreach (var kvp in latchNamesAndValues)
        // {
        //     Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        // }
    }

    // works for both AIG formulae and LL formulae
    public bool ParseAndEvaluate(AbstractFirstOrderFormula formula)
    {
        return formula switch
        {
            Predicate predicate => GetVariableValueFromDictionary(predicate.Name),
            And and => ParseAndEvaluate(and.LeftOperand) & ParseAndEvaluate(and.RightOperand),
            Implies implies => !(ParseAndEvaluate(implies.LeftOperand) & !ParseAndEvaluate(implies.RightOperand)),
            Equivalent equivalent => (ParseAndEvaluate(equivalent.LeftOperand) == ParseAndEvaluate(equivalent.RightOperand)),
            Or or => (ParseAndEvaluate(or.LeftOperand) | ParseAndEvaluate(or.RightOperand)),
            Negation negation => !ParseAndEvaluate(negation.Operand),
            Brackets brackets => ParseAndEvaluate(brackets.Operand),
            _ => throw new ArgumentException($"Unhandled formula: {formula}")
        };
    }

    // given input variables, state variables, output variables and timers
    // input variables and timers are set to FALSE
    // state variables and output variables obtain the value through computation of a rung in the ladder
    // everything with a C_i (which appears as a coil) is a state variable / output variable
    // everything else is a timer or input
    // the dictionary 

    private bool GetVariableValueFromDictionary(string predicateName)
    {
        if (latchNamesAndValues.ContainsKey($"{predicateName}_LATCH"))
            return latchNamesAndValues[$"{predicateName}_LATCH"];
        else
            return false;
    }
}
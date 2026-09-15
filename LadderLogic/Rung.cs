using SwanLLVerifier.PropositionalLogic;
//using SwanLLVerifier.ETCSDC_Properties;
using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;

namespace SwanLLVerifier.LadderLogic
{
    public class Rung
    {
        public string output { get; set; } = string.Empty;

        public AbstractFirstOrderFormula formula { get; set; } = null!;

        public bool? Initialised { get; set; }

        public ISet<string> AllVariables()
        {
            ISet<string> allVariables = PropositionalFormulaUtils.AllVariablesFromFormula(formula);
            _ = allVariables.Add(output);

            return allVariables;
        }
    }
}

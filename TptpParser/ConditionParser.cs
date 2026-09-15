using System.Text;
//using SwanLLVerifier.ETCSDC_Properties;
using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;
using static SwanLLVerifier.PropositionalLogic.PropositionalFormulaBuilder;

namespace SwanLLVerifier.TptpParser
{
    public static class ConditionParser
    {
        private const string Prefix = "fof(ax,axiom, ";
        private const string Postfix = ").";

        public static AbstractFirstOrderFormula ParseTptpSafety(string tptpSafetyFilePath)
        {
            AbstractFirstOrderFormula tptpParsedSafety = MakeVar("null"); // dummy initialisation

            using (var reader = new StreamReader(tptpSafetyFilePath, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()!) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    //Debug.Assert(line.StartsWith(Prefix));
                    //Debug.Assert(line.EndsWith(Postfix));

                    string lineBody = line.Substring(Prefix.Length, line.Length - Prefix.Length - Postfix.Length);

                    tptpParsedSafety = Parser.Parse(lineBody);

                    // read the first line only
                    break;
                }
            }

            return tptpParsedSafety;
        }
    }
}



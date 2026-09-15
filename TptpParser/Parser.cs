using System;
using System.Collections.Generic;
using static SwanLLVerifier.PropositionalLogic.PropositionalFormulaBuilder;
//using SwanLLVerifier.ETCSDC_Properties;
using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;
using SwanLLVerifier.AIG;


namespace SwanLLVerifier.TptpParser
{
    public class Parser
    {
        public static AbstractFirstOrderFormula Parse(string data)
        {
            Tokeniser t = new Tokeniser(data);
            return ParseExpression(t);
        }

        private static AbstractFirstOrderFormula ParseExpression(Tokeniser t)
        {
            return ParseEquivalence(t);
        }

        private static AbstractFirstOrderFormula ParseEquivalence(Tokeniser t)
        {
            AbstractFirstOrderFormula lhs = ParseDisjunction(t);

            while (t.IsMatchAndAdvance("<=>"))
            {
                AbstractFirstOrderFormula rhs = ParseDisjunction(t);
                lhs = MakeEquivalence(lhs, rhs);
            }
            return lhs;
        }

        private static AbstractFirstOrderFormula ParseDisjunction(Tokeniser t)
        {
            AbstractFirstOrderFormula lhs = ParseConjunction(t);

            while (t.IsMatchAndAdvance("|"))
            {
                AbstractFirstOrderFormula rhs = ParseConjunction(t);
                lhs = MakeOr(lhs, rhs);
            }
            return lhs;
        }

        private static AbstractFirstOrderFormula ParseConjunction(Tokeniser t)
        {
            AbstractFirstOrderFormula lhs = ParseNegation(t);

            while (t.IsMatchAndAdvance("&"))
            {
                AbstractFirstOrderFormula rhs = ParseNegation(t);
                lhs = MakeAnd(lhs, rhs);
            }
            return lhs;
        }

        private static AbstractFirstOrderFormula ParseNegation(Tokeniser t)
        {
            if (t.IsMatchAndAdvance("~"))
            {
                AbstractFirstOrderFormula rhs = ParseNegation(t);
                return MakeNegation(rhs);
            }
            return ParsePrimary(t);
        }

        private static AbstractFirstOrderFormula ParsePrimary(Tokeniser t)
        {
            string variable = t.PeekVariableAndAdvance()!;
            if (variable != null)
            {
                // variable = AigConstructor.FormatVarName(variable);
                return MakeVar(variable);
            }

            if (t.IsMatchAndAdvance("("))
            {
                AbstractFirstOrderFormula exp = ParseExpression(t);
                if (t.IsMatchAndAdvance(")"))
                {
                    return exp;
                }

                throw new ParserException(t.GetPosition() + 1, "Expecting ')' after expression");
            }
            throw new ParserException(t.GetPosition() + 1, "Unexpected symbol " + t.Peek().ToString());
        }
    }
}
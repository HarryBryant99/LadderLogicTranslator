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
// Description:  This file defines the SafetyPropertyParser class, which parses a textual safety property into an internal propositional 
// logic formula tree, supporting variables, negation, conjunction, disjunction, implication, and equivalence operators.
//
// ------------------
//
// GPL-3.0 license
//

//using SwanLLVerifier.ETCSDC_Properties;
using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;
using PFB = SwanLLVerifier.PropositionalLogic.PropositionalFormulaBuilder;

namespace SwanLLVerifier.SafetyProperty
{
    public class SafetyPropertyParser
    {
        public static AbstractFirstOrderFormula Parse(string safetyProperty)
        {
            Tokenizer tokenizer = new(safetyProperty);
            return Equivalence(tokenizer);
        }

        private static AbstractFirstOrderFormula Expression(Tokenizer tokenizer)
        {
            return Equivalence(tokenizer);
        }

        private static AbstractFirstOrderFormula Equivalence(Tokenizer tokenizer)
        {
            AbstractFirstOrderFormula implication = Implication(tokenizer);

            while (tokenizer.PeekIsMatchAdvance("<->"))
            {
                AbstractFirstOrderFormula right = Implication(tokenizer);
                implication = PFB.MakeEquivalence(implication, right);
            }

            return implication;
        }

        private static AbstractFirstOrderFormula Implication(Tokenizer tokenizer)
        {
            AbstractFirstOrderFormula or = Or(tokenizer);

            while (tokenizer.PeekIsMatchAdvance("->"))
            {
                AbstractFirstOrderFormula right = Or(tokenizer);
                or = PFB.MakeImplication(or, right);
            }

            return or;
        }

        private static AbstractFirstOrderFormula Or(Tokenizer tokenizer)
        {
            AbstractFirstOrderFormula and = And(tokenizer);

            while (tokenizer.PeekIsMatchAdvance("|"))
            {
                AbstractFirstOrderFormula right = And(tokenizer);
                and = PFB.MakeOr(and, right);
            }

            return and;
        }

        private static AbstractFirstOrderFormula And(Tokenizer tokenizer)
        {
            AbstractFirstOrderFormula negation = Negation(tokenizer);

            while (tokenizer.PeekIsMatchAdvance("&"))
            {
                AbstractFirstOrderFormula right = Negation(tokenizer);
                negation = PFB.MakeAnd(negation, right);
            }

            return negation;
        }

        private static AbstractFirstOrderFormula Negation(Tokenizer tokenizer)
        {
            if (tokenizer.PeekIsMatchAdvance("~"))
            {
                AbstractFirstOrderFormula right = Negation(tokenizer);
                return PFB.MakeNegation(right);
            }

            return Primary(tokenizer);
        }

        private static AbstractFirstOrderFormula Primary(Tokenizer tokenizer)
        {
            if (tokenizer.PeekIsVariable())
            {
                string token = tokenizer.Peek();
                tokenizer.Advance();
                // Strip quotes
                return PFB.MakeVar(token.Substring(1, token.Length - 2));
            }

            if (tokenizer.PeekIsMatchAdvance("("))
            {
                AbstractFirstOrderFormula expression = Expression(tokenizer);
                if (tokenizer.PeekIsMatchAdvance(")"))
                {
                    return PFB.MakeBrackets(expression);
                }
                throw new SafetyPropertyParserException(tokenizer.Mark() + 1, "Expecting ')' after expression");
            }

            throw new SafetyPropertyParserException(tokenizer.Mark(), $"Unexpected symbol {tokenizer.Peek()[0]}");
        }
    }
}


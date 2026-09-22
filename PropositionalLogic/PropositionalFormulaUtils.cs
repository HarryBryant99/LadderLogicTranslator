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
// Description:  This file defines the PropositionalFormulaUtils class, which recursively traverses a propositional logic formula and returns 
// the set of all variables (predicates) appearing within it.
//
// ------------------
//
// GPL-3.0 license
//

//using SwanLLVerifier.ETCSDC_Properties;
//using SwanLLVerifier.ETCSDC_Properties.Operators;
//using SwanLLVerifier.ETCSDC_Properties.OperatorTypes;

using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;

namespace SwanLLVerifier.PropositionalLogic
{
    public static class PropositionalFormulaUtils
    {
        public static ISet<string> AllVariablesFromFormula(AbstractFirstOrderFormula formulae)
        {
            HashSet<string> allVariables = new();

            if (typeof(BinaryOperatorType).IsInstanceOfType(formulae))
            {
                BinaryOperatorType b = (BinaryOperatorType)formulae;
                allVariables.UnionWith(AllVariablesFromFormula(b.LeftOperand));
                allVariables.UnionWith(AllVariablesFromFormula(b.RightOperand));
            }
            else if (typeof(Negation).IsInstanceOfType(formulae))
            {
                Negation n = (Negation)formulae;
                allVariables.UnionWith(AllVariablesFromFormula(n.Operand));
            }
            else if (typeof(Predicate).IsInstanceOfType(formulae))
            {
                Predicate p = (Predicate)formulae;
                _ = allVariables.Add(p.Name);
            }
            else if (typeof(Brackets).IsInstanceOfType(formulae))
            {
                Brackets b = (Brackets)formulae;
                allVariables.UnionWith(AllVariablesFromFormula(b.Operand));
            }
            else
            {
                throw new ArgumentException("Unsupported AbstractFirstOrderFormulae SubClass: " + formulae.GetType());
            }

            return allVariables;
        }

    }
}

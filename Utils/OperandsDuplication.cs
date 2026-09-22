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
// Description:  This file defines the OperandsDuplication utility class, which creates deep copies of propositional logic formula trees so that 
// formulas can be transformed and manipulated without modifying the original structures.
//
// ------------------
//
// GPL-3.0 license
//

using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;

using static SwanLLVerifier.PropositionalLogic.PropositionalFormulaBuilder;

namespace SwanLLVerifier.Utils
{
    public static class OperandsDuplication
    {
        public static AbstractFirstOrderFormula Duplicate(AbstractFirstOrderFormula formula)
        {
            return formula switch
            {
                Predicate pred => MakeVar(pred.Name),
                Equivalent eq => MakeEquivalence(Duplicate(eq.LeftOperand), Duplicate(eq.RightOperand)),
                Implies imp => MakeImplication(Duplicate(imp.LeftOperand), Duplicate(imp.RightOperand)),
                Or or => MakeOr(Duplicate(or.LeftOperand), Duplicate(or.RightOperand)),
                And and => MakeAnd(Duplicate(and.LeftOperand), Duplicate(and.RightOperand)),
                Negation neg => MakeNegation(neg.Operand),
                Brackets brackets => MakeBrackets(brackets.Operand),
                _ => throw new ArgumentException($"Invalid formula type: {formula.FormulaType}")
            };
        }
    }
}

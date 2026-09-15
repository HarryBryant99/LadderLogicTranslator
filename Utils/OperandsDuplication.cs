//using SwanLLVerifier.ETCSDC_Properties;
//using SwanLLVerifier.ETCSDC_Properties.Operators;
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

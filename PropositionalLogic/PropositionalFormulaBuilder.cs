//using SwanLLVerifier.ETCSDC_Properties;
//using static SwanLLVerifier.ETCSDC_Properties.AbstractFirstOrderFormula;
//sing SwanLLVerifier.ETCSDC_Properties.OperatorTypes;
//using SwanLLVerifier.ETCSDC_Properties.Operators;

using Siemens.ETCSDC;
using static Siemens.ETCSDC.Properties.AbstractFirstOrderFormula;
using Siemens.ETCSDC.Properties;

namespace SwanLLVerifier.PropositionalLogic
{
    public class PropositionalFormulaBuilder
    {
        public PropositionalFormulaBuilder() { }

        public static BinaryOperatorType MakeAnd(AbstractFirstOrderFormula leftOperand, AbstractFirstOrderFormula rightOperand)
        {
            BinaryOperatorType f = new And
            {
                Operands = new AbstractFirstOrderFormula[2],
                FormulaType = FOLFormulaType.And,
                LeftOperand = leftOperand,
                RightOperand = rightOperand
            };
            return f;
        }

        public static UnaryOperatorType MakeAndWithBrackets(AbstractFirstOrderFormula leftOperand, AbstractFirstOrderFormula rightOperand)
        {
            return MakeBrackets(MakeAnd(leftOperand, rightOperand));
        }

        public static AbstractFirstOrderFormula MakeAnd(List<AbstractFirstOrderFormula> operands)
        {
            // Allow graceful dealing with Ands of singular item.
            if (operands.Count == 1)
            {
                return operands[0];
            }

            return FoldR1BinaryOperator(MakeAnd, operands);
        }

        public static BinaryOperatorType MakeOr(AbstractFirstOrderFormula leftOperand, AbstractFirstOrderFormula rightOperand)
        {
            BinaryOperatorType f = new Or
            {
                Operands = new AbstractFirstOrderFormula[2],
                FormulaType = FOLFormulaType.Or,
                LeftOperand = leftOperand,
                RightOperand = rightOperand
            };
            return f;
        }

        public static UnaryOperatorType MakeOrWithBrackets(AbstractFirstOrderFormula leftOperand, AbstractFirstOrderFormula rightOperand)
        {
            return MakeBrackets(MakeOr(leftOperand, rightOperand));
        }

        public static AbstractFirstOrderFormula MakeOr(List<AbstractFirstOrderFormula> operands)
        {
            // Allow graceful dealing with Ors of singular item.
            if (operands.Count == 1)
            {
                return operands[0];
            }

            return FoldR1BinaryOperator(MakeOr, operands);
        }

        /// <summary>
        /// Apply a binary operator make function in a fold right style to a list of operands to produce a tree of binary operators produced form the make function.
        /// For example, if f is the make function and the operands are a, b, c, d then the result is f(a, f(b ,f(c, d))).
        /// This is almost equivalent to (BinaryOperatorType)operands.AsEnumerable().Reverse().Aggregate(makeFn);
        /// </summary>
        /// <param name="makeFn"> The binary make function to be used</param>
        /// <param name="operands"> The operands (at leats 2) to be used</param>
        public static BinaryOperatorType FoldR1BinaryOperator(Func<AbstractFirstOrderFormula, AbstractFirstOrderFormula, BinaryOperatorType> makeFn, List<AbstractFirstOrderFormula> operands)
        {
            int n = operands.Count;

            if (n < 2)
            {
                throw new ArgumentException("Must have at least 2 operands, but has only " + n);

            }

            BinaryOperatorType current = makeFn(operands[n - 2], operands[n - 1]);

            for (int i = n - 3; i >= 0; i--)
            {
                current = makeFn(operands[i], current);
            }

            return current;
        }

        public static AbstractFirstOrderFormula MakeVar(string name)
        {
            Predicate p = new()
            {
                Name = name,
                FormulaType = FOLFormulaType.Predicate
            };
            return p;
        }

        public static AbstractFirstOrderFormula MakeNegatedVar(string name)
        {
            return MakeNegation(MakeVar(name));
        }

        public static Negation MakeNegation(AbstractFirstOrderFormula operand)
        {
            // ======= commenting original code ==========
            //Negation n = new Negation();
            //n.Operand = operand;
            //n.FormulaType = FOLFormulaType.Negation;
            //return n;
            // ===========================================

            Negation n = new()
            {
                Operand = operand,
                FormulaType = FOLFormulaType.Negation
            };
            // add brackets if operand is not a Predicate type and not already a Bracket type, for better readability.
            if ((n.OperandType != FOLFormulaType.Predicate) && (n.OperandType != FOLFormulaType.Brackets))
            {
                Brackets b = new()
                {
                    FormulaType = FOLFormulaType.Brackets,
                    Operand = operand
                };
                n.Operand = b;
            }
            return n;
        }

        public static UnaryOperatorType MakeImplication(AbstractFirstOrderFormula leftOperand, AbstractFirstOrderFormula rightOperand)
        {
            BinaryOperatorType f = new Implies
            {
                Operands = new AbstractFirstOrderFormula[2],
                FormulaType = FOLFormulaType.Implies,
                LeftOperand = leftOperand,
                RightOperand = rightOperand
            };
            return MakeBrackets(f);
        }

        public static UnaryOperatorType MakeEquivalence(AbstractFirstOrderFormula leftOperand, AbstractFirstOrderFormula rightOperand)
        {
            BinaryOperatorType f = new Equivalent
            {
                Operands = new AbstractFirstOrderFormula[2],
                FormulaType = FOLFormulaType.Equivalent,
                LeftOperand = leftOperand,
                RightOperand = rightOperand
            };
            return MakeBrackets(f);
        }

        public static Brackets MakeBrackets(AbstractFirstOrderFormula operand)
        {
            Brackets b = new()
            {
                Operand = operand,
                FormulaType = FOLFormulaType.Brackets,

            };
            
            return b;
        }
    }
}

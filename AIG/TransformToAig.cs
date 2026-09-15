using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;
using static SwanLLVerifier.PropositionalLogic.PropositionalFormulaBuilder;
using static SwanLLVerifier.Utils.OperandsDuplication;
//using SwanLLVerifier.ETCSDC_Properties.OperatorTypes;
using SwanLLVerifier.LadderLogic;
using SwanLLVerifier.Utils;

namespace SwanLLVerifier.AIG
{
    public static class TransformToAig
    {
        public static AbstractFirstOrderFormula TransformAllEquivalences(AbstractFirstOrderFormula formula)
        {
            return formula switch
            {
                Predicate p => p,
                UnaryOperatorType unForm => UnaryTransformEquivalence(unForm),
                BinaryOperatorType binForm => BinaryTransformEquivalence(binForm),
                _ => throw new Exception("Unhandled formula type.")
            };

            static AbstractFirstOrderFormula UnaryTransformEquivalence(UnaryOperatorType unForm)
            {
                // creating a duplicate of unForm itself
                UnaryOperatorType dupUnForm = (UnaryOperatorType)Duplicate(unForm);

                // duplicating the operand of the original unForm
                AbstractFirstOrderFormula dupOperand = Duplicate(unForm.Operand);

                // transforming equivalences of the operand
                dupUnForm.Operand = TransformAllEquivalences(dupOperand);

                return dupUnForm;
            }

            static AbstractFirstOrderFormula BinaryTransformEquivalence(BinaryOperatorType binForm)
            {
                // creating a duplicate of binForm
                BinaryOperatorType dupBinForm = (BinaryOperatorType)Duplicate(binForm);

                // duplicating the operands of original binForm
                AbstractFirstOrderFormula dupLeft = Duplicate(binForm.LeftOperand);
                AbstractFirstOrderFormula dupRight = Duplicate(binForm.RightOperand);

                // transforming the equivalences of the operands
                dupBinForm.LeftOperand = TransformAllEquivalences(dupLeft);
                dupBinForm.RightOperand = TransformAllEquivalences(dupRight);

                if (dupBinForm.FormulaType == AbstractFirstOrderFormula.FOLFormulaType.Equivalent)
                {
                    // a <-> b is logically equivalent to ((a -> b) & (b -> a))
                    AbstractFirstOrderFormula leftCopy = Duplicate(dupBinForm.LeftOperand);
                    AbstractFirstOrderFormula rightCopy = Duplicate(dupBinForm.RightOperand);
                    AbstractFirstOrderFormula leftImp = MakeImplication(dupBinForm.LeftOperand, dupBinForm.RightOperand);
                    AbstractFirstOrderFormula rightImp = MakeImplication(rightCopy, leftCopy);

                    return MakeAnd(leftImp, rightImp);
                }

                return dupBinForm;
            }
        }

        public static AbstractFirstOrderFormula TransformAllImplications(AbstractFirstOrderFormula formula)
        {
            return formula switch
            {
                Predicate p => p,
                UnaryOperatorType unForm => UnaryTransformImplication(unForm),
                BinaryOperatorType binForm => BinaryTransformImplication(binForm),
                _ => throw new Exception("Unhandled formula type.")
            };

            static AbstractFirstOrderFormula UnaryTransformImplication(UnaryOperatorType unForm)
            {
                // creating a duplicate of unForm itself
                UnaryOperatorType dupUnForm = (UnaryOperatorType)Duplicate(unForm);

                // duplicating the operand of the original unForm
                AbstractFirstOrderFormula dupOperand = Duplicate(unForm.Operand);

                // transforming equivalences of the operand
                dupUnForm.Operand = TransformAllImplications(dupOperand);

                return dupUnForm;
            }

            static AbstractFirstOrderFormula BinaryTransformImplication(BinaryOperatorType binForm)
            {
                BinaryOperatorType dupBinForm = (BinaryOperatorType)Duplicate(binForm);

                AbstractFirstOrderFormula left = Duplicate(binForm.LeftOperand);
                AbstractFirstOrderFormula right = Duplicate(binForm.RightOperand);

                dupBinForm.LeftOperand = TransformAllImplications(left);
                dupBinForm.RightOperand = TransformAllImplications(right);

                if (dupBinForm.FormulaType == AbstractFirstOrderFormula.FOLFormulaType.Implies)
                    // a -> b is logically equivalent to (!a v b)
                    return MakeOr(MakeNegation(dupBinForm.LeftOperand), dupBinForm.RightOperand);

                return dupBinForm;
            }
        }

        public static AbstractFirstOrderFormula TransformAllOrs(AbstractFirstOrderFormula formula)
        {
            return formula switch
            {
                Predicate p => p,
                UnaryOperatorType unForm => UnaryTransformOr(unForm),
                BinaryOperatorType binForm => BinaryTransformOr(binForm),
                _ => throw new Exception("Unhandled formula type.")
            };

            static AbstractFirstOrderFormula UnaryTransformOr(UnaryOperatorType unForm)
            {
                // creating a duplicate of unForm itself
                UnaryOperatorType dupUnForm = (UnaryOperatorType)Duplicate(unForm);

                // duplicating the operand of the original unForm
                AbstractFirstOrderFormula dupOperand = Duplicate(unForm.Operand);

                // transforming equivalences of the operand
                dupUnForm.Operand = TransformAllOrs(dupOperand);

                return dupUnForm;
            }

            static AbstractFirstOrderFormula BinaryTransformOr(BinaryOperatorType binForm)
            {
                BinaryOperatorType dupBinForm = (BinaryOperatorType)Duplicate(binForm);

                AbstractFirstOrderFormula left = Duplicate(binForm.LeftOperand);
                AbstractFirstOrderFormula right = Duplicate(binForm.RightOperand);

                dupBinForm.LeftOperand = TransformAllOrs(left);
                dupBinForm.RightOperand = TransformAllOrs(right);

                if (dupBinForm.FormulaType == AbstractFirstOrderFormula.FOLFormulaType.Or)
                    // a V b is logically equivalent to !(!a /\ !b)
                    return MakeNegation(MakeAnd(MakeNegation(dupBinForm.LeftOperand), MakeNegation(dupBinForm.RightOperand)));

                return dupBinForm;
            }
        }
        //(("P1947.RUK_0") & (~("P1947.RUK_1"))) -> ((~("P1946A.NUK_1" | "P1946B.NUK_1")) | (~("P1946A.RUK_1" | "P1946B.RUK_1")))
        // LochNess810_SubsequentRouteSectionRelease_group17.cond
        public static AbstractFirstOrderFormula CheckAndRemoveDoubleBrackets(AbstractFirstOrderFormula formula)
        {
            // at this point expect the parameter to be in all AIG format
            // so we just need to clean up double brackets followed by double negations in the next step
            return formula switch
            {
                Predicate p => p,
                UnaryOperatorType unForm => UnaryTransformDoubleBrackets(unForm),
                BinaryOperatorType binForm => BinaryTransformDoubleBrackets(binForm),
                _ => formula
            };

            static AbstractFirstOrderFormula UnaryTransformDoubleBrackets(UnaryOperatorType unForm)
            {
                UnaryOperatorType dupUnForm = (UnaryOperatorType)Duplicate(unForm);

                if (dupUnForm.FormulaType == AbstractFirstOrderFormula.FOLFormulaType.Brackets) // first bracket
                {
                    if (dupUnForm.Operand.FormulaType == AbstractFirstOrderFormula.FOLFormulaType.Predicate)
                        return Duplicate(dupUnForm.Operand); // is duplication necessary here?
                    else if (dupUnForm.Operand.FormulaType == AbstractFirstOrderFormula.FOLFormulaType.Brackets) // second bracket
                        return CheckAndRemoveDoubleBrackets(((Brackets)Duplicate(dupUnForm.Operand)).Operand); // return  CheckAndRemoveDoubleBrackets(operand_of_second_bracket)
                }

                // do nothing (preserve the formula) if unForm is not of type Brackets.
                return dupUnForm;
            }

            static AbstractFirstOrderFormula BinaryTransformDoubleBrackets(BinaryOperatorType binForm)
            {
                BinaryOperatorType dupBinForm = (BinaryOperatorType)Duplicate(binForm);

                dupBinForm.LeftOperand = CheckAndRemoveDoubleBrackets(binForm.LeftOperand);
                dupBinForm.RightOperand = CheckAndRemoveDoubleBrackets(binForm.RightOperand);

                return dupBinForm;
            }
        }

        public static AbstractFirstOrderFormula CheckAndRemoveDoubleNegations(AbstractFirstOrderFormula formula)
        {
            // expect the parameter to be in all AIG format
            return formula switch
            {
                Predicate p => p,
                UnaryOperatorType unForm => UnaryRemoveDoubleNegations(unForm),
                BinaryOperatorType binForm => BinaryRemoveDoubleNegations(binForm),
                _ => formula
            };

            static AbstractFirstOrderFormula UnaryRemoveDoubleNegations(UnaryOperatorType unForm)
            {
                UnaryOperatorType dupUnForm = (UnaryOperatorType)Duplicate(unForm);

                if (dupUnForm.FormulaType == AbstractFirstOrderFormula.FOLFormulaType.Negation) // first negation
                {
                    if (dupUnForm.Operand.FormulaType == AbstractFirstOrderFormula.FOLFormulaType.Negation) // second negation inside negation e.g. !!x
                        return CheckAndRemoveDoubleNegations(((Negation)Duplicate(dupUnForm.Operand)).Operand);

                    if (dupUnForm.Operand.FormulaType == AbstractFirstOrderFormula.FOLFormulaType.Brackets)
                    {
                        if (((Brackets)dupUnForm.Operand).Operand.FormulaType == AbstractFirstOrderFormula.FOLFormulaType.Negation) // e.g. !(!(x or y))
                        {
                            AbstractFirstOrderFormula formAfterDoubleNegRemoval = ((Negation)((Brackets)dupUnForm.Operand).Operand).Operand;
                            return CheckAndRemoveDoubleNegations(formAfterDoubleNegRemoval);

                        }
                        else // e.g. !(x or y)
                            return MakeNegation(CheckAndRemoveDoubleNegations(((Brackets)Duplicate(dupUnForm.Operand)).Operand));
                    }
                }

                if (dupUnForm.FormulaType == AbstractFirstOrderFormula.FOLFormulaType.Brackets)
                    return CheckAndRemoveDoubleNegations(dupUnForm.Operand);

                // do nothing (preserve the formula) if unForm is not of type Negation.
                return dupUnForm;
            }

            static AbstractFirstOrderFormula BinaryRemoveDoubleNegations(BinaryOperatorType binForm)
            {
                BinaryOperatorType dupBinForm = (BinaryOperatorType)Duplicate(binForm);

                dupBinForm.LeftOperand = CheckAndRemoveDoubleNegations(Duplicate(binForm.LeftOperand));
                dupBinForm.RightOperand = CheckAndRemoveDoubleNegations(Duplicate(binForm.RightOperand));

                return dupBinForm;
            }
        }

        public static AbstractFirstOrderFormula Transform(AbstractFirstOrderFormula formula)
        {
            // Console.WriteLine("============== Original formula ==============");
            // PrettyPrinter.PrettyPrint(formula);

            AbstractFirstOrderFormula eqTransformed1 = TransformAllEquivalences(formula);
            // Console.WriteLine("\n============== After removing all EQUIVALENCEs ==============");
            // Symbol of equivalence is <-> or <>
            // PrettyPrinter.PrettyPrint(eqTransformed1);

            AbstractFirstOrderFormula eqTransformed2 = TransformAllImplications(eqTransformed1);
            // Console.WriteLine("\n============== After removing all IMPLICATIONs ==============");
            // Symbol of implication is -> or =>
            // PrettyPrinter.PrettyPrint(eqTransformed2);

            AbstractFirstOrderFormula eqTransformed3 = TransformAllOrs(eqTransformed2);
            // Console.WriteLine("\n============== After removing all ors ==============");
            // PrettyPrinter.PrettyPrint(eqTransformed3);

            // AbstractFirstOrderFormula eqTransformed4 = CheckAndRemoveDoubleBrackets(eqTransformed3);
            // // Console.WriteLine("\n============== After removing all DOUBLE BRACKETs ==============");
            // // PrettyPrinter.PrettyPrint(eqTransformed4);

            // AbstractFirstOrderFormula eqTransformed5 = CheckAndRemoveDoubleNegations(eqTransformed4);
            // // Console.WriteLine("\n============== After removing all DOUBLE NEGATIONs [FINAL TRANSFORMATION STEP] ==============");
            // // PrettyPrinter.PrettyPrint(eqTransformed5);

            // Cancel out double negations
            // e.g !(! x) -> x
            // e.g !!x -> x
            // AbstractFirstOrderFormula eqTransformed4 = CheckAndRemoveDoubleBrackets(eqTransformed3);
            // Console.WriteLine("\n============== After removing all DOUBLE BRACKETs ==============");
            // PrettyPrinter.PrettyPrint(eqTransformed4);

            return eqTransformed3;
        }

        public static Ladder TransformLadder(Ladder ladder)
        {
            Ladder transformedLadder = new();
            foreach (Rung rung in ladder.Rungs)
            {
                // Transform the formula of the rung
                AbstractFirstOrderFormula transformedFormula = Transform(rung.formula);
                // Console.WriteLine("\nTransformed Formula: " + PrettyPrinter.Prettify(transformedFormula));
                Rung tempRung = new()
                {
                    formula = transformedFormula,
                    output = rung.output,
                    Initialised = rung.Initialised, // Preserve the initialised state
                };
                transformedLadder.AddRung(tempRung);
            }
            return transformedLadder;
        }

    }
}

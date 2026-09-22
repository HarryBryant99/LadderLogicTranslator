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
// Description:  This file defines the PrettyPrinter utility class, which converts propositional logic formulas and ladder logic rungs into 
// human-readable text for printing, debugging, and inspection.
//
// ------------------
//
// GPL-3.0 license
//

using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;
using SwanLLVerifier.LadderLogic;

namespace SwanLLVerifier.Utils
{
    public static class PrettyPrinter
    {
        public static string Prettify(AbstractFirstOrderFormula formula)
        {
            return formula switch
            {
                null => string.Empty,
                Predicate predicate => predicate.Name,
                BinaryOperatorType binaryOperator => BinaryOperatorToString(binaryOperator),
                UnaryOperatorType unaryOperator => UnaryOperatorToString(unaryOperator),
                _ => "Not Implemented.",
            };

            static string BinaryOperatorToString(BinaryOperatorType op)
            {
                return op switch
                {
                    Equivalent => Prettify(op.LeftOperand) + " <=> " + Prettify(op.RightOperand),
                    Implies => Prettify(op.LeftOperand) + " => " + Prettify(op.RightOperand),
                    Or => Prettify(op.LeftOperand) + " V " + Prettify(op.RightOperand),
                    And => Prettify(op.LeftOperand) + " /\\ " + Prettify(op.RightOperand),
                    _ => throw new ArgumentException($"Invalid Operator Type: {op}"),
                };
            }

            static string UnaryOperatorToString(UnaryOperatorType op)
            {
                return op switch
                {
                    Negation => "!" + Prettify(op.Operand),
                    Brackets => "(" + Prettify(op.Operand) + ")",
                    _ => throw new ArgumentException($"Invalid Operator Type: {op}"),
                };
            }
        }

        public static void PrettyPrint(AbstractFirstOrderFormula formula)
        {
            Console.WriteLine(Prettify(formula));
        }

        public static string CaptureConsoleOutput(Action action)
        {
            // This method deterministically captures console side effects for later inspection.
            TextWriter originalOut = Console.Out;
            using StringWriter buffer = new();
            Console.SetOut(buffer);

            try
            {
                action();
                Console.Out.Flush();
            }
            finally
            {
                // This restoration re-establishes the global console state to prevent leakage.
                Console.SetOut(originalOut);
            }

            // This materializes the captured trace as a pure string value.
            return buffer.ToString();
        }

        public static void PrettyPrintRung(Rung rung)
        {
            Console.WriteLine("Left Hand Side");
            Console.WriteLine(rung.output);

            Console.WriteLine("Right Hand Side");
            PrettyPrint(rung.formula);
        }

        public static void PrettyPrintWithDelay(
            AbstractFirstOrderFormula formula,
            int delayInMilliseconds = 1000)
        {
            var task = Task.Run(() => PrettyPrint(formula));

            if (!task.Wait(delayInMilliseconds))
            {
                Console.WriteLine("Pretty printing took too long.");
            }
        }

        // pretify using threads
        //public static void PrettyPrintWithDelay(
        //    AbstractFirstOrderFormula formula,
        //    int delayInMilliseconds = 1000
        //)
        //{
        //    Thread thread = new Thread(
        //        () =>
        //        {
        //            PrettyPrint(formula);
        //        },
        //        16 * 1024 * 1024
        //    ); // 16 MB stack size
        //    thread.Start();
        //    thread.Join(delayInMilliseconds);
        //    if (thread.IsAlive)
        //    {
        //        thread.Abort();
        //        Console.WriteLine("Pretty printing took too long and was aborted.");
                // // Optionally, you could log the formula that was being printed
                // Console.WriteLine("Formula: " + Prettify(formula));
        //    }
        //}
    }
}

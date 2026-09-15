using System.Text;
using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;
using SwanLLVerifier.LadderLogic;

namespace SwanLLVerifier.SMTLib
{
    public class SMTLibUtil
    {
        public static void ToSMTLibInductive(Ladder ladder, AbstractFirstOrderFormula safetyCondition, string filenameBasename)
        {
            ISet<string> allVariables = ladder.AllVariables();

            // get any variables from safety property, eg for initial states
            foreach (string variable in safetyCondition.GetAllVariables())
            {
                string baseVar = BaseVariableName(variable);

                allVariables.Add($"{baseVar}_0");
                allVariables.Add($"{baseVar}_1");
            }

            string FileName = filenameBasename.Split("/").Last();

            FileStream streamBase = new(filenameBasename + "_base.smt", FileMode.Create);
            using (StreamWriter writerBase = new(streamBase))
            {
                OutputHeader(writerBase);
                OutputProofLog(writerBase, (FileName + "_base"));
                OutputCreateVars(writerBase, allVariables, 0);
                //OutputCreateVars(writerBase, allVariables, 1);
                OutputInitVars(writerBase, allVariables, 0);
                OutputLadder(writerBase, allVariables, ladder, 1);
                OutputSafetyCondition(writerBase, safetyCondition, 1, true);
                OutputFooter(writerBase);
            }

            FileStream streamStep = new(filenameBasename + "_step.smt", FileMode.Create);
            using StreamWriter writerStep = new(streamStep);
            OutputHeader(writerStep);
            OutputProofLog(writerStep, (FileName + "_step"));
            OutputCreateVars(writerStep, allVariables, 0);
            //OutputCreateVars(writerStep, allVariables, 1);
            //OutputCreateVars(writerStep, allVariables, 2);
            OutputLadder(writerStep, allVariables, ladder, 1);
            //OutputLadder(writerStep, allVariables, ladder, 2);
            OutputSafetyCondition(writerStep, safetyCondition, 1, false);
            OutputSafetyCondition(writerStep, safetyCondition, 2, true);
            OutputFooter(writerStep);

            FileStream streamInvBase = new(filenameBasename + "_inv_base.smt", FileMode.Create);
            using (StreamWriter writerInvBase = new(streamInvBase))
            {
                OutputHeader(writerInvBase);
                OutputProofLog(writerInvBase, (FileName + "_inv_base"));
                OutputCreateVars(writerInvBase, allVariables, 0);
                OutputInitVars(writerInvBase, allVariables, 0);
                OutputFooter(writerInvBase);
            }

            FileStream streamInvStep = new(filenameBasename + "_inv_step.smt", FileMode.Create);
            using StreamWriter writerInvStep = new(streamInvStep);
            OutputHeader(writerInvStep);
            OutputProofLog(writerInvStep, (FileName + "_inv_step"));
            OutputCreateVars(writerInvStep, allVariables, 0);
            OutputLadder(writerInvStep, allVariables, ladder, 1);
            OutputFooter(writerInvStep);
        }

        // Return the variable without any priming or non priming
        private static string BaseVariableName(string variable)
        {
            if (variable.EndsWith("_0") || variable.EndsWith("_1"))
            {
                return variable[..^2];
            }

            return variable;
        }

        public static void ToSMTLibBoundedModelChecking(Ladder ladder, AbstractFirstOrderFormula safetyCondition, int kSteps, string filenameBasename)
        {
            ISet<string> allVariables = ladder.AllVariables();

            FileStream stream = new(filenameBasename + "_bmc.smt", FileMode.Create);
            using StreamWriter writer = new(stream);
            OutputHeader(writer);
            for (int i = 0; i <= kSteps; i++) OutputCreateVars(writer, allVariables, i);
            OutputInitVars(writer, allVariables, 0);
            for (int i = 1; i <= kSteps; i++) OutputLadder(writer, allVariables, ladder, i);
            for (int i = 1; i <= kSteps; i++) OutputSafetyCondition(writer, safetyCondition, i, true);
            OutputFooter(writer);
        }

        private static void OutputLadder(StreamWriter writer, ISet<string> allVariables, Ladder ladder, int targetVersion)
        {
            // Dictionary to keep track of seen variables (i.e., coils).
            ISet<string> seenVariables = new HashSet<string>();

            // Output asserts for all rungs
            foreach (Rung rung in ladder.Rungs)
            {
                string formula = FormulaToSMTLib(rung.formula, (varName) => {
                    int version = seenVariables.Contains(varName) ? targetVersion : targetVersion - 1;
                    //return NameToSMTLib(NameToVersionedName(varName, version));
                    return NameToSMTLib(varName);
                }).ToString();
                _ = seenVariables.Add(rung.output);
                //string output = NameToSMTLib(NameToVersionedName(rung.output, targetVersion));
                string output = NameToSMTLib(rung.output);

                writer.WriteLine($"(assert(= {output} {formula}))");
            }
        }

        private static void OutputHeader(StreamWriter writer)
        {
            writer.WriteLine("(set-option :print-success false)");
            writer.WriteLine("(set-logic QF_UF)");
        }


        //Proof Log Header
        private static void OutputProofLog(StreamWriter writer, string filename)
        {
            writer.WriteLine("(set-option :sat.euf true)");
            writer.WriteLine("(set-option :tactic.default_tactic smt)");
            writer.WriteLine("(set-option :solver.proof.log " + filename + ".smt2)");
        }

        private static void OutputCreateVars(StreamWriter writer, ISet<string> allVariables, int version)
        {
            foreach (string variable in allVariables)
            {
                //writer.WriteLine($"(declare-const {NameToSMTLib(NameToVersionedName(variable, version))} Bool)");
                writer.WriteLine($"(declare-const {NameToSMTLib(variable)} Bool)");
            }
        }

        private static void OutputInitVars(StreamWriter writer, ISet<string> allVariables, int version)
        {
            foreach (string variable in allVariables)
            {
                //writer.WriteLine($"(assert(= {NameToSMTLib(NameToVersionedName(variable, version))} false))");
                writer.WriteLine($"(assert(= {NameToSMTLib(variable)} false))");
            }
        }

        private static void OutputSafetyCondition(StreamWriter writer, AbstractFirstOrderFormula formulae, int version, bool isCheck)
        {
            string formula = FormulaToSMTLib(formulae, (varName) => {
                if (varName.EndsWith("_0"))
                {
                    return NameToSMTLib(varName.Substring(0, varName.Length - 1) + (version - 1));
                }
                else if (varName.EndsWith("_1"))
                {
                    return NameToSMTLib(varName.Substring(0, varName.Length - 1) + (version));
                }
                else
                {
                    throw new Exception($"Safety property variables should have a _0 or _1 version number at the end, but {varName} does not.");
                }
            }).ToString();

            if (isCheck)
            {
                writer.WriteLine($"(assert {formula})");
            }
            else
            {
                writer.WriteLine($"(assert {formula})");
            }
        }

        private static void OutputFooter(StreamWriter writer)
        {
            writer.WriteLine("(check-sat)");
            writer.WriteLine("(exit)");
        }

        private static string NameToSMTLib(string name)
        {
            return name.Replace("(", "_op_").Replace(")", "_cp_").Replace(".", "_dot_").Replace(" ", "_sp_");
        }

        private static string NameToVersionedName(string name, int version)
        {
            return name + "_" + version;
        }

        public static StringBuilder FormulaToSMTLib(AbstractFirstOrderFormula formulae, Func<string, string> VariableNameTransformer)
        {
            var sb = new System.Text.StringBuilder();

            if (typeof(BinaryOperatorType).IsInstanceOfType(formulae))
            {
                BinaryOperatorType b = (BinaryOperatorType)formulae;

                string? opString = null;
                if (typeof(And).IsInstanceOfType(b))
                {
                    opString = "and";
                }
                else if (typeof(Or).IsInstanceOfType(b))
                {
                    opString = "or";
                }
                else if (typeof(Implies).IsInstanceOfType(b))
                {
                    opString = "=>";
                }
                else
                {
                    throw new ArgumentException("Unsupported AbstractFirstOrderFormulae SubClass: " + b.GetType());
                }

                StringBuilder left = FormulaToSMTLib(b.LeftOperand, VariableNameTransformer);
                StringBuilder right = FormulaToSMTLib(b.RightOperand, VariableNameTransformer);
                _ = sb.Append('(').Append(opString).Append(' ').Append(left).Append(' ').Append(right).Append(')');
            }
            else if (typeof(Negation).IsInstanceOfType(formulae))
            {
                Negation n = (Negation)formulae;
                StringBuilder operand = FormulaToSMTLib(n.Operand, VariableNameTransformer);
                _ = sb.Append("(not ").Append(operand).Append(')');
            }
            else if (typeof(Predicate).IsInstanceOfType(formulae))
            {
                Predicate p = (Predicate)formulae;
                string name = VariableNameTransformer(p.Name);
                _ = sb.Append(name);
            }
            else if (typeof(Brackets).IsInstanceOfType(formulae))
            {
                Brackets b = (Brackets)formulae;
                StringBuilder operand = FormulaToSMTLib(b.Operand, VariableNameTransformer);
                _ = sb.Append(operand);
            }
            else
            {
                throw new ArgumentException("Unsupported AbstractFirstOrderFormulae SubClass: " + formulae.GetType());
            }

            return sb;
        }
    }
}
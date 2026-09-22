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
// Description:  This file defines the InductiveVerificationChecker class, which automates the loading of ladder logic models and safety properties, 
// transforms them into a verification-friendly form, and prepares them for inductive verification experiments across collections of 
// signalling case studies.
//
// ------------------
//
// GPL-3.0 license
//

using SwanLLVerifier.AIG;
//using SwanLLVerifier.ETCSDC_Properties;
using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;
using SwanLLVerifier.LadderLogic;
using SwanLLVerifier.SafetyProperty;
using System.Text;
using System.Xml;

namespace SwanLLVerifier.Utils
{
    public class InductiveVerificationChecker
    {
        public InductiveVerificationChecker() { }

        public void RunInductiveVerificationTest()
        {
            string sourceRootPath = @"wetransfer_aigerfier-and-ladder-logic\SiemensData";

            string lochnessSafetyDirPath = Path.Combine(sourceRootPath, "Additional_LochNess_Properties");
            string lochness810XmlFilePath = Path.Combine(sourceRootPath, @"LochNess-810", "810.xml");
            string lochness822XmlFilePath = Path.Combine(sourceRootPath, @"LochNess-822", "822.xml");

            string mostynSafetyDirPath = Path.Combine(sourceRootPath, "Additional_Mostyn_Properties");
            string mostynXmlFilePath = Path.Combine(sourceRootPath, @"Mostyn_946_Data", "946.xml");

            foreach (string safetyPropParentDir in Directory.GetDirectories(lochnessSafetyDirPath))
            {
                // ========== before your loop
                var csv = new StringBuilder();

                string[] fileEntries = Directory.GetFiles(safetyPropParentDir);
                int feCount = fileEntries.Length;

                for (int i = 0; i < fileEntries.Length; i++)
                {
                    // the safety condition is always on the 5th line of the .cond file
                    string safetyPropertyString = File.ReadLines(fileEntries[i]).Skip(4).Take(1).First();

                    AbstractFirstOrderFormula safetyCondition = SafetyPropertyParser.Parse(safetyPropertyString);
                    AbstractFirstOrderFormula transformedSafety = TransformToAig.Transform(safetyCondition);
                    // note: safety negation is not required for IV check

                    XmlDocument doc = new();
                    string csvFirstColumn = "";
                    string condFileName = "";

                    if (safetyPropParentDir.Contains("810"))
                    {
                        doc.Load(lochness810XmlFilePath);
                        condFileName = fileEntries[i].Split("\\").Last();
                        string dirSuffix = safetyPropParentDir.Split("\\").Last().Replace("LochNess", "");
                        csvFirstColumn = $"810_additional_810_{dirSuffix}_{condFileName}";
                    }
                    else if (safetyPropParentDir.Contains("822"))
                    {
                        doc.Load(lochness822XmlFilePath);
                        condFileName = fileEntries[i].Split("\\").Last();
                        string dirSuffix = safetyPropParentDir.Split("\\").Last().Replace("LochNess", "");
                        csvFirstColumn = $"822_additional_822_{dirSuffix}_{condFileName}";
                    }
                    else
                        throw new Exception($"Invalid safety property parent directory: {safetyPropParentDir}");

                    Ladder ladder = LadderLogicXmlParser.ParseXML(doc);

                    Model model = new(ladder);
                    model.InitialiseModel();
                    //model.ComputeCoilValues();

                    // ========== in your loop
                    string first = csvFirstColumn;
                    //string second = (ic3Output == 1) ? "no" : "yes";
                    //var third = (watch.Elapsed.TotalSeconds);
                    //var newLine = string.Format("{0},{1},{2}", first, second, third);
                    //csv.AppendLine(newLine);
                }

                // ========== after your loop
                //File.AppendAllText(outputFileName, csv.ToString());
            }
        }
    }
}

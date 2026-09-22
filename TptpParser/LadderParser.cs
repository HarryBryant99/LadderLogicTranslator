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
// Description:  This file defines the LadderParser class, which reads a ladder logic program expressed in TPTP format and converts each 
// equivalence statement into an internal Ladder representation consisting of rungs with outputs and Boolean formulas.
//
// ------------------
//
// GPL-3.0 license
//

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using SwanLLVerifier.AIG;
//using SwanLLVerifier.ETCSDC_Properties.Operators;
using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;
using SwanLLVerifier.LadderLogic;
using SwanLLVerifier.Utils;

namespace SwanLLVerifier.TptpParser
{
    public class LadderParser
    {
        private const string Prefix = "fof(ax,axiom, ";
        private const string Postfix = ").";

        public static Ladder ParseLadder(Stream inputStream)
        {
            var ladder = new Ladder();

            using (var reader = new StreamReader(inputStream, Encoding.UTF8))
            {
                string line;
                while ((line = reader.ReadLine()!) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }

                    //Debug.Assert(line.StartsWith(Prefix));
                    //Debug.Assert(line.EndsWith(Postfix));

                    string lineBody = line.Substring(Prefix.Length, line.Length - Prefix.Length - Postfix.Length);
                    var parsedBody = Parser.Parse(lineBody);
                    // PrettyPrinter.PrettyPrint(parsedBody);
                    Equivalent equiv;
                    // if type of parsedBody is Bracket
                    if (parsedBody is Brackets bracket)
                    {
                        equiv = (Equivalent)bracket.Operand;
                    }
                    else
                    {
                        equiv = (Equivalent)parsedBody;
                    }
                    // Equivalent equiv = (Equivalent)parsedBody;

                    Rung rung = new Rung();
                    rung.output = ((Predicate)equiv.LeftOperand).Name;
                    // rung.output = AigConstructor.FormatVarName(((Predicate)equiv.LeftOperand).Name);

                    rung.formula = equiv.RightOperand;


                    //PrettyPrinter.PrettyPrintRung(rung);

                    ladder.AddRung(rung);
                }
            }

            return ladder;
        }
    }
}



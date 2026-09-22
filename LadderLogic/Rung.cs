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
// Description:  This file defines the Rung class, which represents a single ladder logic rung consisting of an output variable, its 
// associated Boolean formula, an optional initial value, and a method for retrieving all variables used in the rung.
//
// ------------------
//
// GPL-3.0 license
//

using SwanLLVerifier.PropositionalLogic;
//using SwanLLVerifier.ETCSDC_Properties;
using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;

namespace SwanLLVerifier.LadderLogic
{
    public class Rung
    {
        public string output { get; set; } = string.Empty;

        public AbstractFirstOrderFormula formula { get; set; } = null!;

        public bool? Initialised { get; set; }

        public ISet<string> AllVariables()
        {
            ISet<string> allVariables = PropositionalFormulaUtils.AllVariablesFromFormula(formula);
            _ = allVariables.Add(output);

            return allVariables;
        }
    }
}

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
// Description:  This file defines the Ladder class, which stores a ladder logic program as a collection of rungs and provides methods to identify all variables, 
// outputs, and input variables used in the program.
//
// ------------------
//
// GPL-3.0 license
//

using SwanLLVerifier.AIG;

namespace SwanLLVerifier.LadderLogic
{
    public class Ladder
    {
        public List<Rung> Rungs { get; set; }

        public Ladder()
        {
            Rungs = new List<Rung>();
        }

        public void AddRung(Rung rung)
        {
            Rungs.Add(rung);
        }

        public ISet<string> AllVariables()
        {
            HashSet<string> allVariables = new();

            foreach (Rung rung in Rungs)
            {
                allVariables.UnionWith(rung.AllVariables());

                _ = allVariables.Add(rung.output);
            }


            return allVariables;
        }

     

        public ISet<string> AllOutputVariables()
        {
            HashSet<string> allOutputVariables = new();

            foreach (Rung rung in Rungs)
                _ = allOutputVariables.Add(rung.output);

            return allOutputVariables;
        }

        // all inputs should just be variables that are not outputs
        public ISet<string> AllInputs()
        {
            ISet<string> allVariables = AllVariables();
            ISet<string> allOutputVariables = AllOutputVariables();
            HashSet<string> inputVariables = new();

            HashSet<string> formattedOutputVariables = new();

            foreach (string var in allOutputVariables)
            {
                string formattedVar = AigConstructor.FormatVarName(var);

                _ = formattedOutputVariables.Add(formattedVar);
            }

            foreach (string var in allVariables)
            {
                if (!formattedOutputVariables.Contains(AigConstructor.FormatVarName(var)))
                {
                    _ = inputVariables.Add(var);
                }
            }

            return inputVariables;
        }

    }


}

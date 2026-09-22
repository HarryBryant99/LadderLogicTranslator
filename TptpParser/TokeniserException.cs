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
// Description:  This file defines the TokeniserException class, which represents a custom exception used to report the position and cause of 
// errors encountered while tokenising TPTP formulas.
//
// ------------------
//
// GPL-3.0 license
//

namespace SwanLLVerifier.TptpParser
{
    public class TokeniserException : System.Exception
    {
        private readonly int characterPos;

        public TokeniserException(int characterPos, string message) : base(message)
        {
            this.characterPos = characterPos;
        }

        public override string ToString()
        {
            return $"TokeniserException @ position {characterPos}: {base.Message}";
        }
    }
}
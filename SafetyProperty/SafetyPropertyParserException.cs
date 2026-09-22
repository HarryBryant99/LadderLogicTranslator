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
// Description:  This file defines the SafetyPropertyParserException class, which represents a custom exception used to report the location 
// and cause of errors encountered while parsing safety property expressions.
//
// ------------------
//
// GPL-3.0 license
//

namespace SwanLLVerifier.SafetyProperty
{
    public class SafetyPropertyParserException : Exception
    {
        private readonly int characterNumber;

        public SafetyPropertyParserException(int characterNumber, string message)
            : base($"Safety Property Parse Error at character {characterNumber}: {message}")
        {
            this.characterNumber = characterNumber;
        }
    }
}

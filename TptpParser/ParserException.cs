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
// Description:  This file defines the ParserException class, which represents a custom exception used to report the position and cause of errors 
// encountered while parsing TPTP formulas.
//
// ------------------
//
// GPL-3.0 license
//

namespace SwanLLVerifier.TptpParser
{
    public class ParserException : System.Exception
    {
        public int CharacterPosition { get; }

        public ParserException(int characterPosition, string message) : base(message)
        {
            CharacterPosition = characterPosition;
        }

        public override string ToString()
        {
            return $"ParserException @ position {CharacterPosition}: {Message}";
        }
    }
}


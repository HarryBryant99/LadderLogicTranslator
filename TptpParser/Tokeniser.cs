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
// Description:  This file defines the Tokeniser class, which tokenises TPTP formulas into variables, operators, and brackets, providing the 
// lexical analysis needed by the TPTP parser.
//
// ------------------
//
// GPL-3.0 license
//

using System.Text.RegularExpressions;

namespace SwanLLVerifier.TptpParser
{
    public class Tokeniser
    {
        private readonly string data;
        private int currentStart;

        public Tokeniser(string data)
        {
            this.data = data;
            currentStart = 0;
        }

        private void ConsumeWhiteSpace()
        {
            while (currentStart < data.Length)
            {
                if (char.IsWhiteSpace(data[currentStart]))
                {
                    currentStart++;
                }
                else
                {
                    return;
                }
            }
        }

        public string Peek()
        {
            ConsumeWhiteSpace();

            if (AtEnd())
            {
                throw new TokeniserException(currentStart, "Expecting token, but found end of string");
            }

            char c = data[currentStart];

            // Check for single character symbol
            if (c == '(' || c == ')' || c == '&' || c == '|' || c == '~')
            {
                return c.ToString();
            }

            // Check for multi-char symbols
            if (c == '<')
            {
                if (data.Substring(currentStart, Math.Min(3, data.Length - currentStart)) == "<=>")
                {
                    return data.Substring(currentStart, 3);
                }
                else
                {
                    throw new TokeniserException(currentStart, "Expecting =>");
                }
            }

            // Try a variable
            if (c == 'v')
            {
                // Read until end of variable
                int len = 1;
                while (currentStart + len < data.Length)
                {
                    char c2 = data[currentStart + len];

                    // Check if ending symbol
                    if (!IsCharAllowedInVariable(c2))
                    {
                        if (len == 1)
                        {
                            throw new TokeniserException(currentStart, "Variable must contain a name");
                        }
                        else
                        {
                            return data.Substring(currentStart, len);
                        }
                    }
                    else if (IsCharAllowedInVariable(c2))
                    {
                        len++;
                    }
                    else
                    {
                        throw new TokeniserException(currentStart + len + 1, $"Variable cannot contain this '{c2}'");
                    }
                }

                // Reached end of string while parsing variable
                if (len == 1)
                {
                    throw new TokeniserException(currentStart, "Variable must contain a name");
                }
                else
                {
                    return data.Substring(currentStart, len);
                }
            }

            throw new TokeniserException(currentStart, $"Unexpected character '{c}'");
        }

        private static bool IsCharAllowedInVariable(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_';
        }

        public bool AtEnd()
        {
            ConsumeWhiteSpace();
            return currentStart == data.Length;
        }

        public void Advance()
        {
            string token = Peek();
            currentStart += token.Length;
        }

        /// <summary>
        /// Test if the next token matches a given target.
        /// </summary>
        /// <param name="target">The target to match.</param>
        /// <returns>True if matched.</returns>
        public bool IsMatch(string target)
        {
            if (AtEnd())
            {
                return false;
            }

            string token = Peek();
            return token == target;
        }

        /// <summary>
        /// Test if the next token matches a given target and advances if so.
        /// </summary>
        /// <param name="target">The target to match.</param>
        /// <returns>True if matched.</returns>
        public bool IsMatchAndAdvance(string target)
        {
            bool result = IsMatch(target);

            if (result)
            {
                Advance();
            }
            return result;
        }

        /// <summary>
        /// Match a variable if possible, advances if a variable is matched. If a variable is not
        /// matched, the state of the tokeniser is unchanged.
        /// </summary>
        /// <returns>The variable if matched, null otherwise.</returns>
        public string? PeekVariableAndAdvance()
        {
            if (AtEnd())
            {
                return null;
            }

            string token = Peek();
            if (token.StartsWith("v") && Regex.IsMatch(token, @"^v[a-zA-Z0-9_]+$"))
            {
                Advance();
                return token;
            }

            return null;
        }
        //public string PeekVariableAndAdvance()
        //{
        //    if (AtEnd())
        //    {
        //        return null;
        //    }
        //    string token = Peek();
        //    if (token.StartsWith("v") && Regex.IsMatch(token, @"^v[a-zA-Z0-9_]+$"))
        //    {
        //        Advance();
        //        return token;
        //    }
        //    return null;
        //}

        public int GetPosition()
        {
            return currentStart;
        }
    }
}


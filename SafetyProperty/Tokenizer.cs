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
// Description:  This file defines the Tokenizer class, which breaks a textual safety property into a stream of tokens (variables, operators, 
// and brackets) for use by the safety property parser.
//
// ------------------
//
// GPL-3.0 license
//

namespace SwanLLVerifier.SafetyProperty
{
    public class Tokenizer
    {
        private readonly string data;
        private int currentStart = 0;

        public Tokenizer(string data)
        {
            this.data = data;
        }

        private void ConsumeWhiteSpace()
        {
            int len = 0;

            while (currentStart + len < data.Length)
            {
                char c = data[currentStart + len];

                if (char.IsWhiteSpace(c))
                {
                    len++;
                }
                else
                {
                    break;
                }
            }

            currentStart += len;
        }

        public string Peek()
        {
            ConsumeWhiteSpace();

            if (AtEnd())
            {
                throw new SafetyPropertyParserException(currentStart, "Expecting token, but end of string found");
            }

            char c = data[currentStart];

            // check for single char symbol
            if (c.Equals('(') || c.Equals(')') || c.Equals('&') || c.Equals('|') || c.Equals('~'))
            {
                return c.ToString();
            }


            // check for multi-char symbols
            if (c.Equals('-'))
            {
                if (data.Substring(currentStart, 2).Equals("->"))
                {
                    return data.Substring(currentStart, 2);
                }
                else
                {
                    throw new SafetyPropertyParserException(currentStart + 2, "Expecting '>'");
                }
            }

            if (c.Equals('<'))
            {
                if (data.Substring(currentStart, 3).Equals("<->"))
                {
                    return data.Substring(currentStart, 3);
                }
                else
                {
                    throw new SafetyPropertyParserException(currentStart + 2, "Expecting '->'");
                }
            }

            // Try a variable starting with a quote
            if (c.Equals('"'))
            {
                // Read until next quote.
                int len = 1;
                while (currentStart + len < data.Length)
                {
                    char c2 = data[currentStart + len];

                    if (c2.Equals('"'))
                    {
                        // Found next quote. Whole thing is variable.
                        return data.Substring(currentStart, len + 1);
                    }
                    if (char.IsLetterOrDigit(c2) || c2.Equals('_') || c2.Equals('.') || c2.Equals('(') || c2.Equals(')') || c2.Equals('/'))
                    {
                        // Allowed char in variable, move on.
                        len++;
                    }
                    else
                    {
                        throw new SafetyPropertyParserException(currentStart + len + 1, $"variable cannot contain character '{c2}'");
                    };
                }

                throw new SafetyPropertyParserException(currentStart + len + 1, "Expecting ending \"");
            }

            throw new SafetyPropertyParserException(currentStart + 1, $"Unexpected character '{c}'");
        }

        public int Mark()
        {
            return currentStart;
        }

        public void Reset(int mark)
        {
            currentStart = mark;
        }

        public bool PeekIsMatch(string target)
        {
            if (AtEnd())
            {
                return false;
            }

            string token = Peek();

            return token.Equals(target);
        }

        public bool PeekIsMatchAdvance(string target)
        {
            bool result = PeekIsMatch(target);
            if (result)
            {
                Advance();
            }
            return result;
        }

        public bool PeekIsVariable()
        {
            if (AtEnd())
            {
                return false;
            }

            string token = Peek();

            return token[0].Equals('"');
        }

        public void Advance()
        {
            string token = Peek();
            currentStart += token.Length;
        }

        public bool AtEnd()
        {
            ConsumeWhiteSpace();
            return currentStart == data.Length;
        }
    }
}

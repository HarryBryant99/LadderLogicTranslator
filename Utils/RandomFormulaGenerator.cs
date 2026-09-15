using Siemens.ETCSDC;
using Siemens.ETCSDC.Properties;
using static SwanLLVerifier.PropositionalLogic.PropositionalFormulaBuilder;

namespace SwanLLVerifier.Utils
{
    public static class RandomFormulaGenerator
    {
        public static int iterationCount = 0;
        public static AbstractFirstOrderFormula Generate(int depth)
        {
            if (depth > 10)
                depth = 10;

            iterationCount++;

            Random rnd = new();

            if (depth == 0)
            {
                return MakeVar("z");
            }
            else
            {
                if (iterationCount <= 3) // if generate has not been recursively called at least twice, keep generating anyways
                {
                    int rndInt = rnd.Next(0, 4);
                    if (rndInt == 0)
                    {
                        return MakeBrackets(MakeAnd(Generate(depth), Generate(depth)));
                    }
                    else if (rndInt == 1)
                    {
                        return MakeBrackets(MakeOr(Generate(depth), Generate(depth)));
                    }
                    else if (rndInt == 2)
                    {
                        return MakeImplication(Generate(depth), Generate(depth));
                    }
                    else if (rndInt == 3)
                    {
                        return MakeNegation(Generate(depth));
                    }
                    else// if (rndInt == 4)
                    {
                        return MakeEquivalence(Generate(depth), Generate(depth));
                    }
                }
                else
                {
                    int rndInt = rnd.Next(0, 7);
                    if (rndInt == 0)
                    {
                        return MakeBrackets(MakeAnd(Generate(depth - 1), Generate(depth - 1)));
                    }
                    else if (rndInt == 1)
                    {
                        return MakeBrackets(MakeOr(Generate(depth - 1), Generate(depth - 1)));
                    }
                    else if (rndInt == 2)
                    {
                        return MakeImplication(Generate(depth - 1), Generate(depth - 1));
                    }
                    else if (rndInt == 3)
                    {
                        return MakeNegation(Generate(depth - 1));
                    }
                    else if (rndInt == 4)
                    {
                        return MakeEquivalence(Generate(depth - 1), Generate(depth - 1));
                    }
                    else if (rndInt == 5)
                    {
                        return MakeVar("a");
                    }
                    else if (rndInt == 6)
                    {
                        return MakeVar("b");
                    }
                    else // if (rndInt == 7)
                    {
                        return MakeVar("c");
                    }
                }
            }
        } // end Generate
    } // end RandomFormulaGenerator

}

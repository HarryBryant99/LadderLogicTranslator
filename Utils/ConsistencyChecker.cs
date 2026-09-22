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
// Description:  This file defines the ConsistencyChecker class, which compares the results of different verification techniques (Inductive 
// Verification, Bounded Model Checking, and IC3) to determine whether their outcomes are logically consistent with one another.
//
// ------------------
//
// GPL-3.0 license
//

namespace SwanLLVerifier.Utils
{
    public static class ConsistencyChecker
    {
        public enum VersusType
        {
            IvVsBmc,
            IvVsIc3,
            BmcVsIc3
        }

        public static bool CalculateConsistency(VersusType vsType, bool arg1, bool arg2)
        {
            return vsType switch
            {
                VersusType.IvVsBmc => CheckIvVsBmcConsistency(arg1, arg2),
                VersusType.IvVsIc3 => CheckIvVsIc3ResultConsistency(arg1, arg2),
                VersusType.BmcVsIc3 => CheckBmcVsIc3ResultConsistency(arg1, arg2),
                _ => throw new ArgumentException("Ïnvalid Arguments.")
            };
        }

        public static bool CheckIvVsBmcConsistency(bool ivResult, bool bmcResult)
        {
            if ((ivResult && bmcResult) || (!ivResult && bmcResult) || (!ivResult & !bmcResult))
                return true;
            else // (ivResult && !bmcResult)
                return false;
        }

        public static bool CheckIvVsIc3ResultConsistency(bool ivResult, bool ic3Result)
        {
            if ((ivResult && ic3Result) || (!ivResult && ic3Result) || (!ivResult & !ic3Result))
                return true;
            else // (ivResult && !ic3Result)
                return false;
        }

        public static bool CheckBmcVsIc3ResultConsistency(bool bmcResult, bool ic3Result)
        {
            if ((bmcResult && ic3Result) || (bmcResult && !ic3Result) || (!bmcResult & !ic3Result))
                return true;
            else // (!bmcResult && ic3Result)
                return false;
        }
    }
}

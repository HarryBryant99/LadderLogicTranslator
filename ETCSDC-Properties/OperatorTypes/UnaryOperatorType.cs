//////// 
/// Written by Siemens Mobility UK
// approved to publish by Siemens Mobility UK in September 2026
// no liability 
// code may be used freely
// File Details 
// -------------- 
// Filename:   UnaryOperatorType.cs
//
// File Description
// -----------------
// Description:  A class representing Unary Operators.
//
// Protection Class: Public
//
// SPDX-FileCopyrightText: Copyright 2021-2024 Siemens Mobility Limited
//
// SPDX-License-Identifier: LGPL-3.0-only
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
//using Newtonsoft.Json;
//using Siemens.ETCSModularDataParser.Logging;

namespace Siemens.ETCSDC.Properties
{
    /// <summary>
    /// Class to represent a first order logic expression with a unary operator i.e. an operator with one argument.
    /// </summary>
    [DataContract]
    public abstract class UnaryOperatorType : AbstractFirstOrderFormula
    {
        //[System.Xml.Serialization.XmlElement("Forall", typeof(Forall), Namespace = Constants.NAMESPACE)]
        //[System.Xml.Serialization.XmlElement("Exists", typeof(Exists), Namespace = Constants.NAMESPACE)]
        [System.Xml.Serialization.XmlElement("Negation", typeof(Negation), Namespace = Constants.NAMESPACE)]
        [System.Xml.Serialization.XmlElement("And", typeof(And), Namespace = Constants.NAMESPACE)]
        [System.Xml.Serialization.XmlElement("Or", typeof(Or), Namespace = Constants.NAMESPACE)]
        [System.Xml.Serialization.XmlElement("Implies", typeof(Implies), Namespace = Constants.NAMESPACE)]
        [System.Xml.Serialization.XmlElement("Predicate", typeof(Predicate), Namespace = Constants.NAMESPACE)]
        [System.Xml.Serialization.XmlElement("Equivalent", typeof(Equivalent), Namespace = Constants.NAMESPACE)]
        [System.Xml.Serialization.XmlElement("Brackets", typeof(Brackets), Namespace = Constants.NAMESPACE)]
        //[System.Xml.Serialization.XmlElement("Equality", typeof(Equality), Namespace = Constants.NAMESPACE)]
        [System.Xml.Serialization.XmlChoiceIdentifier("OperandType")]

        /// <summary>
        /// Property to get and set the operand on the formula.
        /// </summary>
        [DataMember(Name = "operand")]
        public AbstractFirstOrderFormula Operand { get; set; }

        /// <summary>
        /// Property to get the operand type for the formula
        /// </summary>
        public AbstractFirstOrderFormula.FOLFormulaType OperandType
        {
            get { return Operand.FormulaType; }
            set { Operand.FormulaType = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public UnaryOperatorType()
        {
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        //public override string ToJson()
        //{
        //    return JsonConvert.SerializeObject(this, Formatting.Indented);
        //}

        /// <summary>
        /// Returns true if UnaryOperatorType formula instances are equal
        /// </summary>
        /// <param name="input">Instance of UnaryOperatorType formula to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(AbstractFirstOrderFormula input)
        {
            //Log.Information("UnaryOperatorType AbstractFirstOrderFormula  equality called", string.Empty, LogClient.Checker);
            if (input == null)
                return false;

            if (input.GetType() != this.GetType())
                return false;

            return this.Equals((UnaryOperatorType)input);
        }

        /// <summary>
        /// Returns true if UnaryOperatorType instances are equal
        /// </summary>
        /// <param name="input">Instance of UnaryOperatorType to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(UnaryOperatorType input)
        {
            if (input == null)
                return false;

            return
                (
                    this.FormulaType == input.FormulaType ||
                    this.FormulaType.Equals(input.FormulaType)
                ) &&
                (this.Operand == input.Operand ||
                    this.Operand != null &&
                    input.Operand != null &&
                    this.Operand.Equals(input.Operand)
                );
        }


    }//end UnaryOperatorType

}//end namespace PropertySchema
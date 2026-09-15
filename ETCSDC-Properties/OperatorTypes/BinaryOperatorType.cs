// Written by Siemens Mobility UK
// approved to publish by Siemens Mobility UK in September 2026
// no liability 
// code may be used freely
// File Details 
// -------------- 
// Filename:   BinaryOperatorType.cs
//
// File Description
// ------------------
// Description:  A class representing a binary operator expression.
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
using Newtonsoft.Json;
using Siemens.ETCSModularDataParser.Logging;

namespace Siemens.ETCSDC.Properties
{
	/// <summary>
	/// Abstract class representing an expression with a binary operator.
	/// </summary>
	[DataContract]
	public abstract class BinaryOperatorType : AbstractFirstOrderFormula ,  IEquatable<BinaryOperatorType>
	{
		[System.Xml.Serialization.XmlElement("Forall", typeof(Forall), Namespace = Constants.NAMESPACE)]
		[System.Xml.Serialization.XmlElement("Exists", typeof(Exists), Namespace = Constants.NAMESPACE)]
		[System.Xml.Serialization.XmlElement("Negation", typeof(Negation), Namespace = Constants.NAMESPACE)]
		[System.Xml.Serialization.XmlElement("And", typeof(And), Namespace = Constants.NAMESPACE)]
		[System.Xml.Serialization.XmlElement("Or", typeof(Or), Namespace = Constants.NAMESPACE)]
		[System.Xml.Serialization.XmlElement("Implies", typeof(Implies), Namespace = Constants.NAMESPACE)]
		[System.Xml.Serialization.XmlElement("Predicate", typeof(Predicate), Namespace = Constants.NAMESPACE)]
		[System.Xml.Serialization.XmlElement("Equivalent", typeof(Equivalent), Namespace = Constants.NAMESPACE)]
		[System.Xml.Serialization.XmlElement("Brackets", typeof(Brackets), Namespace = Constants.NAMESPACE)]
		[System.Xml.Serialization.XmlElement("Equality", typeof(Equality), Namespace = Constants.NAMESPACE)]
		[System.Xml.Serialization.XmlChoiceIdentifier("OperandTypes")]

		/// <summary>
		/// Property to get and set the operands of the expression as an array.
		/// </summary>
		public AbstractFirstOrderFormula[] Operands { get; set; }


		/// <summary>
		/// Property to get and set the left operand of the expression.
		/// </summary>
		[DataMember(Name = "leftoperand")]
		public AbstractFirstOrderFormula LeftOperand 
		{ 
			get { return Operands[0]; } 
			set { Operands[0] = value; } 
		}

		/// <summary>
		/// Property to get and set the right operand of the expression.
		/// </summary>
		[DataMember(Name = "rightoperand")]
		public AbstractFirstOrderFormula RightOperand
		{
			get { return Operands[1]; }
			set { Operands[1] = value; }
		}

		/// <summary>
		/// Property to get and set the types of the operands
		/// </summary>
		public AbstractFirstOrderFormula.FOLFormulaType [] OperandTypes
		{
			get { return new FOLFormulaType[] { Operands[0].FormulaType, Operands[1].FormulaType }; }
			set { 
				  Operands[0].FormulaType = value[0]; 
				  Operands[1].FormulaType = value[1]; 
			    }
		}

		/// <summary>
		/// Default Constructor
		/// </summary>
		public BinaryOperatorType()
		{
			Operands = new AbstractFirstOrderFormula[2];
		}

		/// <summary>
		/// Returns true if BinaryOperatorType formula instances are equal
		/// </summary>
		/// <param name="input">Instance of BinaryOperatorType formula to be compared</param>
		/// <returns>Boolean</returns>
		public override bool Equals(AbstractFirstOrderFormula input)
		{
			Log.Information("BinaryOperatorType AbstractFirstOrderFormula equality called", string.Empty, LogClient.Checker);
			if (input == null)
				return false;

			if (input.GetType() != this.GetType())
				return false;

			return this.Equals((BinaryOperatorType)input);
		}

		/// <summary>
		/// Returns true if BinaryOperatorType instances are equal
		/// </summary>
		/// <param name="input">Instance of BinaryOperatorType to be compared</param>
		/// <returns>Boolean</returns>
		public bool Equals(BinaryOperatorType input)
		{

			if (input == null)
				return false;

			return
				(
					this.FormulaType == input.FormulaType ||
					this.FormulaType.Equals(input.FormulaType)
				) &&
				(this.LeftOperand == input.LeftOperand ||
					this.LeftOperand != null &&
					input.LeftOperand != null &&
					this.LeftOperand.Equals(input.LeftOperand)
				) &&
				(this.RightOperand == input.RightOperand ||
					this.RightOperand != null &&
					input.RightOperand != null &&
					this.RightOperand.Equals(input.RightOperand)
				);
		}

	}//end BinaryOperatorType

}//end namespace PropertySchema
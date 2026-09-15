// Written by Siemens Mobility UK
// approved to publish by Siemens Mobility UK in September 2026
// no liability 
// code may be used freely
// File Details 
// -------------- 
// Filename:   AbstractFirstOrderFormula.cs
//
// File Description
// ------------------
// Description:  A class representing an abstract first order formulae
//
// Protection Class: Public
//
// SPDX-FileCopyrightText: Copyright 2021-2024 Siemens Mobility Limited
//
// SPDX-License-Identifier: LGPL-3.0-only
//


using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Siemens.ETCSDC.PropertyVisitor;
using Siemens.ETCSModularDataParser.Logging;
using System;
using System.Runtime.Serialization;

namespace Siemens.ETCSDC.Properties
{
    /// <summary>
    /// Abstract base class representing formulae from first order logic.
    /// First order logic is an extension of propositional logic i.e. And,Or,Not,Implication etc plus predicates and quantifiers.
    /// </summary>
    [DataContract]
	public abstract class AbstractFirstOrderFormula : IEquatable<AbstractFirstOrderFormula>
	{
		/// <summary>
		/// Enum capturing the type of the first order formula
		/// </summary>
		[JsonConverter(typeof(StringEnumConverter))]
		public enum FOLFormulaType
		{
			/// <remarks/>
			[EnumMember(Value = "And")]
			And,

			/// <remarks/>
			[EnumMember(Value = "Or")]
			Or,

			/// <remarks/>
			[EnumMember(Value = "Implies")]
			Implies,

			/// <remarks/>
			[EnumMember(Value = "Negation")]
			Negation,

			/// <remarks/>
			[EnumMember(Value = "Equivalent")]
			Equivalent,

			/// <remarks/>
			[EnumMember(Value = "Brackets")]
			Brackets,

			/// <remarks/>
			[EnumMember(Value = "Forall")]
			Forall,

			/// <remarks/>
			[EnumMember(Value = "Exists")]
			Exists,

			/// <remarks/>
			[EnumMember(Value = "Predicate")]
			Predicate,

			/// <remarks/>
			[EnumMember(Value = "Equality")]
			Equality,
		}

		/// <summary>
		/// Default Constructor
		/// </summary>
		public AbstractFirstOrderFormula()
		{
		}


		/// <summary>
		/// Property to access the first order formula type.
		/// </summary>
		[DataMember(Name = "formulatype")]
		public FOLFormulaType FormulaType { get; set; }

		/// <summary>
		/// Method to accept a property visitor.
		/// </summary>
		/// <param name="visitor">The visitor being accepted</param>
		public abstract Object Accept(IPropertyVisitor visitor);



		/// <summary>
		/// Returns the JSON string presentation of the object
		/// </summary>
		/// <returns>JSON string presentation of the object</returns>
		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		/// <summary>
		/// Returns true if AbstractFirstOrderFormula instances are equal
		/// </summary>
		/// <param name="input">Instance of AbstractFirstOrderFormula to be compared</param>
		/// <returns>Boolean</returns>
		public virtual bool Equals(AbstractFirstOrderFormula input)
		{
			Log.Information("AbstractFirstOrderFormula equality called", string.Empty, LogClient.Checker);
			if (input == null)
				return false;

			return
				(
					this.FormulaType == input.FormulaType ||
					this.FormulaType.Equals(input.FormulaType)
				) ;
		}


	}//end AbstractFirstOrderFormula

}//end namespace PropertySchema
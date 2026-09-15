//////// 
/// Written by Siemens Mobility UK
// approved to publish by Siemens Mobility UK in September 2026
// no liability 
// code may be used freely
// File Details 
// -------------- 
// Filename:   Term.cs
//
// File Description
// -----------------
// Description:  A class representing a Term.
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
//using Newtonsoft.Json.Converters;
//using Siemens.ETCSDC.PropertyVisitor;
//using Siemens.ETCSModularDataParser.Logging;

namespace Siemens.ETCSDC.Properties
{
    /// <summary>
    /// Enumeration representing the type of the term.
    /// </summary>
    [DataContract]
    [System.Xml.Serialization.XmlType(Namespace = Constants.NAMESPACE, IncludeInSchema = false)]
    //[JsonConverter(typeof(StringEnumConverter))]
    public enum TermType
    {
        /// <remarks/>
        [EnumMember(Value = "NamedConstant")]
        NamedConstant,

        /// <remarks/>
        [EnumMember(Value = "LiteralConstant")]
        LiteralConstant,

        /// <remarks/>
        [EnumMember(Value = "Var")]
        Var,

        /// <remarks/>
        [EnumMember(Value = "Function")]
        Function,
    }

    /// <summary>
    /// Class representing a term.
    /// </summary>
    [System.Serializable()]
    [DataContract]
    [System.Xml.Serialization.XmlRoot("Term", IsNullable = false, Namespace = Constants.NAMESPACE)]
    public class Term : IEquatable<Term>
    {
        /// <summary>
        /// Property to get and set the value of the term.
        /// </summary>
        [System.Xml.Serialization.XmlElement("Var", typeof(string), Namespace = Constants.NAMESPACE)]
        [System.Xml.Serialization.XmlElement("NamedConstant", typeof(string), Namespace = Constants.NAMESPACE)]
        //[System.Xml.Serialization.XmlElement("LiteralConstant", typeof(LiteralConstant), Namespace = Constants.NAMESPACE)]
        //[System.Xml.Serialization.XmlElement("Function", typeof(Siemens.ETCSDC.Properties.Function))]
        [System.Xml.Serialization.XmlChoiceIdentifier("TypeValue")]
        [DataMember(Name = "value")]
        public object Value
        {
            get;
            set;
        }

        /// <summary>
        /// Property to get and set the type of the term
        /// </summary>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        [DataMember(Name = "typevalue")]
        public TermType TypeValue
        {
            get;
            set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Term()
        {
        }
        /// <summary>
        /// Method to accept a property visitor.
        /// </summary>
        /// <param name="visitor">The visitor being accepted</param>
        //public Object Accept(IPropertyVisitor visitor)
        //{
        //    return visitor.Visit(this);
        //}


        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        //public string ToJson()
        //{
        //    return JsonConvert.SerializeObject(this, Formatting.Indented);
        //}

        /// <summary>
        /// Returns true if Term instances are equal
        /// </summary>
        /// <param name="input">Instance of Term to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Term input)
        {
            bool result = true;
            //Log.Information("Term equality called", string.Empty, LogClient.Checker);
            if (input == null)
                result = false;

            if (result && (this.TypeValue != input.TypeValue))
                result = false;

            if (result && (this.TypeValue == TermType.Function))
                //result = ((Function)this.Value).Equals((Function)input.Value);

            if (result && (this.TypeValue == TermType.LiteralConstant))
                //result = ((LiteralConstant)this.Value).Equals((LiteralConstant)input.Value);

            if (result && (this.TypeValue == TermType.NamedConstant || this.TypeValue == TermType.Var))
                result = ((string)this.Value) == ((string)input.Value);

            return result;
        }

    }//end Term

}//end namespace PropertySchema
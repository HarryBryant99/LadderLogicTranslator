//////// 
/// Written by Siemens Mobility UK
// approved to publish by Siemens Mobility UK in September 2026
// no liability 
// code may be used freely
// File Details 
// -------------- 
// Filename:   Property.cs
//
// File Description
// ------------------
// Description:  A class representing a Property.
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
    static class Constants
    {
        public const string NAMESPACE = "https://sigrad.siemens.cloud/SafetyPropSchema";
    }

    /// <summary>
    /// Class representing a property. Properties are logical statements about a model which can be checked and verified.
    /// </summary>
    [DataContract]
    [System.Serializable()]
    [System.Xml.Serialization.XmlRoot("Property", IsNullable = false, Namespace = Constants.NAMESPACE)]
    public class Property : IEquatable<Property>
    {


        /// <summary>
        /// Property to get and set the description of the property.
        /// </summary>
        [System.Xml.Serialization.XmlElement(Namespace = Constants.NAMESPACE)]
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Property to get and set the name of the property.
        /// </summary>
        [DataMember(Name = "id")]
        [System.Xml.Serialization.XmlAttribute("id", Namespace = Constants.NAMESPACE)]
        public string Id { get; set; }

        /// <summary>
        /// Property to get and set the formula associated with the property.
        /// </summary>
        [DataMember(Name = "formula")]
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
        [System.Xml.Serialization.XmlChoiceIdentifier("FormulaType")]
        public Siemens.ETCSDC.Properties.AbstractFirstOrderFormula FirstOrderFormula { get; set; }

        /// <summary>
        /// Property to get and set the type of the formula associated with the property.
        /// </summary>
        public AbstractFirstOrderFormula.FOLFormulaType FormulaType
        {
            get { return FirstOrderFormula.FormulaType; }
            set { FirstOrderFormula.FormulaType = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>

        public Property()
        {
        }

        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        //public string ToJson()
        //{
        //    return JsonConvert.SerializeObject(this, Formatting.Indented);
        //}

        /// <summary>
        /// Returns true if Property instances are equal
        /// </summary>
        /// <param name="input">Instance of Property to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Property input)
        {
            //Log.Information("Property equality called", string.Empty, LogClient.Checker);
            if (input == null)
                return false;

            return
                (
                    this.Id == input.Id ||
                    (this.Id != null &&
                    this.Id.Equals(input.Id))
                ) &&
                (
                    this.Description == input.Description ||
                    this.Description != null &&
                    input.Description != null &&
                    this.Description.Equals(input.Description)
                ) &&
                (
                    this.FirstOrderFormula == input.FirstOrderFormula ||
                    (this.FirstOrderFormula != null &&
                     input.FirstOrderFormula != null &&
                    this.FirstOrderFormula.Equals(input.FirstOrderFormula)));
        }


    }//end Property

}//end namespace PropertySchema
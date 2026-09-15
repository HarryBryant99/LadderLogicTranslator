// Written by Siemens Mobility UK
// approved to publish by Siemens Mobility UK in September 2026
// no liability 
// code may be used freely
// File Details 
// -------------- 
// Filename:   Predicate.cs
//
// File Description
// ------------------
// Description:  A class representing a Predicate.
//
// Protection Class: Public
//
// SPDX-FileCopyrightText: Copyright 2021 Siemens Mobility Limited
//
// SPDX-License-Identifier: LGPL-3.0-only
//

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Linq;
using Siemens.ETCSDC.PropertyVisitor;

namespace Siemens.ETCSDC.Properties
{
    /// <summary>
    /// Class to represent an logical expression containing a predicate. For example, the predicate P(a,b) has a name P and inner terms a and b.
    /// </summary>
    [DataContract]
    [System.Serializable()]
    [System.Xml.Serialization.XmlRootAttribute("Predicate", IsNullable = false, Namespace = Constants.NAMESPACE)]
    public class Predicate : AbstractFirstOrderFormula, IEquatable<Predicate>, IEquatable<AbstractFirstOrderFormula>
    {
        /// <summary>
        /// Property to get and set the name of the Predicate.
        /// </summary>
        [DataMember(Name = "name")]
        [System.Xml.Serialization.XmlAttribute("name")]
        public string Name { get; set; }

        /// <summary>
        /// Property to get and set the terms passed as arguements to the predicate.
        /// </summary>
        [DataMember(Name = "term")]
        [System.Xml.Serialization.XmlElement("Term", typeof(Siemens.ETCSDC.Properties.Term), Namespace = Constants.NAMESPACE)]
        public Siemens.ETCSDC.Properties.Term[] Term { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Predicate()
        {
        }

        /// <summary>
        /// Method to accept a property visitor.
        /// </summary>
        /// <param name="visitor">The visitor being accepted</param>
        public override Object Accept(IPropertyVisitor visitor)
        {
            return visitor.Visit(this);
        }

        /// <summary>
        /// Returns true if Predicate instances are equal
        /// </summary>
        /// <param name="input">Instance of Predicate to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(AbstractFirstOrderFormula input)
        {
            if (input == null)
                return false;

            if (input.GetType() != this.GetType())
                return false;

            return this.Equals((Predicate)input);
        }


        /// <summary>
        /// Returns true if Predicate instances are equal
        /// </summary>
        /// <param name="input">Instance of Predicate to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Predicate input)
        {
            if (input == null)
                return false;

            return
                (
                    this.FormulaType == input.FormulaType ||
                    this.FormulaType.Equals(input.FormulaType)
                ) &&
                (this.Name == input.Name ||
                    this.Name != null &&
                    input.Name != null &&
                    this.Name.Equals(input.Name)
                ) &&
                (this.Term == input.Term ||
                    this.Term != null &&
                    input.Term != null &&
                    Enumerable.SequenceEqual(this.Term, input.Term)
                );
        }

    }//end Predicate

}//end namespace PropertySchema
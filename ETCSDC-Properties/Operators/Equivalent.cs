// Written by Siemens Mobility UK
// approved to publish by Siemens Mobility UK in September 2026
// no liability 
// code may be used freely
// File Details 
// -------------- 
// Filename:   Equivalent.cs
//
// File Description
// ------------------
// Description:  A class representing an equivalence relation expression.
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
using Siemens.ETCSDC.PropertyVisitor;

namespace Siemens.ETCSDC.Properties
{
	/// <summary>
	/// Class to represent a expression containing an equivalance operator.
	/// </summary>
	[DataContract]
	public class Equivalent : BinaryOperatorType 
	{
		/// <summary>
		/// Default Constructor
		/// </summary>
		public Equivalent()
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

	}//end Equivalent

}//end namespace PropertySchema
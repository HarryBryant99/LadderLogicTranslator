// Written by Siemens Mobility UK
// approved to publish by Siemens Mobility UK in September 2026
// no liability 
// code may be used freely
// File Details 
// -------------- 
// Filename:   Or.cs
//
// File Description
// ------------------
// Description:  A class representing a logical or expression.
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
//using Newtonsoft.Json;
//using Siemens.ETCSDC.PropertyVisitor;

namespace Siemens.ETCSDC.Properties
{
	/// <summary>
	/// Class to represent an logical expression containing an or operation.
	/// </summary>
	[DataContract]
	public class Or : BinaryOperatorType 
	{
		/// <summary>
		/// Default constructor
		/// </summary>
		public Or()
		{
		}

		/// <summary>
		/// Method to accept a property visitor.
		/// </summary>
		/// <param name="visitor">The visitor being accepted</param>
		//public override Object Accept(IPropertyVisitor visitor)
		//{
		//	return visitor.Visit(this);
		//}
	}//end Or

}//end namespace PropertySchema
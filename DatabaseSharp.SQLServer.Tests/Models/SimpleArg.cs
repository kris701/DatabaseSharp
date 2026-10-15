using DatabaseSharp.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseSharp.SQLServer.Tests.Models
{
	public class SimpleArg
	{
		[DatabaseSharp(ParameterName = "ARGVALUE")]
		public int ArgValue { get; set; }

		public SimpleArg(int argValue)
		{
			ArgValue = argValue;
		}
	}
}

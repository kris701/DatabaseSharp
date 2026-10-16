using DatabaseSharp.Attributes;

namespace DatabaseSharp.PostgreSQL.Tests.Models
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

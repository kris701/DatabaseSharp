using DatabaseSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseSharp.Tests.TestModels
{
	public class TestClass9
	{
		public string Name { get; set; }
		public string Value { get; set; }
		[DatabaseSharp(FillTable = 1)]
		public List<TestClass3> Items { get; set; }
		[DatabaseSharp(FillTable = 2)]
		public string Value3 { get; set; }
	}
}

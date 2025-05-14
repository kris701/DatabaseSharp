using DatabaseSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseSharp.Tests.TestModels
{
	public class TestClass8
	{
		public string Name { get; set; }
		public string Value { get; set; }
		[DatabaseSharp(FillTable = 1, ColumnName = "val2")]
		public List<string> Items { get; set; }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseSharp
{
	public class DBBase
	{
		public DBClient Client { get; }

		public DBBase(DBClient client)
		{
			Client = client;
		}
	}
}

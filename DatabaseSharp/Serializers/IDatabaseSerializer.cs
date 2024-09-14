using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseSharp.Serializers
{
	public interface IDatabaseSerializer
	{
		public dynamic Deserialise(string text, Type asType);
		public string Serialize(dynamic item);
	}
}

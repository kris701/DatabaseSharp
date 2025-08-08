using DatabaseSharp.Models;
using System.Reflection;

namespace DatabaseSharp.Serializers
{
	public interface IDatabaseSerializer
	{
		public Type? OverrideDatabaseType { get; }
		public dynamic? Deserialise(PropertyInfo propInfo, DatabaseResultRow row, DatabaseResult source);
		public ISQLParameter Serialize(object item, PropertyInfo propInfo);
	}
}

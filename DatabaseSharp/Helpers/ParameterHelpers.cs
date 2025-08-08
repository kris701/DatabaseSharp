using DatabaseSharp.Attributes;
using DatabaseSharp.Models;
using DatabaseSharp.Serializers;
using System.Reflection;

namespace DatabaseSharp.Helpers
{
	public static class ParameterHelpers
	{
		public static List<ISQLParameter>? GenerateParametersFromObject(object item, Dictionary<string, IDatabaseSerializer> serializers)
		{
			var parameters = new List<ISQLParameter>();
			if (item != null)
			{
				var props = item.GetType().GetProperties();
				foreach (var prop in props)
				{
					var parameter = GenerateParameterFromProperty(item, prop, serializers);
					if (parameter != null)
						parameters.Add(parameter);
				}
			}

			if (parameters.Count == 0)
				return null;
			return parameters;
		}

		public static ISQLParameter? GenerateParameterFromProperty(object item, PropertyInfo prop, Dictionary<string, IDatabaseSerializer> serializers)
		{
			if (prop.GetCustomAttribute<DatabaseSharpIgnoreAttribute>() is DatabaseSharpIgnoreAttribute ignoreData)
				if (ignoreData.IgnoreAsParameter)
					return null;

			var serializerName = DatabaseDefaultSerializer.SerializerName;
			if (prop.GetCustomAttribute<DatabaseSharpAttribute>() is DatabaseSharpAttribute attr)
			{
				if (attr.Serializer != null)
					serializerName = attr.Serializer;
			}
			var serializer = serializers[serializerName];
			return serializer.Serialize(item, prop);
		}
	}
}

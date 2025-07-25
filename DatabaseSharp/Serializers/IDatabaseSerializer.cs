namespace DatabaseSharp.Serializers
{
	public interface IDatabaseSerializer
	{
		public Type DatabaseType { get; }
		public dynamic Deserialise(string text, Type asType);
		public string Serialize(dynamic item, Type asType);
	}
}

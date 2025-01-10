namespace DatabaseSharp.Serializers
{
	public interface IDatabaseSerializer
	{
		public dynamic Deserialise(string text, Type asType);
		public string Serialize(dynamic item, Type asType);
	}
}

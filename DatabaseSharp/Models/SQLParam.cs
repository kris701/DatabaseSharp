namespace DatabaseSharp.Models
{
	public class SQLParam
	{
		public string ParamName { get; set; }
		public object Value { get; set; }

		public SQLParam(string paramName, object value)
		{
			ParamName = paramName;
			Value = value;
		}
	}
}

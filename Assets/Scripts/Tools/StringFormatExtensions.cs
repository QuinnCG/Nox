namespace Game
{
	/// <summary>
	/// Look up "C# extension classes".
	/// Because of this class,
	/// now anywhere in the project you can type ".Bold()" or one of the other methods
	/// directly after a string to make that string bold or colored when using <c>Debug.Log</c>.
	/// </summary>
	public static class StringFormatExtensions
	{
		public static string Bold(this string str) => $"<b>{str}</b>";
		public static string Italic(this string str) => $"<i>{str}</i>";
		public static string Size(this string str, int point) => $"<size={point}>{str}</size>";
		public static string Color(this string str, StringColor color) => $"<color={color.ToString().ToLower()}>{str}</color>";
		public static string Color(this string str, string color) => $"<color={color}>{str}</color>";
	}
}

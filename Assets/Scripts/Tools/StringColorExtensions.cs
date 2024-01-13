namespace Quinn
{
	public static class StringColorExtensions
	{
		public static string Bold(this string str) => $"<b>{str}</b>";
		public static string Italic(this string str) => $"<i>{str}</i>";
		public static string Size(this string str, int point) => $"<size={point}>{str}</size>";
		public static string Color(this string str, StringColor color) => $"<color={color.ToString().ToLower()}>{str}</color>";
		public static string Color(this string str, string color) => $"<color={color}>{str}</color>";
	}
}

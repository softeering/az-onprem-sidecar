namespace AzOnPremSidecar.Utils
{
	public static class Extensions
	{
		public static string? Standardize(this string? source, bool emptyToNull = true, bool whitespaceToNull = true)
		{
			return string.IsNullOrWhiteSpace(source) ? null : source;
		}
	}
}

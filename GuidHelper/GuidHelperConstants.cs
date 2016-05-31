using System.Text.RegularExpressions;

namespace Kbg.NppPluginNET.GuidHelper
{
	public static class GuidHelperConstants
	{
		public const int StartPartOfGuidLength = 8;
		public const int EndPartOfGuidLength = 12;
		public const int Regexlength = 36;
		public static Regex GuidRex = new Regex("\\b[0-9a-fA-F]{8}[-]([0-9a-fA-F]{4}[-]){3}[0-9a-fA-F]{12}\\b", RegexOptions.Compiled);
	}
}

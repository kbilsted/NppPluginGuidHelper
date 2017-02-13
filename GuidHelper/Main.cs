using System.Windows.Forms;
using Kbg.NppPluginNET.PluginInfrastructure;
using Kbg.NppPluginNET.GuidHelper;

namespace Kbg.NppPluginNET
{
	class Main
	{
		internal const string PluginName = "&GuidHelper";

		static readonly SelectWholeGuidIfStartOrEndIsSelected selectWholeGuidIfStartOrEndIsSelected =
			new SelectWholeGuidIfStartOrEndIsSelected(PluginBase.GetGatewayFactory());

		public static void OnNotification(ScNotification notification)
		{
			selectWholeGuidIfStartOrEndIsSelected.Execute(notification);
		}

		internal static void CommandMenuInit()
		{
			PluginBase.SetCommand(0, "&Insert Guid", InsertGuid, new ShortcutKey(false, false, false, Keys.None));
			PluginBase.SetCommand(1, "&Insert GUID", InsertUpperCaseGuid, new ShortcutKey(false, false, false, Keys.None));
			PluginBase.SetCommand(2, "&About GuidHelper", ShowAbout, new ShortcutKey(false, false, false, Keys.None));
		}

		private static void ShowAbout()
		{
			var message = @"Version: 1.04

License: This is freeware (Apache v2.0 license).

Author: Kasper B. Graversen 2016-

Website: https://github.com/kbilsted/NppPluginGuidHelper";
			var title = "GuidHelper plugin";
			MessageBox.Show(message, title, MessageBoxButtons.OK);
		}

		internal static void SetToolBarIcon()
		{
		}

		internal static void PluginCleanUp()
		{
		}

		internal static void InsertUpperCaseGuid()
		{
			new InsertGuid(new ScintillaGateway(PluginBase.GetCurrentScintilla())).Execute(true);
		}

		internal static void InsertGuid()
		{
			new InsertGuid(new ScintillaGateway(PluginBase.GetCurrentScintilla())).Execute(false);
		}
	}
}
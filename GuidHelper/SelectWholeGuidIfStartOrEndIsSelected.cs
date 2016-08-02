using System;
using Kbg.NppPluginNET.PluginInfrastructure;

namespace Kbg.NppPluginNET.GuidHelper
{
	internal class SelectWholeGuidIfStartOrEndIsSelected
	{
		private readonly Func<IScintillaGateway> gatewayFactory;
		const int LengthGuidExceptLastPart = GuidHelperConstants.Regexlength - GuidHelperConstants.EndPartOfGuidLength;

		public SelectWholeGuidIfStartOrEndIsSelected(Func<IScintillaGateway> gatewayFactory)
		{
			this.gatewayFactory = gatewayFactory;
		}

		public void Execute(ScNotification notification)
		{
			if (notification.Header.Code != (ulong) SciMsg.SCN_UPDATEUI)
				return;
			if (notification.Updated != (int) SciMsg.SC_UPDATE_SELECTION)
				return;

			var scintillaGateway = gatewayFactory();
			var selectionLength = scintillaGateway.GetSelectionLength();

			if (selectionLength != GuidHelperConstants.StartPartOfGuidLength 
				&& selectionLength != GuidHelperConstants.EndPartOfGuidLength)
				return;

			var anchor = scintillaGateway.GetAnchor();
			var currentPos = scintillaGateway.GetCurrentPos();
			var absolutePosStart = Position.Min(anchor, currentPos);
			var lineNumber = scintillaGateway.LineFromPosition(absolutePosStart);
			var lineContent = scintillaGateway.GetLine(lineNumber);

			var absolutePosOfLine = scintillaGateway.PositionFromLine(lineNumber);
			var relativeLineOfset = (absolutePosStart - absolutePosOfLine).Value;

			var isLeftToRigthSelection = currentPos > anchor;

			switch (selectionLength)
			{
				case GuidHelperConstants.StartPartOfGuidLength:
					FirstPartOfGuidPossiblySelected(relativeLineOfset, lineContent, absolutePosOfLine, isLeftToRigthSelection, scintillaGateway, absolutePosStart);
					break;

				case GuidHelperConstants.EndPartOfGuidLength:
					LastPartOfGuidPossiblySelected(relativeLineOfset, lineContent, absolutePosStart, isLeftToRigthSelection, scintillaGateway);
					break;
			}
		}

		private void LastPartOfGuidPossiblySelected(int relativeLineOfset, string lineContent, Position absolutePosStart,
			bool isLeftToRigthSelection, IScintillaGateway scintillaGateway)
		{
			var isMatch = relativeLineOfset >= LengthGuidExceptLastPart
			              && lineContent.Length >= GuidHelperConstants.Regexlength
			              && GuidHelperConstants.GuidRex.IsMatch(lineContent, relativeLineOfset - LengthGuidExceptLastPart);
			if (!isMatch)
				return;

			var finalStartSelection = new Position(absolutePosStart.Value - LengthGuidExceptLastPart);
			var end = new Position(finalStartSelection.Value + GuidHelperConstants.Regexlength);

			if (isLeftToRigthSelection)
			{
				scintillaGateway.SetAnchor(finalStartSelection);
				scintillaGateway.SetCurrentPos(end);
			}
			else // "reverse" selection from right-to-left
			{
				scintillaGateway.SetAnchor(end);
				scintillaGateway.SetCurrentPos(finalStartSelection);
			}
		}

		private void FirstPartOfGuidPossiblySelected(int relativeLineOfset, string lineContent, Position absolutePosOfLine, bool isLeftToRigthSelection, 
			IScintillaGateway scintillaGateway, Position absolutePosStart)
		{
			var isMatch = lineContent.Length >= GuidHelperConstants.Regexlength + relativeLineOfset 
							&& GuidHelperConstants.GuidRex.IsMatch(lineContent, relativeLineOfset);
			if (!isMatch)
				return;

			var end = new Position(absolutePosOfLine.Value + relativeLineOfset + GuidHelperConstants.Regexlength);
			if (isLeftToRigthSelection)
			{
				scintillaGateway.SetAnchor(absolutePosStart);
				scintillaGateway.SetCurrentPos(end);
			}
			else // "reverse" selection from right-to-left
			{
				scintillaGateway.SetAnchor(end);
				scintillaGateway.SetCurrentPos(absolutePosStart);
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kbg.NppPluginNET;

namespace GuidHelper
{
	class InsertGuid
	{
		private readonly ScintillaGateway scintilla;

		public InsertGuid(ScintillaGateway scintilla)
		{
			this.scintilla = scintilla;
		}

		public void Execute()
		{
			scintilla.BeginUndoAction();
			var selections = GetSelections();
			var sumChanges = InsertGuids(selections);
			scintilla.EndUndoAction();

			var first = selections.First();
			var totalDelta = new Position(sumChanges + first.Item2);
			scintilla.GotoPos(first.Item1 + totalDelta);
		}

		private int InsertGuids(Tuple<Position, int>[] selections)
		{
			var sumChanges = selections.Sum(x =>
			{
				scintilla.DeleteRange(x.Item1, x.Item2);
				scintilla.InsertText(x.Item1, Guid.NewGuid().ToString());

				return GuidHelperConstants.Regexlength - x.Item2;
			});
			return sumChanges;
		}

		private Tuple<Position, int>[] GetSelections()
		{
			var selectionsCount = scintilla.GetSelections();
			var selections = new List<Tuple<Position, int>>(selectionsCount);
			for (int i = 0; i < selectionsCount; i++)
			{
				var selectionStart = scintilla.GetSelectionNStart(i);
				var delta = (scintilla.GetSelectionNEnd(i) - selectionStart).Value;

				selections.Add(Tuple.Create(selectionStart, delta));
			}

			return selections.OrderByDescending(x => x.Item1.Value).ToArray();
		}
	}
}

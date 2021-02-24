﻿using UnityEngine;

namespace Lean.Gui
{
	/// <summary>This component works just like LeanToggle, but it registers itself with the LeanWindowCloser.
	/// This allows the window to be automatically closed if you press the LeanWindowCloser.CloseKey.</summary>
	[HelpURL(LeanGui.HelpUrlPrefix + "LeanWindow")]
	[AddComponentMenu(LeanGui.ComponentMenuPrefix + "Window")]
	public class LeanWindow : LeanToggle
	{
		protected override void TurnOnNow()
		{
			base.TurnOnNow();

			LeanWindowCloser.Register(this);
		}
	}
}
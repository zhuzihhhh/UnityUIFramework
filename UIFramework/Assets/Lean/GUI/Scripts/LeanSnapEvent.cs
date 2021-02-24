﻿using UnityEngine;
using UnityEngine.Events;
using Lean.Common;

namespace Lean.Gui
{
	/// <summary>This component allows you to fire an events when the <b>LeanSnap</b> component's <b>Position</b> is at specific values.</summary>
	[ExecuteInEditMode]
	[RequireComponent(typeof(LeanSnap))]
	[HelpURL(LeanGui.HelpUrlPrefix + "LeanSnapEvent")]
	[AddComponentMenu(LeanGui.ComponentMenuPrefix + "Snap Event")]
	public class LeanSnapEvent : MonoBehaviour
	{
		/// <summary>The <b>LeanSnap.Position</b> you want to listen for.</summary>
		public Vector2Int Position { set { position = value; } get { return position; } } [SerializeField] private Vector2Int position;

		/// <summary>The action that will be invoked.</summary>
		public UnityEvent OnAction { get { if (onAction == null) onAction = new UnityEvent(); return onAction; } } [SerializeField] private UnityEvent onAction;
	}
}

#if UNITY_EDITOR
namespace Lean.Gui.Inspector
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanSnapEvent))]
	public class LeanSnapEvent_Inspector : LeanInspector<LeanSnapEvent>
	{
		protected override void DrawInspector()
		{
			Draw("position", "The LeanSnap.Position you want to listen for.");
			Draw("onAction");
		}
	}
}
#endif
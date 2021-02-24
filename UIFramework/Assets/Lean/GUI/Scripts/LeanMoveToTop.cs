﻿using UnityEngine;
using UnityEngine.EventSystems;
using Lean.Common;
using System.Collections.Generic;

namespace Lean.Gui
{
	/// <summary>This component will automatically move the specified transform to be the first sibling when you press down on this UI element.</summary>
	[HelpURL(LeanGui.HelpUrlPrefix + "LeanMoveToTop")]
	[AddComponentMenu(LeanGui.ComponentMenuPrefix + "Move To Top")]
	public class LeanMoveToTop : MonoBehaviour, IPointerDownHandler
	{
		public enum MoveType
		{
			ThisTransform,
			TargetTransform,
			ParentComponents
		}

		/// <summary>This allows you to choose which component to perform the action on.
		/// ThisTransform = transform.SetAsLastSibling().
		/// TargetTransform = Target.SetAsLastSibling().
		/// ParentComponents = Invoke all <b>LeanMoveToTop</b> ancestor components.</summary>
		public MoveType Move { set { move = value; } get { return move; } } [SerializeField] private MoveType move;

		/// <summary>If you want a different transform to be moved when pressing down on this UI element, then specify it here.
		/// None = The current GameObject's transform.</summary>
		public Transform Target { set { target = value; } get { return target; } } [SerializeField] private Transform target;

		private static List<LeanMoveToTop> tempOthers = new List<LeanMoveToTop>();

		public void OnPointerDown(PointerEventData eventData)
		{
			switch (move)
			{
				case MoveType.ThisTransform:
				{
					transform.SetAsLastSibling();
				}
				break;

				case MoveType.TargetTransform:
				{
					if (target != null)
					{
						target.SetAsLastSibling();
					}
				}
				break;

				case MoveType.ParentComponents:
				{
					GetComponentsInParent(false, tempOthers);

					foreach (var other in tempOthers)
					{
						if (other.move != MoveType.ParentComponents)
						{
							other.OnPointerDown(eventData);
						}
					}
				}
				break;
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Gui.Inspector
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanMoveToTop))]
	public class LeanMoveToTop_Inspector : LeanInspector<LeanMoveToTop>
	{
		protected override void DrawInspector()
		{
			Draw("move", "This allows you to choose which component to perform the action on.\n\nThisTransform = transform.SetAsLastSibling().\n\nTargetTransform = Target.SetAsLastSibling().\n\nParentComponents = Invoke all <b>LeanMoveToTop</b> ancestor components.");

			if (Any(t => t.Move == LeanMoveToTop.MoveType.TargetTransform))
			{
				EditorGUI.indentLevel++;
					BeginError(Any(t => t.Target == null));
						Draw("target", "If you want a different transform to be moved when pressing down on this UI element, then specify it here.\n\nNone = The current GameObject's transform.");
					EndError();
				EditorGUI.indentLevel--;
			}
		}
	}
}
#endif
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	/// <summary>This component allows you to detect when a specific amount of selectable objects in the scene have been selected.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanSelectedCount")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Selected Count")]
	public class LeanSelectedCount : MonoBehaviour
	{
		[System.Serializable] public class IntEvent : UnityEvent<int> {}

		/// <summary>When the amount of selected objects changes, this event is invoked with the current count.</summary>
		public IntEvent OnCount { get { if (onCount == null) onCount = new IntEvent(); return onCount; } } [SerializeField] private IntEvent onCount;

		/// <summary>The minimum amount of objects that must be selected for a match.
		/// -1 = Max.</summary>
		public int MatchMin = -1;

		/// <summary>The maximum amount of objects that can be selected for a match.
		/// -1 = Max.</summary>
		public int MatchMax = -1;

		/// <summary>When the amount of selected objects matches the <b>RequiredCount</b>, this event will be invoked.</summary>
		public UnityEvent OnMatch { get { if (onMatch == null) onMatch = new UnityEvent(); return onMatch; } } [SerializeField] private UnityEvent onMatch;

		/// <summary>When the amount of selected objects no longer matches the <b>RequiredCount</b>, this event will be invoked.</summary>
		public UnityEvent OnUnmatch { get { if (onUnmatch == null) onUnmatch = new UnityEvent(); return onUnmatch; } } [SerializeField] private UnityEvent onUnmatch;

		[SerializeField]
		private bool inside;

		protected virtual void OnEnable()
		{
			LeanSelectable.OnSelectGlobal   += HandleSelectGlobal;
			LeanSelectable.OnDeselectGlobal += HandleDeselectGlobal;
			LeanSelectable.OnDisableGlobal  += HandleDisableGlobal;
		}
		protected virtual void OnDisable()
		{
			LeanSelectable.OnSelectGlobal   -= HandleSelectGlobal;
			LeanSelectable.OnDeselectGlobal -= HandleDeselectGlobal;
			LeanSelectable.OnDisableGlobal  -= HandleDisableGlobal;
		}

		private void HandleSelectGlobal(LeanSelectable selectable, LeanFinger finger)
		{
			UpdateState();
		}

		private void HandleDeselectGlobal(LeanSelectable selectable)
		{
			UpdateState();
		}

		private void HandleDisableGlobal(LeanSelectable selectable)
		{
			UpdateState();
		}

		private void UpdateState()
		{
			var min       = MatchMin >= 0 ? MatchMin : LeanSelectable.Instances.Count;
			var max       = MatchMax >= 0 ? MatchMax : LeanSelectable.Instances.Count;
			var raw       = LeanSelectable.IsSelectedRawCount;
			var newInside = raw >= min && raw <= max;

			if (newInside != inside)
			{
				inside = newInside;

				if (inside == true)
				{
					if (onMatch != null)
					{
						onMatch.Invoke();
					}
				}
				else
				{
					if (onUnmatch != null)
					{
						onUnmatch.Invoke();
					}
				}
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Inspector
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanSelectedCount))]
	public class LeanSelectedCount_Inspector : Lean.Common.LeanInspector<LeanSelectedCount>
	{
		private bool showUnusedEvents;

		protected override void DrawInspector()
		{
			var usedA = Any(t => t.OnCount.GetPersistentEventCount() > 0);
			var usedB = Any(t => t.OnMatch.GetPersistentEventCount() > 0);
			var usedC = Any(t => t.OnUnmatch.GetPersistentEventCount() > 0);

			EditorGUI.BeginDisabledGroup(usedA && usedB && usedC);
				showUnusedEvents = EditorGUILayout.Foldout(showUnusedEvents, "Show Unused Events");
			EditorGUI.EndDisabledGroup();

			if (usedA == true || showUnusedEvents == true)
			{
				Draw("onCount");
			}

			if (usedB == true || usedC == true || showUnusedEvents == true)
			{
				Draw("MatchMin", "The minimum amount of objects that must be selected for a match.\n\n-1 = Max.");
				Draw("MatchMax", "The maximum amount of objects that can be selected for a match.\n\n-1 = Max.");
			}

			if (usedB == true || showUnusedEvents == true)
			{
				Draw("onMatch");
			}

			if (usedC == true || showUnusedEvents == true)
			{
				Draw("onUnmatch");
			}
		}
	}
}
#endif
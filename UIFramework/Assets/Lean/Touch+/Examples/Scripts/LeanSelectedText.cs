using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	/// <summary>This component allows you to display text showing the currently selected object count.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanSelectedText")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Selected Text")]
	public class LeanSelectedText : MonoBehaviour
	{
		[System.Serializable] public class StringEvent : UnityEvent<string> {}

		/// <summary>The format of the string.
		/// {0} = The amount of objects that can be selected.
		/// {1} = The amount of selected objects.
		/// {2} = The remaining objects to be selected.
		/// {3} = The percentage of selected objects.
		/// {4} = The percentage of objects remaining to be selected.</summary>
		public string Format { set { format = value; } get { return format; } } [SerializeField] private string format = "You have selected {1} out of {0} objects!";

		/// <summary>The formatted string will be output using this event.</summary>
		public StringEvent OnText { get { if (onText == null) onText = new StringEvent(); return onText; } } [SerializeField] private StringEvent onText;

		/// <summary>This method will immediately update the text.</summary>
		[ContextMenu("Update Now")]
		public void UpdateNow()
		{
			if (onText != null)
			{
				var dataA = LeanSelectable.Instances.Count;
				var dataB = LeanSelectable.IsSelectedRawCount;
				var dataC = dataA - dataB;
				var dataD = (dataB / (float)dataA) * 100;
				var dataE = (dataC / (float)dataA) * 100;

				onText.Invoke(string.Format(format, dataA, dataB, dataC, dataD, dataE));
			}
		}

		protected virtual void Update()
		{
			UpdateNow();
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Inspector
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanSelectedText))]
	public class LeanSelectedText_Inspector : Lean.Common.LeanInspector<LeanSelectedText>
	{
		protected override void DrawInspector()
		{
			Draw("format", "The format of the string.\n\n{0} = The amount of objects that can be selected.\n\n{1} = The amount of selected objects.\n\n{2} = The remaining objects to be selected.\n\n{3} = The percentage of selected objects.\n\n{4} = The percentage of objects remaining to be selected.");

			EditorGUILayout.Separator();

			Draw("onText");
		}
	}
}
#endif
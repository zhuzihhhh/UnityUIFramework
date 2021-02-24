using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	/// <summary>This component allows you to modify a value passed from an event and emit it again.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanFormatText")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Format Text")]
	public class LeanFormatText : MonoBehaviour
	{
		[System.Serializable] public class StringEvent : UnityEvent<string> {}

		/// <summary>The format of the string.</summary>
		public string Format { set { format = value; } get { return format; } } [SerializeField] private string format = "{0}";

		/// <summary>The modified value will be output from this event.</summary>
		public StringEvent OnFormatted { get { if (onFormatted == null) onFormatted = new StringEvent(); return onFormatted; } } [SerializeField] private StringEvent onFormatted;

		public void FormatFloat(float value)
		{
			if (onFormatted != null)
			{
				onFormatted.Invoke(string.Format(format, value));
			}
		}

		public void FormatInt(int value)
		{
			if (onFormatted != null)
			{
				onFormatted.Invoke(string.Format(format, value));
			}
		}

		public void FormatString(string value)
		{
			if (onFormatted != null)
			{
				onFormatted.Invoke(string.Format(format, value));
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Inspector
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanFormatText))]
	public class LeanFormatText_Inspector : Lean.Common.LeanInspector<LeanFormatText>
	{
		protected override void DrawInspector()
		{
			Draw("format", "The format of the string.");

			EditorGUILayout.Separator();

			Draw("onFormatted");
		}
	}
}
#endif
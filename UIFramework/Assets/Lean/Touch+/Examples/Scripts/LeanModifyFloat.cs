using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	/// <summary>This component allows you to modify a value passed from an event and emit it again.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanModifyFloat")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Modify Float")]
	public class LeanModifyFloat : MonoBehaviour
	{
		[System.Serializable] public class FloatEvent : UnityEvent<float> {}

		/// <summary>The value will be incremented by this.</summary>
		public float Offset { set { offset = value; } get { return offset; } } [SerializeField] private float offset;

		/// <summary>If you enable this then the delta values will be multiplied by Time.deltaTime. This allows you to maintain framerate independent actions.</summary>
		public bool ScaleByTime { set { scaleByTime = value; } get { return scaleByTime; } } [SerializeField] private bool scaleByTime;

		/// <summary>The value will be multiplied by this.</summary>
		public float Multiplier { set { multiplier = value; } get { return multiplier; } } [SerializeField] private float multiplier = 1.0f;

		/// <summary>The modified value will be output from this event.</summary>
		public FloatEvent OnModified { get { if (onModified == null) onModified = new FloatEvent(); return onModified; } } [SerializeField] private FloatEvent onModified;

		public void ModifyValue(float value)
		{
			if (onModified != null)
			{
				value += offset;
				value *= multiplier;

				onModified.Invoke(value);
			}
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Inspector
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanModifyFloat))]
	public class LeanModifyFloat_Inspector : Lean.Common.LeanInspector<LeanModifyFloat>
	{
		private bool showUnusedEvents;

		protected override void DrawInspector()
		{
			Draw("offset", "The value will be incremented by this.");
			Draw("scaleByTime", "If you enable this then the delta values will be multiplied by Time.deltaTime. This allows you to maintain framerate independent actions.");
			Draw("multiplier", "The value will be multiplied by this.");

			EditorGUILayout.Separator();

			Draw("onModified");
		}
	}
}
#endif
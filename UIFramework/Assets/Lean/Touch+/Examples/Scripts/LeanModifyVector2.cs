using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	/// <summary>This component allows you to modify a value passed from an event and emit it again.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanModifyVector2")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Modify Vector2")]
	public class LeanModifyVector2 : MonoBehaviour
	{
		public enum ModifyType
		{
			None,
			Normalize,
			Normalize4
		}

		[System.Serializable] public class Vector2Event : UnityEvent<Vector2> {}

		/// <summary>Should the swipe delta be modified before use?
		/// Normalize = The swipe delta magnitude/length will be set to 1.
		/// Normalize4 = The swipe delta will be + or - 1 on either the x or y axis.</summary>
		public ModifyType Modify { set { modify = value; } get { return modify; } } [SerializeField] private ModifyType modify;

		/// <summary>If you enable this then the delta values will be multiplied by Time.deltaTime. This allows you to maintain framerate independent actions.</summary>
		public bool ScaleByTime { set { scaleByTime = value; } get { return scaleByTime; } } [SerializeField] private bool scaleByTime;

		/// <summary>The value will be multiplied by this.</summary>
		public float Multiplier { set { multiplier = value; } get { return multiplier; } } [SerializeField] private float multiplier;

		/// <summary>The modified value will be output from this event.</summary>
		public Vector2Event OnModified { get { if (onModified == null) onModified = new Vector2Event(); return onModified; } } [SerializeField] private Vector2Event onModified;

		public void ModifyValue(Vector2 value)
		{
			if (onModified != null)
			{
				switch (Modify)
				{
					case ModifyType.Normalize:
					{
						value = value.normalized;
					}
					break;

					case ModifyType.Normalize4:
					{
						if (value.x < -Mathf.Abs(value.y)) value = -Vector2.right;
						if (value.x >  Mathf.Abs(value.y)) value =  Vector2.right;
						if (value.y < -Mathf.Abs(value.x)) value = -Vector2.up;
						if (value.y >  Mathf.Abs(value.x)) value =  Vector2.up;
					}
					break;
				}

				if (scaleByTime == true)
				{
					value *= Time.deltaTime;
				}

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
	[CustomEditor(typeof(LeanModifyVector2))]
	public class LeanModifyVector2_Inspector : Lean.Common.LeanInspector<LeanModifyVector2>
	{
		private bool showUnusedEvents;

		protected override void DrawInspector()
		{
			Draw("modify", "Should the swipe delta be modified before use?\n\nNormalize = The swipe delta magnitude/length will be set to 1.\n\nNormalize4 = The swipe delta will be + or - 1 on either the x or y axis.");
			Draw("scaleByTime", "If you enable this then the delta values will be multiplied by Time.deltaTime. This allows you to maintain framerate independent actions.");
			Draw("multiplier", "The value will be multiplied by this.");

			EditorGUILayout.Separator();

			Draw("onModified");
		}
	}
}
#endif
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Lean.Touch
{
	/// <summary>This component allows you to detect when the first finger begins touching the screen.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanFirstDown")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "First Down")]
	public class LeanFirstDown : MonoBehaviour
	{
		[System.Serializable] public class LeanFingerEvent : UnityEvent<LeanFinger> {}
		[System.Serializable] public class Vector3Event : UnityEvent<Vector3> {}
		[System.Serializable] public class Vector2Event : UnityEvent<Vector2> {}

		/// <summary>Ignore fingers with StartedOverGui?</summary>
		public bool IgnoreStartedOverGui = true;

		/// <summary>Do nothing if this LeanSelectable isn't selected?</summary>
		public LeanSelectable RequiredSelectable;

		/// <summary>This event will be called if the above conditions are met when you first touch the screen.</summary>
		public LeanFingerEvent OnFinger { get { if (onFinger == null) onFinger = new LeanFingerEvent(); return onFinger; } } [SerializeField] private LeanFingerEvent onFinger;

		/// <summary>The method used to find world coordinates from a finger. See LeanScreenDepth documentation for more information.</summary>
		public LeanScreenDepth ScreenDepth = new LeanScreenDepth(LeanScreenDepth.ConversionType.DepthIntercept);

		/// <summary>This event will be called if the above conditions are met when you first touch the screen.
		/// Vector3 = Finger position in world space.</summary>
		public Vector3Event OnWorld { get { if (onWorld == null) onWorld = new Vector3Event(); return onWorld; } } [SerializeField] private Vector3Event onWorld;

		/// <summary>This event will be called if the above conditions are met when you first touch the screen.
		/// Vector2 = Finger position in screen space.</summary>
		public Vector2Event OnScreen { get { if (onScreen == null) onScreen = new Vector2Event(); return onScreen; } } [SerializeField] private Vector2Event onScreen;

		[System.NonSerialized]
		private List<LeanFinger> fingers = new List<LeanFinger>();

#if UNITY_EDITOR
		protected virtual void Reset()
		{
			RequiredSelectable = GetComponentInParent<LeanSelectable>();
		}
#endif

		protected virtual void Awake()
		{
			if (RequiredSelectable == null)
			{
				RequiredSelectable = GetComponentInParent<LeanSelectable>();
			}
		}

		protected virtual void OnEnable()
		{
			LeanTouch.OnFingerDown += HandleFingerDown;
			LeanTouch.OnFingerUp   += HandleFingerUp;
		}

		protected virtual void OnDisable()
		{
			LeanTouch.OnFingerDown -= HandleFingerDown;
			LeanTouch.OnFingerUp   -= HandleFingerUp;
		}

		private void HandleFingerDown(LeanFinger finger)
		{
			if (IgnoreStartedOverGui == true && finger.IsOverGui == true)
			{
				return;
			}

			if (RequiredSelectable != null && RequiredSelectable.IsSelected == false)
			{
				return;
			}

			fingers.Add(finger);

			if (fingers.Count == 1)
			{
				if (onFinger != null)
				{
					onFinger.Invoke(finger);
				}

				if (onWorld != null)
				{
					var position = ScreenDepth.Convert(finger.StartScreenPosition, gameObject);

					onWorld.Invoke(position);
				}

				if (onScreen != null)
				{
					onScreen.Invoke(finger.ScreenPosition);
				}
			}
		}

		private void HandleFingerUp(LeanFinger finger)
		{
			fingers.Remove(finger);
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Inspector
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanFirstDown))]
	public class LeanFirstDown_Inspector : Lean.Common.LeanInspector<LeanFirstDown>
	{
		private bool showUnusedEvents;

		protected override void DrawInspector()
		{
			Draw("IgnoreStartedOverGui", "Ignore fingers with StartedOverGui?");
			Draw("RequiredSelectable", "Do nothing if this LeanSelectable isn't selected?");

			EditorGUILayout.Separator();

			var usedA = Any(t => t.OnFinger.GetPersistentEventCount() > 0);
			var usedB = Any(t => t.OnWorld.GetPersistentEventCount() > 0);
			var usedC = Any(t => t.OnScreen.GetPersistentEventCount() > 0);

			EditorGUI.BeginDisabledGroup(usedA && usedB && usedC);
				showUnusedEvents = EditorGUILayout.Foldout(showUnusedEvents, "Show Unused Events");
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.Separator();

			if (usedA == true || showUnusedEvents == true)
			{
				Draw("onFinger");
			}

			if (usedB == true || showUnusedEvents == true)
			{
				Draw("ScreenDepth");
				Draw("onWorld");
			}

			if (usedC == true || showUnusedEvents == true)
			{
				Draw("onScreen");
			}
		}
	}
}
#endif
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using Lean.Common;
using FSA = UnityEngine.Serialization.FormerlySerializedAsAttribute;

namespace Lean.Touch
{
	/// <summary>This script allows you to twist the selected object around like a dial or knob.</summary>
	[ExecuteInEditMode]
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanSelectableDial")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Selectable Dial")]
	public class LeanSelectableDial : LeanSelectableBehaviour
	{
		[System.Serializable]
		public class Trigger
		{
			[Tooltip("The central Angle of this trigger in degrees.")]
			public float Angle;

			[Tooltip("The angle range of this trigger in degrees.\n\n90 = Quarter circle.\n180 = Half circle.")]
			public float Arc;

			[HideInInspector]
			public bool Inside;

			public UnityEvent OnEnter { get { if (onEnter == null) onEnter = new UnityEvent(); return onEnter; } } [SerializeField] private UnityEvent onEnter;

			public UnityEvent OnExit { get { if (onExit == null) onExit = new UnityEvent(); return onExit; } } [SerializeField] private UnityEvent onExit;

			public bool IsInside(float angle, bool clamp)
			{
				var range = Arc * 0.5f;

				if (clamp == false)
				{
					var delta  = Mathf.Abs(Mathf.DeltaAngle(Angle, angle));

					return delta < range;
				}

				return angle >= Angle - range && angle <= Angle + range;
			}
		}

		[System.Serializable] public class FloatEvent : UnityEvent<float> {}

		/// <summary>The camera we will be used.
		/// None = MainCamera.</summary>
		public Camera Camera;

		/// <summary>The base rotation in local space.</summary>
		public Vector3 Tilt;

		/// <summary>The axis of the rotation in local space.</summary>
		public Vector3 Axis = Vector3.up;

		/// <summary>The angle of the dial in degrees.</summary>
		public float Angle { set { var newAngle = value; if (Clamp == true) { newAngle = Mathf.Clamp(newAngle, ClampMin, ClampMax); } if (angle != newAngle) { angle = newAngle; if (onAngleChanged != null) onAngleChanged.Invoke(angle); } } get { return angle; } } [FSA("Angle")] [SerializeField] private float angle;

		/// <summary>Should the Angle value be clamped?</summary>
		public bool Clamp;

		/// <summary>The minimum Angle value.</summary>
		public float ClampMin = -45.0f;

		/// <summary>The maximum Angle value.</summary>
		public float ClampMax = 45.0f;

		/// <summary>This allows you to perform a custom event when the dial is within a specifid angle range.</summary>
		public List<Trigger> Triggers;

		/// <summary>This event is invoked when the <b>Angle</b> changes.
		/// Float = Current Angle.</summary>
		public FloatEvent OnAngleChanged { get { if (onAngleChanged == null) onAngleChanged = new FloatEvent(); return onAngleChanged; } } [SerializeField] private FloatEvent onAngleChanged;

		private Vector2 oldPoint;

		private bool oldPointSet;

		/// <summary>This method allows you to increase the <b>Angle</b> value from an external event (e.g. UI button click).</summary>
		public void IncrementAngle(float delta)
		{
			Angle += delta;
		}

#if UNITY_EDITOR
		protected virtual void OnDrawGizmosSelected()
		{
			Gizmos.DrawLine(transform.position, transform.TransformPoint(Axis));
		}
#endif

		protected virtual void Update()
		{
			var newAngle = angle;

			// Reset rotation and get axis
			transform.localEulerAngles = Tilt;

			// Is this GameObject selected?
			if (Selectable != null && Selectable.IsSelected == true)
			{
				// Does it have a selected finger?
				var finger = Selectable.SelectingFinger;

				if (finger != null)
				{
					var newPoint = GetPoint(finger.ScreenPosition);

					if (oldPointSet == true)
					{
						newAngle -= Vector2.SignedAngle(newPoint, oldPoint);
					}

					oldPoint    = newPoint;
					oldPointSet = true;
				}
			}
			else
			{
				oldPointSet = false;
			}

			if (Clamp == true)
			{
				newAngle = Mathf.Clamp(newAngle, ClampMin, ClampMax);
			}

			transform.Rotate(Axis, angle, Space.Self);

			if (Triggers != null)
			{
				for (var i = 0; i < Triggers.Count; i++)
				{
					var trigger = Triggers[i];

					if (trigger.IsInside(angle, Clamp) == true)
					{
						if (trigger.Inside == false)
						{
							trigger.Inside = true;

							trigger.OnEnter.Invoke();
						}
					}
					else
					{
						if (trigger.Inside == true)
						{
							trigger.Inside = false;

							trigger.OnExit.Invoke();
						}
					}
				}
			}

			Angle = newAngle;
		}

		private Vector2 GetPoint(Vector2 screenPoint)
		{
			// Make sure the camera exists
			var camera = LeanHelper.GetCamera(Camera, gameObject);

			if (camera != null)
			{
				var rectTransform = transform as RectTransform;

				if (rectTransform != null)
				{
					var worldPoint = default(Vector3);

					if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, camera, out worldPoint) == true)
					{
						return Quaternion.LookRotation(Axis) * transform.InverseTransformPoint(worldPoint);
					}
				}
				else
				{
					var ray      = camera.ScreenPointToRay(screenPoint);
					var plane    = new Plane(transform.TransformDirection(Axis), transform.position);
					var distance = default(float);

					if (plane.Raycast(ray, out distance) == true)
					{
						return Quaternion.Inverse(Quaternion.LookRotation(Axis)) * transform.InverseTransformPoint(ray.GetPoint(distance));
					}
				}
			}
			else
			{
				Debug.LogError("Failed to find camera. Either tag your cameras MainCamera, or set one in this component.", this);
			}

			return oldPoint;
		}
	}
}

#if UNITY_EDITOR
namespace Lean.Touch.Inspector
{
	using UnityEditor;

	[CanEditMultipleObjects]
	[CustomEditor(typeof(LeanSelectableDial))]
	public class LeanSelectableDial_Inspector : Lean.Common.LeanInspector<LeanSelectableDial>
	{
		protected override void DrawInspector()
		{
			Draw("Camera", "The camera we will be used.\n\nNone = MainCamera.");
			Draw("Tilt", "The base rotation in local space.");
			Draw("Axis", "The axis of the rotation in local space.");
			Draw("angle", "The angle of the dial in degrees.");

			EditorGUILayout.Separator();

			Draw("Clamp", "Should the Angle value be clamped?");
			EditorGUI.indentLevel++;
				Draw("ClampMin", "The minimum Angle value.", "Min");
				Draw("ClampMax", "The maximum Angle value.", "Max");
			EditorGUI.indentLevel--;

			EditorGUILayout.Separator();

			Draw("Triggers", "This allows you to perform a custom event when the dial is within a specifid angle range.");

			EditorGUILayout.Separator();

			Draw("onAngleChanged");
		}
	}
}
#endif
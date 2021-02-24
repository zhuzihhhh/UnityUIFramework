using UnityEngine;
using System.Collections.Generic;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the specified Transform.localScale to the target value.</summary>
	[HelpURL(LeanTransition.HelpUrlPrefix + "LeanTransformLocalScale")]
	[AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Transform/Transform.localScale" + LeanTransition.MethodsMenuSuffix + "(LeanTransformLocalScale)")]
	public class LeanTransformLocalScale : LeanMethodWithStateAndTarget
	{
		public enum StyleType
		{
			Replace,
			Multiply,
			Increment
		}

		public override System.Type GetTargetType()
		{
			return typeof(Transform);
		}

		public override void Register()
		{
			PreviousState = Register(GetAliasedTarget(Data.Target), Data.Scale, Data.Duration, Data.Ease);
		}

		public static LeanState Register(Transform target, Vector3 scale, float duration, LeanEase ease = LeanEase.Smooth)
		{
			var state = LeanTransition.SpawnWithTarget(State.Pool, target);

			state.Scale = scale;
			state.Ease  = ease;

			return LeanTransition.Register(state, duration);
		}

		[System.Serializable]
		public class State : LeanStateWithTarget<Transform>
		{
			[Tooltip("The scale we will transition to.")]
			public Vector3 Scale = Vector3.one;

			[Tooltip("The ease method that will be used for the transition.")]
			public LeanEase Ease = LeanEase.Smooth;

			[Tooltip("Replace = The localScale value will transition to the Scale value.\n\nMultiply = The localScale value will transition to the localScale*Scale value.\n\nIncrement = The localScale value will transition to the localScale+Scale value.")]
			public StyleType Style;

			[System.NonSerialized] private Vector3 oldScale;

			public override int CanFill
			{
				get
				{
					return Target != null && Target.localScale != Scale ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				Scale = Target.localScale;
			}

			public override void BeginWithTarget()
			{
				oldScale = Target.localScale;
			}

			public override void UpdateWithTarget(float progress)
			{
				var finalScale = Scale;

				switch (Style)
				{
					case StyleType.Multiply : finalScale.x *= oldScale.x; finalScale.y *= oldScale.y; finalScale.z *= oldScale.z; break;
					case StyleType.Increment: finalScale.x += oldScale.x; finalScale.y += oldScale.y; finalScale.z += oldScale.z; break;
				}

				Target.localScale = Vector3.LerpUnclamped(oldScale, finalScale, Smooth(Ease, progress));
			}

			public static Stack<State> Pool = new Stack<State>(); public override void Despawn() { Pool.Push(this); }
		}

		public State Data;
	}
}

namespace Lean.Transition
{
	public static partial class LeanExtensions
	{
		public static Transform localScaleTransition(this Transform target, Vector3 scale, float duration, LeanEase ease = LeanEase.Smooth, Method.LeanTransformLocalScale.StyleType style = Method.LeanTransformLocalScale.StyleType.Replace)
		{
			Method.LeanTransformLocalScale.Register(target, scale, duration, ease); return target;
		}
	}
}
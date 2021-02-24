using UnityEngine;
using System.Collections.Generic;

namespace Lean.Transition.Method
{
	/// <summary>This component allows you to transition the specified Transform.localScale.x to the target value.</summary>
	[HelpURL(LeanTransition.HelpUrlPrefix + "LeanTransformLocalScaleX")]
	[AddComponentMenu(LeanTransition.MethodsMenuPrefix + "Transform/Transform.localScale.x" + LeanTransition.MethodsMenuSuffix + "(LeanTransformLocalScaleX)")]
	public class LeanTransformLocalScaleX : LeanMethodWithStateAndTarget
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

		public static LeanState Register(Transform target, float scale, float duration, LeanEase ease = LeanEase.Smooth)
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
			public float Scale;

			[Tooltip("The ease method that will be used for the transition.")]
			public LeanEase Ease = LeanEase.Smooth;

			[Tooltip("Replace = The localScale value will transition to the Scale value.\n\nMultiply = The localScale value will transition to the localScale*Scale value.\n\nIncrement = The localScale value will transition to the localScale+Scale value.")]
			public StyleType Style;

			[System.NonSerialized] private float oldScale;

			public override int CanFill
			{
				get
				{
					return Target != null && Target.localScale.x != Scale ? 1 : 0;
				}
			}

			public override void FillWithTarget()
			{
				Scale = Target.localScale.x;
			}

			public override void BeginWithTarget()
			{
				oldScale = Target.localScale.x;
			}

			public override void UpdateWithTarget(float progress)
			{
				var localScale = Target.localScale;
				var finalScale = Scale;

				switch (Style)
				{
					case StyleType.Multiply : finalScale *= oldScale; break;
					case StyleType.Increment: finalScale += oldScale; break;
				}

				localScale.x = Mathf.LerpUnclamped(oldScale, finalScale, Smooth(Ease, progress));

				Target.localScale = localScale;
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
		public static Transform localScaleTransition_X(this Transform target, float scale, float duration, LeanEase ease = LeanEase.Smooth)
		{
			Method.LeanTransformLocalScaleX.Register(target, scale, duration, ease); return target;
		}
	}
}
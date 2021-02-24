using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	/// <summary>This component allows you to detect when any selectable object in the scene has been selected.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanSelected")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Selected")]
	public class LeanSelected : MonoBehaviour
	{
		[System.Serializable] public class LeanSelectableEvent : UnityEvent<LeanSelectable> {}

		public LeanSelectableEvent OnSelectable { get { if (onSelectable == null) onSelectable = new LeanSelectableEvent(); return onSelectable; } } [SerializeField] private LeanSelectableEvent onSelectable;

		protected virtual void OnEnable()
		{
			LeanSelectable.OnSelectGlobal += HandleSelectGlobal;
		}
		protected virtual void OnDisable()
		{
			LeanSelectable.OnSelectGlobal -= HandleSelectGlobal;
		}

		private void HandleSelectGlobal(LeanSelectable selectable, LeanFinger finger)
		{
			if (onSelectable != null)
			{
				onSelectable.Invoke(selectable);
			}
		}
	}
}
using UnityEngine;
using UnityEngine.Events;

namespace Lean.Touch
{
	/// <summary>This component allows you to detect when any selectable object in the scene has been deselected.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanDeselected")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Deselected")]
	public class LeanDeselected : MonoBehaviour
	{
		[System.Serializable] public class LeanSelectableEvent : UnityEvent<LeanSelectable> {}

		public LeanSelectableEvent OnSelectable { get { if (onSelectable == null) onSelectable = new LeanSelectableEvent(); return onSelectable; } } [SerializeField] private LeanSelectableEvent onSelectable;

		protected virtual void OnEnable()
		{
			LeanSelectable.OnDeselectGlobal += HandleDeselectGlobal;
		}
		protected virtual void OnDisable()
		{
			LeanSelectable.OnDeselectGlobal -= HandleDeselectGlobal;
		}

		private void HandleDeselectGlobal(LeanSelectable selectable)
		{
			if (onSelectable != null)
			{
				onSelectable.Invoke(selectable);
			}
		}
	}
}

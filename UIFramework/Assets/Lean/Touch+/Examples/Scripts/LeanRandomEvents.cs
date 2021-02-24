using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Lean.Touch
{
	/// <summary>This component allows you to randomly invoke one of the specified events when you manually call the <b>Invoke</b> method.</summary>
	[HelpURL(LeanTouch.PlusHelpUrlPrefix + "LeanRandomEvents")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Random Events")]
	public class LeanRandomEvents : MonoBehaviour
	{
		public List<UnityEvent> Events { get { if (events == null) events = new List<UnityEvent>(); return events; } } [SerializeField] private List<UnityEvent> events;

		[ContextMenu("Invoke")]
		public void Invoke()
		{
			if (events != null && events.Count > 0)
			{
				var index   = Random.Range(0, events.Count);
				var element = events[index];

				if (element != null)
				{
					element.Invoke();
				}
			}
		}
	}
}
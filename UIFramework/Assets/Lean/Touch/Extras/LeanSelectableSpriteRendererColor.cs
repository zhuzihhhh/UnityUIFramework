using UnityEngine;

namespace Lean.Touch
{
	/// <summary>This component allows you to change the color of the SpriteRenderer attached to the current GameObject when selected.</summary>
	[RequireComponent(typeof(SpriteRenderer))]
	[HelpURL(LeanTouch.HelpUrlPrefix + "LeanSelectableSpriteRendererColor")]
	[AddComponentMenu(LeanTouch.ComponentPathPrefix + "Selectable SpriteRenderer Color")]
	public class LeanSelectableSpriteRendererColor : LeanSelectableBehaviour
	{
		/// <summary>Automatically read the DefaultColor from the SpriteRenderer?</summary>
		[Tooltip("Automatically read the DefaultColor from the SpriteRenderer?")]
		public bool AutoGetDefaultColor;

		/// <summary>The default color given to the SpriteRenderer.</summary>
		[Tooltip("The default color given to the SpriteRenderer.")]
		public Color DefaultColor = Color.white;

		/// <summary>The color given to the SpriteRenderer when selected.</summary>
		[Tooltip("The color given to the SpriteRenderer when selected.")]
		public Color SelectedColor = Color.green;

		protected virtual void Awake()
		{
			if (AutoGetDefaultColor == true)
			{
				var spriteRenderer = GetComponent<SpriteRenderer>();

				DefaultColor = spriteRenderer.color;
			}
		}

		protected override void OnSelect(LeanFinger finger)
		{
			ChangeColor(SelectedColor);
		}

		protected override void OnDeselect()
		{
			ChangeColor(DefaultColor);
		}

		private void ChangeColor(Color color)
		{
			var spriteRenderer = GetComponent<SpriteRenderer>();

			spriteRenderer.color = color;
		}
	}
}
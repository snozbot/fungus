using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Sprite", 
	             "Set Mouse Cursor", 
	             "Sets the mouse cursor sprite.")]
	[AddComponentMenu("")]
	public class SetMouseCursor : Command 
	{
		[Tooltip("Texture to use for cursor. Will use default mouse cursor if no sprite is specified")]
		public Texture2D cursorTexture;

		[Tooltip("The offset from the top left of the texture to use as the target point")]
		public Vector2 hotSpot;

		// Cached static cursor settings
		protected static Texture2D activeCursorTexture;
		protected static Vector2 activeHotspot;

	 	public static void ResetMouseCursor()
		{
			// Change mouse cursor back to most recent settings
			Cursor.SetCursor(activeCursorTexture, activeHotspot, CursorMode.Auto);
		}

		public override void OnEnter()
		{
			Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);

			activeCursorTexture = cursorTexture;
			activeHotspot = hotSpot;

			Continue();
		}

		public override string GetSummary()
		{
			if (cursorTexture == null)
			{
				return "Error: No cursor sprite selected";
			}

			return cursorTexture.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}
}
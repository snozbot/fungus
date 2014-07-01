using UnityEngine;
using System.Collections;

namespace Fungus
{
	/**
	 * Draws a GUI button at a consistent size regardless of screen resolution.
	 * The button can be positioned anywhere on the screen using normalized screen coords.
	 * Vertical and horizontal padding can be applied to offset the button away from the screen edge.
	 * Several options are available for handling the player click action.
	 */
	public class GUIButton : MonoBehaviour 
	{
		/// Button texture to draw on the screen.
		[Tooltip("Button texture to draw on the screen.")]
		public Texture2D texture;	

		/// Button size as a fraction of screen height [0..1].
		[Range(0, 1)]
		[Tooltip("Button size as a fraction of screen height [0..1].")]
		public float verticalScale = 0.2f; 

		/// Texture position on screen in localized screen coords [0..1]
		[Tooltip("Texture position on screen in localized screen coords [0..1]")]
		public Vector2 screenPosition; 

		/// Space between edge of screen and texture (in pixels).
		[Tooltip("Space between edge of screen and texture (in pixels).")]
		public Vector2 padding;

		/// Supported actions to perform when player clicks the button.
		public enum ClickAction
		{
			/// Perform no action, useful when you just want to display a sprite in screen space.
			None,
			/// Open the URL specified in actionParameter in the browser
			OpenURL,
			/// Call a method specified by actionParameter on all Room objects in the scene.
			SendMessage
		};

		/// Action to perform when player clicks the button.
		[Tooltip("Action to perform when player clicks the button.")]
		public ClickAction clickAction;

		/// Parameter associated with the click action.
		[Tooltip("Parameter associated with the click action.")]
		public string actionParameter;

		void OnGUI()
		{
			if (texture == null)
			{
				return;
			}

			// Calc initial center point
			float x = screenPosition.x * Screen.width;
			float y = screenPosition.y * Screen.height;

			// Height is calculated as a fraction of screen height for resolution independent sizing.
			// Width is then calculated so as to maintain the original aspect ratio of the texture.
			float height = Screen.height * verticalScale;
			float width = texture.width * (height / texture.height);

			// Calc initial rect for rendering texture 
			float x1 = x - width / 2f;
			float y1 = y - height / 2f;
			float x2 = x + width / 2f;
			float y2 = y + height / 2f;

			// Adjust rect to fit on screen, and apply vertical & horizontal padding
			if (x1 < padding.x)
			{
				x1 = padding.x;
				x2 = x1 + width;
			}
			if (x2 > Screen.width - padding.x)
			{
				x2 = Screen.width - padding.x;
				x1 = x2 - width;
			}
			if (y1 < padding.y)
			{
				y1 = padding.y;
				y2 = y1 + height;
			}
			if (y2 > Screen.height - padding.y)
			{
				y2 = Screen.height - padding.y;
				y1 = y2 - height;
			}

			// Draw the texture
			Rect textureRect = new Rect(x1, y1, x2 - x1, y2 - y1);
			if (GUI.Button(textureRect, texture, new GUIStyle()))
			{
				switch (clickAction)
				{
				case ClickAction.OpenURL:
					Application.OpenURL(actionParameter);
					break;
				case ClickAction.SendMessage:
					// Send the message to all Room objects in the scene
					Room[] allRooms = GameObject.FindObjectsOfType<Room>();
					foreach (Room room in allRooms)
					{
						room.SendMessage(actionParameter, SendMessageOptions.DontRequireReceiver);
					}
					break;
				}
			}
		}
	}
}

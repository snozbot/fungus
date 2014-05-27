using UnityEngine;
using System.Collections;

/**
 * Draws a GUI texture at a consistent size regardless of screen resolution.
 * The texture can be positioned anywhere on the screen using normalized screen coords.
 * Vertical and horizontal padding can be applied to offset the texture away from the screen edge.
 */
public class AnchorGUITexture : MonoBehaviour 
{
	/// Texture to draw on the screen.
	public Texture2D texture;	

	/// Fraction of screen height (for resolution independent sizing).
	public float verticalScale = 0.2f; 

	/// Texture position on screen in localized screen coords ([0..1], [0..1])
	public Vector2 screenPosition; 

	/// Vertical and horizontal space between edge of screen and texture (in pixels).
	public Vector2 padding;
	
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
		GUI.DrawTexture(textureRect, texture);
	}
}

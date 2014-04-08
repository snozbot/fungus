using UnityEngine;
using System.Collections;

public class ContinueStyle : MonoBehaviour 
{
	/**
	 * Text to use on 'Continue' buttons.
	 */
	public string continueText = "Continue";

	/// Continue font size as a fraction of screen height.
	public float continueFontScale = 1f / 30f;

	/// Style for continue button
	public GUIStyle style;

	/**
	 * Specifies continue button position in normalized screen coordinates.
	 * (0,0) is top left of screen.
	 * (1,1) is bottom right of screen
	 */
	public Vector2 screenPosition = new Vector2(1,1);

	/**
	 * Padding distance between button and edge of the screen in pixels.
	 */
	public Vector2 padding = new Vector2(4,4);

	/**
	 * Returns the style for the Continue button.
	 * Overrides the font size to compensate for varying device resolution.
	 * Font size is calculated as a fraction of the current screen height.
	 */
	public GUIStyle GetScaledContinueStyle()
	{
		GUIStyle guiStyle;
		guiStyle = new GUIStyle(style);
		guiStyle.fontSize = Mathf.RoundToInt((float)Screen.height * continueFontScale);
		return guiStyle;
	}

	public Rect CalcContinueRect()
	{
		GUIStyle continueStyle = GetScaledContinueStyle();
		
		GUIContent content = new GUIContent(continueText);
		Vector2 size = continueStyle.CalcSize(content);
		
		float x = Screen.width * screenPosition.x;
		float y = Screen.height * screenPosition.y;
		float width = size.x;
		float height = size.y;

		x = Mathf.Max(x, padding.x);
		x = Mathf.Min(x, Screen.width - width - padding.x); 

		y = Mathf.Max(y, padding.y);
		y = Mathf.Min(y, Screen.height - height - padding.y); 

		return new Rect(x, y, width, height);
	}
}

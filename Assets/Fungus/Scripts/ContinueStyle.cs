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
	 * If true, places the continue button on the active page.
	 * If false, places the continue button on the screen.
	 */
	public bool onPage;

	/**
	 * Specifies continue button position in normalized screen coordinates.
	 * This setting is ignored if onPage == true
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
}

using UnityEngine;
using System.Collections;

namespace Fungus
{
	/**
	 * Defines visual appearance of a Page.
	 * Usage: 
	 * 1. Add this component to an empty game object
	 * 2. Customize the style properties
	 * 3. Assign the style object to the pageStyle property of the Page you wish to style
	 */
	public class PageStyle : MonoBehaviour 
	{
		// The font size for title, say and option text is calculated by multiplying the screen height
		// by the corresponding font scale. Text appears the same size across all device resolutions.
		
		/// Title font size as a fraction of screen height.
		public float titleFontScale = 1f / 20f;

		/// Say font size as a fraction of screen height.
		public float sayFontScale = 1f / 25f;

		/// Option font size as a fraction of screen height.
		public float optionFontScale = 1f / 25f;

		/// Style for title text
		public GUIStyle titleStyle;

		/// Style for say text
		public GUIStyle sayStyle;

		/// Style for option text
		public GUIStyle optionStyle;

		/// Style for text box
		public GUIStyle boxStyle;

		void Update()
		{
			// Override the font size to compensate for varying device resolution
			// Font size is calculated as a fraction of the current screen height
			titleStyle.fontSize = Mathf.RoundToInt((float)Screen.height * titleFontScale);
			sayStyle.fontSize = Mathf.RoundToInt((float)Screen.height * sayFontScale);
			optionStyle.fontSize = Mathf.RoundToInt((float)Screen.height * optionFontScale);
		}
	}
}
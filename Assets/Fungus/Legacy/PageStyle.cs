using UnityEngine;
using System.Collections;

namespace Fungus
{
	/**
	 * Defines visual appearance of a Page.
	 * The Game.activePageStyle property controls the visual appearance of the displayed Page
	 */
	public class PageStyle : MonoBehaviour 
	{
		// The font size for title, say and option text is calculated by multiplying the screen height
		// by the corresponding font scale. Text appears the same size across all device resolutions.
		
		/// Header font size as a fraction of screen height.
		public float headerFontScale = 1f / 20f;

		/// Say font size as a fraction of screen height.
		public float sayFontScale = 1f / 25f;

		/// Header font size as a fraction of screen height.
		public float footerFontScale = 1f / 20f;

		/// Option font size as a fraction of screen height.
		public float optionFontScale = 1f / 25f;

		/// Style for header text
		public GUIStyle headerStyle;

		/// Style for header text
		public GUIStyle footerStyle;

		/// Style for say text
		public GUIStyle sayStyle;

		/// Style for option text
		public GUIStyle optionStyle;

		/// Style for option text (alternate rows)
		public GUIStyle optionAlternateStyle;

		/// Style for text box
		public GUIStyle boxStyle;

		/**
		 * Returns the style for Header text.
		 * Overrides the font size to compensate for varying device resolution.
		 * Font size is calculated as a fraction of the current screen height.
		 */
		public GUIStyle GetScaledHeaderStyle()
		{
			GUIStyle style = new GUIStyle(headerStyle);
			style.fontSize = Mathf.RoundToInt((float)Screen.height * headerFontScale);
			return style;
		}

		/**
		 * Returns the style for SetFooter text.
		 * Overrides the font size to compensate for varying device resolution.
		 * Font size is calculated as a fraction of the current screen height.
		 */
		public GUIStyle GetScaledFooterStyle()
		{
			GUIStyle style = new GUIStyle(footerStyle);
			style.fontSize = Mathf.RoundToInt((float)Screen.height * footerFontScale);
			return style;
		}

		/**
		 * Returns the style for Say text.
		 * Overrides the font size to compensate for varying device resolution.
		 * Font size is calculated as a fraction of the current screen height.
		 */
		public GUIStyle GetScaledSayStyle()
		{
			GUIStyle style = new GUIStyle(sayStyle);
			style.fontSize = Mathf.RoundToInt((float)Screen.height * sayFontScale);
			return style;
		}

		/**
		 * Returns the style for Option buttons.
		 * Overrides the font size to compensate for varying device resolution.
		 * Font size is calculated as a fraction of the current screen height.
		 */
		public GUIStyle GetScaledOptionStyle()
		{
			GUIStyle style;
			style = new GUIStyle(optionStyle);
			style.fontSize = Mathf.RoundToInt((float)Screen.height * optionFontScale);
			return style;
		}

		/**
		 * Returns the alternate style for Option buttons.
		 * This can be used to create alternating color rows.
		 * Overrides the font size to compensate for varying device resolution.
		 * Font size is calculated as a fraction of the current screen height.
		 */
		public GUIStyle GetScaledOptionAlternateStyle()
		{
			GUIStyle style;
			style = new GUIStyle(optionAlternateStyle);
			style.fontSize = Mathf.RoundToInt((float)Screen.height * optionFontScale);
			return style;
		}
	}
}
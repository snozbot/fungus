/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.Internal;
using UnityEditor;
using System;
using System.Reflection;

namespace Fungus
{

	/**
	 * Utility functions for drawing custom UI in the editor
	 */
	public static class CustomGUI
	{	
		public static Texture2D CreateBlackTexture()
		{
			Texture2D blackTex = new Texture2D(1,2);
			blackTex.SetPixel(0, 0, Color.gray);
			blackTex.SetPixel(1, 0, Color.black);
			blackTex.Apply();
			blackTex.hideFlags = HideFlags.DontSave;
			return blackTex;
		}

		public static Texture2D CreateColorTexture(Color color)
		{
			Texture2D colorTex = new Texture2D(1,1);
			colorTex.SetPixel(0, 0, color);
			colorTex.Apply();
			colorTex.hideFlags = HideFlags.DontSave;
			return colorTex;
		}
		
		public static void DrawLineSeparator(Texture2D blackTex, float width)
		{
			GUIStyle separatorStyle = new GUIStyle(GUI.skin.box);
			separatorStyle.fixedWidth = width;
			separatorStyle.fixedHeight = 2;
			separatorStyle.margin.top = 10;
			separatorStyle.margin.bottom = 10;
			GUILayout.Box(blackTex,separatorStyle);
		}
	}

}
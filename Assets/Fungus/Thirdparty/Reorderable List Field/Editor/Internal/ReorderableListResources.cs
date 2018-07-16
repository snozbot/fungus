// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System;
using UnityEditor;
using UnityEngine;

namespace Rotorz.ReorderableList.Internal {

	/// <exclude/>
	public enum ReorderableListTexture {
		Icon_Add_Normal = 0,
		Icon_Add_Active,
		Icon_AddMenu_Normal,
		Icon_AddMenu_Active,
		Icon_Menu_Normal,
		Icon_Menu_Active,
		Icon_Remove_Normal,
		Icon_Remove_Active,
		Button_Normal,
		Button_Active,
		Button2_Normal,
		Button2_Active,
		TitleBackground,
		ContainerBackground,
		Container2Background,
		GrabHandle,
	}

	/// <summary>
	/// Resources to assist with reorderable list control.
	/// </summary>
	/// <exclude/>
	public static class ReorderableListResources {

		static ReorderableListResources() {
			GenerateSpecialTextures();
			LoadResourceAssets();
		}

		#region Texture Resources

		/// <summary>
		/// Resource assets for light skin.
		/// </summary>
		/// <remarks>
		/// <para>Resource assets are PNG images which have been encoded using a base-64
		/// string so that actual asset files are not necessary.</para>
		/// </remarks>
		private static string[] s_LightSkin = {
			"iVBORw0KGgoAAAANSUhEUgAAAAgAAAAICAYAAADED76LAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAACxJREFUeNpi/P//PwMM6OvrgzkXL15khIkxMRAABBUw6unp/afMBNo7EiDAAEKeD5EsXZcTAAAAAElFTkSuQmCC",
			"iVBORw0KGgoAAAANSUhEUgAAAAgAAAAICAYAAADED76LAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAC1JREFUeNpi/P//PwMM3L17F8xRVlZmhIkxMRAABBUw3rlz5z9lJtDekQABBgCvqxGbQWpEqwAAAABJRU5ErkJggg==",
			"iVBORw0KGgoAAAANSUhEUgAAABYAAAAICAYAAAD9aA/QAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAERJREFUeNpi/P//PwMxQF9fH6zw4sWLjMSoZ2KgEaCZwYz4ggLmfVwAX7AMjIuJjTxsPqOKi9EtA/GpFhQww2E0QIABAPF5IGHNU7adAAAAAElFTkSuQmCC",
			"iVBORw0KGgoAAAANSUhEUgAAABYAAAAICAYAAAD9aA/QAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAERJREFUeNpi/P//PwMx4O7du2CFysrKjMSoZ2KgEaCZwYz4ggLmfVwAX7AMjIuJjTxsPqOKi9EtA/GpFhQww2E0QIABACBuGkOOEiPJAAAAAElFTkSuQmCC",
			"iVBORw0KGgoAAAANSUhEUgAAAAUAAAAICAYAAAAx8TU7AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAADBJREFUeNpi/P//PwM6YGLAAigUZNHX18ewienixYuMyAJgPshJIKynp/cfxgYIMACCMhb+oVNPwwAAAABJRU5ErkJggg==",
			"iVBORw0KGgoAAAANSUhEUgAAAAUAAAAICAYAAAAx8TU7AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAADFJREFUeNpi/P//PwM6YGLAAigUZLl79y6GTUzKysqMyAJgPshJIHznzp3/MDZAgAEAkoIW/jHg7H4AAAAASUVORK5CYII=",
			"iVBORw0KGgoAAAANSUhEUgAAAAgAAAACCAIAAADq9gq6AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAABVJREFUeNpiVFZWZsAGmBhwAIAAAwAURgBt4C03ZwAAAABJRU5ErkJggg==",
			"iVBORw0KGgoAAAANSUhEUgAAAAgAAAACCAIAAADq9gq6AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAABVJREFUeNpivHPnDgM2wMSAAwAEGAB8VgKYlvqkBwAAAABJRU5ErkJggg==",
			"iVBORw0KGgoAAAANSUhEUgAAAAcAAAAFCAYAAACJmvbYAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAEFJREFUeNpiKCoq+v/p06f/ly9fhmMQHyTOxIAH4JVkARHv379nkJeXhwuC+CDA+P//f4bi4uL/6Lp6e3sZAQIMACmoI7rWhl0KAAAAAElFTkSuQmCC",
			"iVBORw0KGgoAAAANSUhEUgAAAAcAAAAFCAYAAACJmvbYAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAEFJREFUeNpiFBER+f/jxw8GNjY2Bhj49esXAwcHBwMTAx6AV5IFRPz58wdFEMZn/P//P4OoqOh/dF2vX79mBAgwADpeFCsbeaC+AAAAAElFTkSuQmCC",
			"iVBORw0KGgoAAAANSUhEUgAAAAcAAAAHCAYAAADEUlfTAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAFBJREFUeNpi/P//P0NxcfF/BjTQ29vLyFBUVPT/4cOH/z99+gTHID5InAWkSlBQkAEoANclLy8PppkY8AC8kmBj379/DzcKxgcBRnyuBQgwACVNLqBePwzmAAAAAElFTkSuQmCC",
			"iVBORw0KGgoAAAANSUhEUgAAAAcAAAAHCAYAAADEUlfTAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAElJREFUeNp8jjEKADEIBNdDrCz1/w+0tRQMOchxpHC6dVhW6m64e+MiIojMrDMTzPyJqoKq4r1sISJ3GQ8GRsln48/JNH27BBgAUhQbSyMxqzEAAAAASUVORK5CYII=",
			"iVBORw0KGgoAAAANSUhEUgAAAAUAAAAECAYAAABGM/VAAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAEFJREFUeNpi/P//P0NxcfF/BgRgZP78+fN/VVVVhpCQEAZjY2OGs2fPNrCApBwdHRkePHgAVwoWnDVrFgMyAAgwAAt4E1dCq1obAAAAAElFTkSuQmCC",
			"iVBORw0KGgoAAAANSUhEUgAAAAUAAAAECAYAAABGM/VAAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAADtJREFUeNpi/P//P0NxcfF/Bijo7e1lZCgqKvr/6dOn/5cvXwbTID4TSPb9+/cM8vLyYBoEGLFpBwgwAHGiI8KoD3BZAAAAAElFTkSuQmCC",
			"iVBORw0KGgoAAAANSUhEUgAAAAUAAAAECAIAAADJUWIXAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAACJJREFUeNpiDA0NZUACLEDc2dkJ4ZSXlzMxoAJGNPUAAQYAwbcFBwYygqkAAAAASUVORK5CYII=",
			"iVBORw0KGgoAAAANSUhEUgAAAAkAAAAFCAYAAACXU8ZrAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAACdJREFUeNpi/PTp038GAoClvr6ekBoGxv//CRrEwPL582fqWAcQYAAnaA2zsd+RkQAAAABJRU5ErkJggg==",
		};
		/// <summary>
		/// Resource assets for dark skin.
		/// </summary>
		/// <remarks>
		/// <para>Resource assets are PNG images which have been encoded using a base-64
		/// string so that actual asset files are not necessary.</para>
		/// </remarks>
		private static string[] s_DarkSkin = {
			"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",
		};

		/// <summary>
		/// Gets light or dark version of the specified texture.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static Texture2D GetTexture(ReorderableListTexture name) {
			return s_Cached[(int)name];
		}

		#endregion

		#region Generated Resources

		public static Texture2D texHighlightColor { get; private set; }

		/// <summary>
		/// Generate special textures.
		/// </summary>
		private static void GenerateSpecialTextures() {
			texHighlightColor = CreatePixelTexture("(Generated) Highlight Color", ReorderableListStyles.SelectionBackgroundColor);
		}

		/// <summary>
		/// Create 1x1 pixel texture of specified color.
		/// </summary>
		/// <param name="name">Name for texture object.</param>
		/// <param name="color">Pixel color.</param>
		/// <returns>
		/// The new <c>Texture2D</c> instance.
		/// </returns>
		public static Texture2D CreatePixelTexture(string name, Color color) {
			var tex = new Texture2D(1, 1, TextureFormat.ARGB32, false, true);
			tex.name = name;
			tex.hideFlags = HideFlags.HideAndDontSave;
			tex.filterMode = FilterMode.Point;
			tex.SetPixel(0, 0, color);
			tex.Apply();
			return tex;
		}

		#endregion

		#region Load PNG from Base-64 Encoded String

		private static Texture2D[] s_Cached;

		/// <summary>
		/// Read textures from base-64 encoded strings. Automatically selects assets based
		/// upon whether the light or dark (pro) skin is active.
		/// </summary>
		private static void LoadResourceAssets() {
			var skin = EditorGUIUtility.isProSkin ? s_DarkSkin : s_LightSkin;
			s_Cached = new Texture2D[skin.Length];

			for (int i = 0; i < s_Cached.Length; ++i) {
				// Get image data (PNG) from base64 encoded strings.
				byte[] imageData = Convert.FromBase64String(skin[i]);

				// Gather image size from image data.
				int texWidth, texHeight;
				GetImageSize(imageData, out texWidth, out texHeight);

				// Generate texture asset.
				var tex = new Texture2D(texWidth, texHeight, TextureFormat.ARGB32, false, true);
				tex.hideFlags = HideFlags.HideAndDontSave;
				tex.name = "(Generated) ReorderableList:" + i;
				tex.filterMode = FilterMode.Point;
#if UNITY_2017_1_OR_NEWER
                ImageConversion.LoadImage(tex, imageData, markNonReadable: true);
#else
                tex.LoadImage(imageData);
#endif

				s_Cached[i] = tex;
			}

			s_LightSkin = null;
			s_DarkSkin = null;
		}

		/// <summary>
		/// Read width and height if PNG file in pixels.
		/// </summary>
		/// <param name="imageData">PNG image data.</param>
		/// <param name="width">Width of image in pixels.</param>
		/// <param name="height">Height of image in pixels.</param>
		private static void GetImageSize(byte[] imageData, out int width, out int height) {
			width = ReadInt(imageData, 3 + 15);
			height = ReadInt(imageData, 3 + 15 + 2 + 2);
		}

		private static int ReadInt(byte[] imageData, int offset) {
			return (imageData[offset] << 8) | imageData[offset + 1];
		}

		#endregion

	}

}

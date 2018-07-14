// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using Rotorz.ReorderableList.Internal;
using UnityEditor;
using UnityEngine;

namespace Rotorz.ReorderableList {

	/// <summary>
	/// Styles for the <see cref="ReorderableListControl"/>.
	/// </summary>
	public static class ReorderableListStyles {

		static ReorderableListStyles() {
			Title = new GUIStyle();
			Title.border = new RectOffset(2, 2, 2, 1);
			Title.margin = new RectOffset(5, 5, 5, 0);
			Title.padding = new RectOffset(5, 5, 3, 3);
			Title.alignment = TextAnchor.MiddleLeft;
			Title.normal.background = ReorderableListResources.GetTexture(ReorderableListTexture.TitleBackground);
			Title.normal.textColor = EditorGUIUtility.isProSkin
				? new Color(0.8f, 0.8f, 0.8f)
				: new Color(0.2f, 0.2f, 0.2f);

			Container = new GUIStyle();
			Container.border = new RectOffset(2, 2, 2, 2);
			Container.margin = new RectOffset(5, 5, 0, 0);
			Container.padding = new RectOffset(2, 2, 2, 2);
			Container.normal.background = ReorderableListResources.GetTexture(ReorderableListTexture.ContainerBackground);

			Container2 = new GUIStyle(Container);
			Container2.normal.background = ReorderableListResources.GetTexture(ReorderableListTexture.Container2Background);

			FooterButton = new GUIStyle();
			FooterButton.fixedHeight = 16;
			FooterButton.alignment = TextAnchor.MiddleCenter;
			FooterButton.normal.background = ReorderableListResources.GetTexture(ReorderableListTexture.Button_Normal);
			FooterButton.active.background = ReorderableListResources.GetTexture(ReorderableListTexture.Button_Active);
			FooterButton.border = new RectOffset(3, 3, 1, 3);
			FooterButton.padding = new RectOffset(2, 2, 0, 2);
			FooterButton.clipping = TextClipping.Overflow;

			FooterButton2 = new GUIStyle();
			FooterButton2.fixedHeight = 18;
			FooterButton2.alignment = TextAnchor.MiddleCenter;
			FooterButton2.normal.background = ReorderableListResources.GetTexture(ReorderableListTexture.Button2_Normal);
			FooterButton2.active.background = ReorderableListResources.GetTexture(ReorderableListTexture.Button2_Active);
			FooterButton2.border = new RectOffset(3, 3, 3, 3);
			FooterButton2.padding = new RectOffset(2, 2, 2, 2);
			FooterButton2.clipping = TextClipping.Overflow;

			ItemButton = new GUIStyle();
			ItemButton.active.background = ReorderableListResources.CreatePixelTexture("Dark Pixel (List GUI)", new Color32(18, 18, 18, 255));
			ItemButton.imagePosition = ImagePosition.ImageOnly;
			ItemButton.alignment = TextAnchor.MiddleCenter;
			ItemButton.overflow = new RectOffset(0, 0, -1, 0);
			ItemButton.padding = new RectOffset(0, 0, 1, 0);
			ItemButton.contentOffset = new Vector2(0, -1f);

			SelectedItem = new GUIStyle();
			SelectedItem.normal.background = ReorderableListResources.texHighlightColor;
			SelectedItem.normal.textColor = Color.white;
			SelectedItem.fontSize = 12;
		}

		/// <summary>
		/// Gets style for title header.
		/// </summary>
		public static GUIStyle Title { get; private set; }

		/// <summary>
		/// Gets style for the background of list control.
		/// </summary>
		public static GUIStyle Container { get; private set; }
		/// <summary>
		/// Gets an alternative style for the background of list control.
		/// </summary>
		public static GUIStyle Container2 { get; private set; }
		/// <summary>
		/// Gets style for footer button.
		/// </summary>
		public static GUIStyle FooterButton { get; private set; }
		/// <summary>
		/// Gets an alternative style for footer button.
		/// </summary>
		public static GUIStyle FooterButton2 { get; private set; }
		/// <summary>
		/// Gets style for remove item button.
		/// </summary>
		public static GUIStyle ItemButton { get; private set; }

		/// <summary>
		/// Gets style for the background of a selected item.
		/// </summary>
		public static GUIStyle SelectedItem { get; private set; }

		/// <summary>
		/// Gets color for the horizontal lines that appear between list items.
		/// </summary>
		public static Color HorizontalLineColor {
			get { return EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.14f) : new Color(0.59f, 0.59f, 0.59f, 0.55f); }
		}

		/// <summary>
		/// Gets color of background for a selected list item.
		/// </summary>
		public static Color SelectionBackgroundColor {
			get { return EditorGUIUtility.isProSkin ? new Color32(62, 95, 150, 255) : new Color32(62, 125, 231, 255); }
		}

	}

}

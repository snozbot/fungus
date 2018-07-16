// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using Rotorz.ReorderableList;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ReorderableListDemo : EditorWindow {

	[MenuItem("Window/List Demo (C#)")]
	static void ShowWindow() {
		GetWindow<ReorderableListDemo>("List Demo");
	}

	public List<string> shoppingList;
	public List<string> purchaseList;
	
	private void OnEnable() {
		shoppingList = new List<string>();
		shoppingList.Add("Bread");
		shoppingList.Add("Carrots");
		shoppingList.Add("Beans");
		shoppingList.Add("Steak");
		shoppingList.Add("Coffee");
		shoppingList.Add("Fries");

		purchaseList = new List<string>();
		purchaseList.Add("Cheese");
		purchaseList.Add("Crackers");
	}

	private Vector2 _scrollPosition;

	private void OnGUI() {
		_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
		
		ReorderableListGUI.Title("Shopping List");
		ReorderableListGUI.ListField(shoppingList, PendingItemDrawer, DrawEmpty);

		ReorderableListGUI.Title("Purchased Items");
		ReorderableListGUI.ListField(purchaseList, PurchasedItemDrawer, DrawEmpty, ReorderableListFlags.HideAddButton | ReorderableListFlags.DisableReordering);
	
		GUILayout.EndScrollView();
	}
	
	private string PendingItemDrawer(Rect position, string itemValue) {
		// Text fields do not like null values!
		if (itemValue == null)
			itemValue = "";
		
		position.width -= 50;
		itemValue = EditorGUI.TextField(position, itemValue);
		
		position.x = position.xMax + 5;
		position.width = 45;
		if (GUI.Button(position, "Info")) {
		}
		
		return itemValue;
	}

	private string PurchasedItemDrawer(Rect position, string itemValue) {
		position.width -= 50;
		GUI.Label(position, itemValue);

		position.x = position.xMax + 5;
		position.width = 45;
		if (GUI.Button(position, "Info")) {
		}

		return itemValue;
	}

	private void DrawEmpty() {
		GUILayout.Label("No items in list.", EditorStyles.miniLabel);
	}
	
}
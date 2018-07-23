// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using Rotorz.ReorderableList;
using UnityEditor;

[CustomEditor(typeof(DemoBehaviour))]
public class DemoBehaviourEditor : Editor {

	private SerializedProperty _wishlistProperty;
	private SerializedProperty _pointsProperty;

	private void OnEnable() {
		_wishlistProperty = serializedObject.FindProperty("wishlist");
		_pointsProperty = serializedObject.FindProperty("points");
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();

		ReorderableListGUI.Title("Wishlist");
		ReorderableListGUI.ListField(_wishlistProperty);

		ReorderableListGUI.Title("Points");
		ReorderableListGUI.ListField(_pointsProperty);

		serializedObject.ApplyModifiedProperties();
	}

}
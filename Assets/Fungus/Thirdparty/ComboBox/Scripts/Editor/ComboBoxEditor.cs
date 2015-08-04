using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ComboBox))]
public class ComboBoxEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		var comboBoxGO = target as ComboBox;

		var allowUpdate = comboBoxGO.transform.Find("Button") != null;

		if (allowUpdate)
			comboBoxGO.UpdateGraphics();
		
		EditorGUI.BeginChangeCheck();
		DrawDefaultInspector();
		if (EditorGUI.EndChangeCheck())
		{
			if (Application.isPlaying)
			{
				comboBoxGO.HideFirstItem = comboBoxGO.HideFirstItem;
				comboBoxGO.Interactable = comboBoxGO.Interactable;
			}
			else
				if (allowUpdate)
					comboBoxGO.RefreshSelected();
		}
	}
}

public class ComboBoxMenuItem
{
	[MenuItem("GameObject/UI/ComboBox")]
	public static void CreateComboBox()
	{
		var canvas = Object.FindObjectOfType<Canvas>();
		var canvasGO = canvas == null ? null : canvas.gameObject;
		if (canvasGO == null)
		{
			canvasGO = new GameObject("Canvas");
			canvas = canvasGO.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvasGO.AddComponent<CanvasScaler>();
			canvasGO.AddComponent<GraphicRaycaster>();
		}
		var eventSystem = Object.FindObjectOfType<EventSystem>();
		var eventSystemGO = eventSystem == null ? null : eventSystem.gameObject;
		if (eventSystemGO == null)
		{
			eventSystemGO = new GameObject("EventSystem");
			eventSystem = eventSystemGO.AddComponent<EventSystem>();
			eventSystemGO.AddComponent<StandaloneInputModule>();
			eventSystemGO.AddComponent<TouchInputModule>();
		}
		var comboBoxGO = new GameObject("ComboBox");
		comboBoxGO.transform.SetParent(canvasGO.transform, false);
		var rTransform = comboBoxGO.AddComponent<RectTransform>();
		rTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160);
		rTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 30);
		for (var i = 0; i < Selection.objects.Length; i++)
		{
			var selected = Selection.objects[i] as GameObject;
			var hierarchyItem = selected.transform;
			canvas = null;
			while (hierarchyItem != null && (canvas = hierarchyItem.GetComponent<Canvas>()) == null)
				hierarchyItem = hierarchyItem.parent;
			if (canvas != null)
			{
				comboBoxGO.transform.SetParent(selected.transform, false);
				break;
			}
		}
		rTransform.anchoredPosition = Vector2.zero;
		var comboBox = comboBoxGO.AddComponent<ComboBox>();
		LoadAssets();
		comboBox.Sprite_UISprite = Sprite_UISprite;
		comboBox.Sprite_Background = Sprite_Background;
		comboBox.CreateControl();
		Selection.activeGameObject = comboBoxGO;
	}

	private static Sprite Sprite_UISprite;
	private static Sprite Sprite_Background;
	public static void LoadAssets()
	{
		while (Sprite_UISprite == null || Sprite_Background == null)
		{
			var sprites = Resources.FindObjectsOfTypeAll<Sprite>();
			foreach (var sprite in sprites)
				switch (sprite.name)
				{
					case "UISprite":
						Sprite_UISprite = sprite;
						break;
					case "Background":
						Sprite_Background = sprite;
						break;
				}
			if (Sprite_UISprite == null || Sprite_Background == null)
				AssetDatabase.LoadAllAssetsAtPath("Resources/unity_builtin_extra");
		}
	}
}
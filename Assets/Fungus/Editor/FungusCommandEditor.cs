using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[CustomEditor (typeof(FungusCommand), true)]
	public class FungusCommandEditor : Editor 
	{

		public static FungusCommand selectedCommand;

		public override void OnInspectorGUI() 
		{
			Rect rect = EditorGUILayout.BeginVertical();

			DrawCommandInspectorGUI();

			FungusCommand t = target as FungusCommand;
			if (t != null)
			{
				if (t.errorMessage.Length > 0)
				{
					GUIStyle style = new GUIStyle(GUI.skin.label);
					style.normal.textColor = new Color(1,0,0);
					EditorGUILayout.LabelField(new GUIContent("Error: " + t.errorMessage), style);
				}

				if (t.IsExecuting())
				{
					EditorGUI.DrawRect(rect, new Color(0f, 1f, 0f, 0.25f));
				}
				else if (t == selectedCommand)
				{
					EditorGUI.DrawRect(rect, new Color(1f, 1f, 0f, 0.25f));
				}
			}

			EditorGUILayout.EndVertical();
		}

		public virtual void DrawCommandInspectorGUI()
		{
			DrawDefaultInspector();
		}
	}

}

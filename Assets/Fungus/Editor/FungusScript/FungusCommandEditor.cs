using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus.Script
{

	[CustomEditor (typeof(FungusCommand), true)]
	public class FungusCommandEditor : Editor 
	{
		public static FungusCommand selectedCommand;

		void OnEnable()
		{
			FungusCommand t = target as FungusCommand;
			if (t != null)
			{
				t.hideFlags = HideFlags.HideInInspector;
			}
		}

		public override void OnInspectorGUI() 
		{
			FungusCommand t = target as FungusCommand;

			Rect rect = EditorGUILayout.BeginVertical();

			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Up"))
			{
				UnityEditorInternal.ComponentUtility.MoveComponentUp(t);
			}
			if (GUILayout.Button("Down"))
			{
				UnityEditorInternal.ComponentUtility.MoveComponentDown(t);
			}

			GUILayout.FlexibleSpace();

			FungusScript fungusScript = t.GetFungusScript();

			if (fungusScript != null)
			{
				if (GUILayout.Button("Copy"))
				{
					fungusScript.copyCommand = t;
				}

				if (fungusScript.copyCommand != null)
				{
					if (GUILayout.Button("Paste"))
					{
						CopyComponent<FungusCommand>(fungusScript.copyCommand, t.gameObject);
					}
				}
			}

			if (GUILayout.Button("Delete"))
			{
				Undo.DestroyObjectImmediate(t);
				return;
			}

			GUILayout.EndHorizontal();

			EditorGUILayout.Separator();

			DrawCommandInspectorGUI();

			EditorGUILayout.Separator();

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

		T CopyComponent<T>(T original, GameObject destination) where T : Component
		{
			System.Type type = original.GetType();
			Component copy = Undo.AddComponent(destination, type);
			System.Reflection.FieldInfo[] fields = type.GetFields();
			foreach (System.Reflection.FieldInfo field in fields)
			{
				field.SetValue(copy, field.GetValue(original));
			}
			return copy as T;
		}
	}

}

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

			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();

			bool expanded = EditorGUILayout.Foldout(t.expanded, t.GetType().Name);

			if (expanded != t.expanded)
			{
				Undo.RecordObject(t, "Set Expanded");
				t.expanded = expanded;
			}

			GUIStyle labelStyle = EditorStyles.miniLabel;
			GUILayout.Label(t.GetSummary().Replace("\n", "").Replace("\r", ""), labelStyle);

			GUILayout.FlexibleSpace();

			GUILayout.EndHorizontal();

			if (!t.expanded)
			{
				GUILayout.EndVertical();
				if (Event.current.type == EventType.Repaint &&
				    t.IsExecuting())
				{
					Rect rect = GUILayoutUtility.GetLastRect();
					rect.x -= 10;
					rect.width += 10;
					GLDraw.DrawBox(rect, Color.green, 1.5f);
				}
				return;
			}

			GUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();

			if (GUILayout.Button("Up", EditorStyles.miniButtonLeft))
			{
				UnityEditorInternal.ComponentUtility.MoveComponentUp(t);
			}
			if (GUILayout.Button("Down", EditorStyles.miniButtonMid))
			{
				UnityEditorInternal.ComponentUtility.MoveComponentDown(t);
			}
			
			FungusScript fungusScript = t.GetFungusScript();
			
			if (fungusScript != null)
			{
				if (GUILayout.Button("Copy", EditorStyles.miniButtonMid))
				{
					fungusScript.copyCommand = t;
				}
				
				if (fungusScript.copyCommand != null)
				{
					if (GUILayout.Button("Paste", EditorStyles.miniButtonMid))
					{
						CopyComponent<FungusCommand>(fungusScript.copyCommand, t.gameObject);
					}
				}
			}
			
			if (GUILayout.Button("Delete", EditorStyles.miniButtonRight))
			{
				Undo.DestroyObjectImmediate(t);
				return;
			}

			GUILayout.FlexibleSpace();

			GUILayout.EndHorizontal();

			EditorGUILayout.Separator();

			DrawCommandGUI();

			EditorGUILayout.Separator();

			if (t != null)
			{
				if (t.errorMessage.Length > 0)
				{
					GUIStyle style = new GUIStyle(GUI.skin.label);
					style.normal.textColor = new Color(1,0,0);
					EditorGUILayout.LabelField(new GUIContent("Error: " + t.errorMessage), style);
				}
			}

			GUILayout.EndVertical();

			if (Event.current.type == EventType.Repaint &&
			    t.IsExecuting())
			{
				Rect rect = GUILayoutUtility.GetLastRect();
				rect.x -= 10;
				rect.width += 10;
				GLDraw.DrawBox(rect, Color.green, 1.5f);
			}
		}

		public virtual void DrawCommandGUI()
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

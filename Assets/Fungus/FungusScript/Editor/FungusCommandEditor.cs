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

		public virtual void DrawInspectorGUI(int commandIndex) 
		{
			FungusCommand t = target as FungusCommand;

			if (t == null)
			{
				return;
			}

			GUILayout.BeginVertical();

			GUILayout.BeginHorizontal();

			bool error = false;
			string summary = t.GetSummary().Replace("\n", "").Replace("\r", "");
			if (summary.Length > 80)
			{
				summary = summary.Substring(0, 80) + "...";
			}
			if (summary.StartsWith("Error:"))
			{
				error = true;
			}

			if (!t.enabled)
			{
				GUI.backgroundColor = Color.grey;
			}
			else if (error)
			{
				GUI.backgroundColor = Color.red;
			}
			else if (t.expanded)
			{
				GUI.backgroundColor = Color.yellow;
			}

			string commandName = FungusScriptEditor.GetCommandName(t.GetType());
			if (GUILayout.Button(commandName, EditorStyles.miniButton, GUILayout.MinWidth(80)))
			{
				Undo.RecordObject(t, "Toggle Expanded");
				t.expanded = !t.expanded;
			}
			GUI.backgroundColor = Color.white;

			GUIStyle labelStyle = new GUIStyle(EditorStyles.whiteMiniLabel);
			labelStyle.wordWrap = true;
			if (!t.enabled)
			{
				labelStyle.normal.textColor = Color.grey;
			}
			else if (error)
			{
				labelStyle.normal.textColor = Color.red;
			}

			GUILayout.Label(summary, labelStyle);
			GUILayout.FlexibleSpace();

			GUILayout.EndHorizontal();

			if (t.expanded)
			{
				GUILayout.BeginHorizontal();

				GUILayout.FlexibleSpace();

				bool enabled = GUILayout.Toggle(t.enabled, "");
				if (t.enabled != enabled)
				{
					Undo.RecordObject(t, "Set Enabled");
					t.enabled = enabled;
				}

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

				GUILayout.EndHorizontal();

				DrawCommandGUI();

				EditorGUILayout.Separator();

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

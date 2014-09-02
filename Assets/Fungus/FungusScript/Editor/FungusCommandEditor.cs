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

		public virtual void DrawCommandRowGUI() 
		{
			FungusCommand t = target as FungusCommand;
			if (t == null)
			{
				return;
			}

			FungusScript fungusScript = t.GetFungusScript();

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
			else if (fungusScript.selectedCommand == t)
			{
				GUI.backgroundColor = Color.yellow;
			}

			string commandName = FungusScriptEditor.GetCommandName(t.GetType());
			if (GUILayout.Button(commandName, EditorStyles.miniButton, GUILayout.MinWidth(80)))
			{
				fungusScript.selectedCommand = t;
				GUIUtility.keyboardControl = 0; // Fix for textarea not refeshing (change focus)
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

			if (Event.current.type == EventType.Repaint &&
			    t.IsExecuting())
			{
				Rect rect = GUILayoutUtility.GetLastRect();
				GLDraw.DrawBox(rect, Color.green, 1.5f);
			}
		}

		public virtual void DrawCommandInspectorGUI()
		{
			FungusCommand t = target as FungusCommand;
			if (t == null)
			{
				return;
			}

			FungusScript fungusScript = t.GetFungusScript();

			GUILayout.BeginVertical(GUI.skin.box);

			GUILayout.BeginHorizontal();

			string commandName = FungusScriptEditor.GetCommandName(t.GetType());
			GUILayout.Label(commandName, EditorStyles.largeLabel);

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

			EditorGUILayout.Separator();
			
			DrawCommandGUI();

			EditorGUILayout.Separator();

			if (t.errorMessage.Length > 0)
			{
				GUIStyle style = new GUIStyle(GUI.skin.label);
				style.normal.textColor = new Color(1,0,0);
				EditorGUILayout.LabelField(new GUIContent("Error: " + t.errorMessage), style);
			}

			GUILayout.EndVertical();
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

		static public T ObjectField<T>(GUIContent label, GUIContent nullLabel, T selectedObject, List<T> objectList) where T : MonoBehaviour
		{
			List<GUIContent> objectNames = new List<GUIContent>();
			
			int selectedIndex = 0;
			objectNames.Add(nullLabel);
			for (int i = 0; i < objectList.Count; ++i)
			{
				objectNames.Add(new GUIContent(objectList[i].name));
				
				if (selectedObject == objectList[i])
				{
					selectedIndex = i + 1;
				}
			}

			T result;

			selectedIndex = EditorGUILayout.Popup(label, selectedIndex, objectNames.ToArray());
			if (selectedIndex == 0)
			{
				result = null; // Null option
			}
			else
			{
				result = objectList[selectedIndex - 1];
			}
			
			return result;
		}
	}

}

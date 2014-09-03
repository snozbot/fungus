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

		public static CommandInfoAttribute GetCommandInfo(System.Type commandType)
		{
			object[] attributes = commandType.GetCustomAttributes(typeof(CommandInfoAttribute), false);
			foreach (object obj in attributes)
			{
				CommandInfoAttribute commandInfoAttr = obj as CommandInfoAttribute;
				if (commandInfoAttr != null)
				{
					return commandInfoAttr;
				}
			}
			
			return null;
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

			bool enabled = GUILayout.Toggle(t.enabled, new GUIContent());
			if (t.enabled != enabled)
			{
				Undo.RecordObject(t, "Set Enabled");
				t.enabled = enabled;
			}

			CommandInfoAttribute commandInfoAttr = FungusCommandEditor.GetCommandInfo(t.GetType());
			if (commandInfoAttr == null)
			{
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
				return;
			}

			string commandName = commandInfoAttr.CommandName;
			GUILayout.Label(commandName + " Command", EditorStyles.boldLabel);

			GUILayout.FlexibleSpace();

			if (fungusScript != null)
			{
				if (GUILayout.Button("Copy", EditorStyles.miniButtonMid))
				{
					fungusScript.copyCommand = t;
				}
				
				if (fungusScript.copyCommand != null)
				{
					if (GUILayout.Button("Paste", EditorStyles.miniButton))
					{
						Sequence parentSequence = t.GetComponent<Sequence>();
						if (parentSequence != null)
						{
							PasteCommand(fungusScript.copyCommand, parentSequence);
						}
					}
				}
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

		static public FungusCommand PasteCommand(FungusCommand copyCommand, Sequence sequence)
		{
			System.Type type = copyCommand.GetType();
			Component copy = Undo.AddComponent(sequence.gameObject, type);
			System.Reflection.FieldInfo[] fields = type.GetFields();
			foreach (System.Reflection.FieldInfo field in fields)
			{
				field.SetValue(copy, field.GetValue(copyCommand));
			}

			FungusScript fungusScript = sequence.GetFungusScript();

			Undo.RecordObject(fungusScript, "Paste Command");

			FungusCommand newCommand = copy as FungusCommand;
			sequence.commandList.Add(newCommand);

			return newCommand;
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

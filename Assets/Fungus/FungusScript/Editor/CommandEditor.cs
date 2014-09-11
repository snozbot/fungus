using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[CustomEditor (typeof(Command), true)]
	public class CommandEditor : Editor 
	{
		public static Command selectedCommand;

		void OnEnable()
		{
			Command t = target as Command;
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
			Command t = target as Command;
			if (t == null)
			{
				return;
			}

			FungusScript fungusScript = t.GetFungusScript();
			if (fungusScript == null)
			{
				return;
			}

			CommandInfoAttribute commandInfoAttr = CommandEditor.GetCommandInfo(t.GetType());
			if (commandInfoAttr == null)
			{
				return;
			}

			GUILayout.BeginVertical(GUI.skin.box);

			GUI.backgroundColor = Color.green;
			GUILayout.BeginHorizontal(GUI.skin.button);

			string commandName = commandInfoAttr.CommandName;
			GUIStyle commandStyle = new GUIStyle(EditorStyles.miniButton);
			if (t.enabled)
			{
				if (fungusScript.colorCommands)
				{
					GUI.backgroundColor = t.GetButtonColor();
				}
				else
				{
					GUI.backgroundColor = Color.white;
				}
			}
			else
			{
				GUI.backgroundColor = Color.grey;
			}

			bool enabled = t.enabled;

			if (GUILayout.Button(commandName, commandStyle, GUILayout.MinWidth(80), GUILayout.ExpandWidth(true)))
			{
				enabled = !enabled;
			}

			GUI.backgroundColor = Color.white;
			enabled = GUILayout.Toggle(enabled, new GUIContent());

			if (t.enabled != enabled)
			{
				Undo.RecordObject(t, "Set Enabled");
				t.enabled = enabled;
			}

			if (fungusScript != null)
			{
				if (GUILayout.Button("Copy", EditorStyles.miniButton))
				{
					fungusScript.copyCommand = t;
				}				
			}

			GUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;

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

		static public Command PasteCommand(Command copyCommand, Sequence sequence)
		{
			if (sequence == null)
			{
				return null;
			}

			System.Type type = copyCommand.GetType();
			Component copy = Undo.AddComponent(sequence.gameObject, type);
			System.Reflection.FieldInfo[] fields = type.GetFields();
			foreach (System.Reflection.FieldInfo field in fields)
			{
				field.SetValue(copy, field.GetValue(copyCommand));
			}

			Undo.RecordObject(sequence, "Paste Command");

			Command newCommand = copy as Command;
			sequence.commandList.Add(newCommand);

			return newCommand;
		}

		static public void ObjectField<T>(SerializedProperty property, GUIContent label, GUIContent nullLabel, List<T> objectList) where T : MonoBehaviour
		{
			if (property == null)
			{
				return;
			}

			List<GUIContent> objectNames = new List<GUIContent>();

			T selectedObject = property.objectReferenceValue as T;

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

			property.objectReferenceValue = result;
		}
	}

}

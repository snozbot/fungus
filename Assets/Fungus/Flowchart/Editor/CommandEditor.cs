using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;

namespace Fungus
{

	[CustomEditor (typeof(Command), true)]
	public class CommandEditor : Editor 
	{
		public static Command selectedCommand;

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

			Flowchart flowchart = t.GetFlowchart();
			if (flowchart == null)
			{
				return;
			}

			CommandInfoAttribute commandInfoAttr = CommandEditor.GetCommandInfo(t.GetType());
			if (commandInfoAttr == null)
			{
				return;
			}

			GUILayout.BeginVertical(GUI.skin.box);

			if (t.enabled)
			{
				if (flowchart.colorCommands)
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
			GUILayout.BeginHorizontal(GUI.skin.button);

			string commandName = commandInfoAttr.CommandName;
			GUILayout.Label(commandName, GUILayout.MinWidth(80), GUILayout.ExpandWidth(true));

			GUILayout.FlexibleSpace();

			GUILayout.Label(new GUIContent("(" + t.itemId + ")"));

			GUILayout.Space(10);

			GUI.backgroundColor = Color.white;
			bool enabled = t.enabled;
			enabled = GUILayout.Toggle(enabled, new GUIContent());

			if (t.enabled != enabled)
			{
				Undo.RecordObject(t, "Set Enabled");
				t.enabled = enabled;
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

			// Display help text
			CommandInfoAttribute infoAttr = CommandEditor.GetCommandInfo(t.GetType());
			if (infoAttr != null)
			{
				EditorGUILayout.HelpBox(infoAttr.HelpText, MessageType.Info, true);
			}
		}

		public virtual void DrawCommandGUI()
		{
			Command t = target as Command;
			
			// Code below was copied from here
			// http://answers.unity3d.com/questions/550829/how-to-add-a-script-field-in-custom-inspector.html

			// Users should not be able to change the MonoScript for the command using the usual Script field.
			// Doing so could cause block.commandList to contain null entries.
			// To avoid this we manually display all properties, except for m_Script.
			serializedObject.Update();
			SerializedProperty iterator = serializedObject.GetIterator();
			bool enterChildren = true;
			while (iterator.NextVisible(enterChildren))
			{
				enterChildren = false;

				if (iterator.name == "m_Script")
				{
					continue;
				}

				if (!t.IsPropertyVisible(iterator.name))
				{
					continue;
				}

				if (iterator.isArray &&
					t.IsReorderableArray(iterator.name))
				{
					ReorderableListGUI.Title(new GUIContent(iterator.displayName, iterator.tooltip));
					ReorderableListGUI.ListField(iterator);
				}
				else
				{
					EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
				}
			}

			serializedObject.ApplyModifiedProperties();
		}

		static public void ObjectField<T>(SerializedProperty property, GUIContent label, GUIContent nullLabel, List<T> objectList) where T : Object 
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
				if (objectList[i] == null) continue;
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

		/**
		 * When modifying custom editor code you can occasionally end up with orphaned editor instances.
		 * When this happens, you'll get a null exception error every time the scene serializes / deserialized.
		 * Once this situation occurs, the only way to fix it is to restart the Unity editor.
		 * 
		 * As a workaround, this function detects if this command editor is an orphan and deletes it. 
		 * To use it, just call this function at the top of the OnEnable() method in your custom editor.
		 */
		protected virtual bool NullTargetCheck()
		{
			try
			{
				// The serializedObject accessor create a new SerializedObject if needed.
				// However, this will fail with a null exception if the target object no longer exists.
				#pragma warning disable 0219
				SerializedObject so = serializedObject;
			}
			catch (System.NullReferenceException)
			{
				DestroyImmediate(this);
				return true;
			}

			return false;
		}
	}
}

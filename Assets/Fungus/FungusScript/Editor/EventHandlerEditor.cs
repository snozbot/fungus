using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	[CustomEditor (typeof(EventHandler), true)]
	public class EventHandlerEditor : Editor 
	{
		public virtual void DrawInspectorGUI()
		{
			// Users should not be able to change the MonoScript for the command using the usual Script field.
			// Doing so could cause sequence.commandList to contain null entries.
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
				
				EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
			}

			EventHandler t = target as EventHandler;
			EventHandlerInfoAttribute info = EventHandler.GetEventHandlerInfo(t.GetType());
			if (info != null &&
			    info.HelpText.Length > 0)
			{
				EditorGUILayout.HelpBox(info.HelpText, MessageType.Info);
			}

			serializedObject.ApplyModifiedProperties();
		}
	}

}

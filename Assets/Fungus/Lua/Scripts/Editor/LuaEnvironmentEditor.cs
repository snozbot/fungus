using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

namespace Fungus
{
    
    [CustomEditor (typeof(LuaEnvironment))]
	public class LuaEnvironmentEditor : Editor 
    {
		protected SerializedProperty registerTypesProp;
		protected ReorderableList registerTypeList;

        protected virtual void OnEnable()
        {
			registerTypesProp = serializedObject.FindProperty("registerTypes");
			registerTypeList = new ReorderableList(serializedObject, registerTypesProp, true, true, true, true);

			registerTypeList.drawHeaderCallback = (Rect rect) => {  
				EditorGUI.LabelField(rect, "Type Lists");
			};

			registerTypeList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => { 
				Rect r = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
				SerializedProperty element = registerTypesProp.GetArrayElementAtIndex(index);
				EditorGUI.PropertyField(r, element, new GUIContent(""));
			};
        }

        public override void OnInspectorGUI() 
        {
			base.OnInspectorGUI();

            serializedObject.Update();

			EditorGUILayout.PrefixLabel(new GUIContent("Register Types", "Text files which list the CLR types that should be registered with this Lua environment."));
			registerTypeList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
   }

}

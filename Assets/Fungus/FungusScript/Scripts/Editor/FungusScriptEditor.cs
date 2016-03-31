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
    
    [CustomEditor (typeof(FungusScript))]
	public class FungusScriptEditor : Editor 
    {
		protected SerializedProperty registerTypesProp;
		protected ReorderableList registerTypeList;

        protected virtual void OnEnable()
        {
			registerTypesProp = serializedObject.FindProperty("registerTypes");
			registerTypeList = new ReorderableList(serializedObject, registerTypesProp, true, true, true, true);

			registerTypeList.drawHeaderCallback = (Rect rect) => {  
				EditorGUI.LabelField(rect, "Register Types");
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

			registerTypeList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
   }

}

// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(Label))]
    public class LabelEditor : CommandEditor
    {
        protected SerializedProperty keyProp;
        
        public static void LabelField(SerializedProperty property, 
                                      GUIContent labelText, 
                                      Block block)
        {
            List<string> labelKeys = new List<string>();
            List<Label> labelObjects = new List<Label>();
            
            labelKeys.Add("<None>");
            labelObjects.Add(null);
            
            Label selectedLabel = property.objectReferenceValue as Label;

            int index = 0;
            int selectedIndex = 0;
            var commandList = block.CommandList;
            foreach (var command in commandList)
            {
                Label label = command as Label;
                if (label == null)
                {
                    continue;
                }

                labelKeys.Add(label.Key);
                labelObjects.Add(label);
                
                index++;
                
                if (label == selectedLabel)
                {
                    selectedIndex = index;
                }
            }

            selectedIndex = EditorGUILayout.Popup(labelText.text, selectedIndex, labelKeys.ToArray());

            property.objectReferenceValue = labelObjects[selectedIndex];
        }

        public override void OnEnable()
        {
            base.OnEnable();

            keyProp = serializedObject.FindProperty("key");
        }
        
        public override void DrawCommandGUI()
        {
            Label t = target as Label;

            var flowchart = (Flowchart)t.GetFlowchart();
            if (flowchart == null)
            {
                return;
            }
        
            serializedObject.Update();

            EditorGUILayout.PropertyField(keyProp);
            keyProp.stringValue = flowchart.GetUniqueLabelKey(keyProp.stringValue, t);

            serializedObject.ApplyModifiedProperties();
        }
    }    
}
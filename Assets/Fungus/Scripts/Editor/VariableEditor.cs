// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fungus.EditorUtils
{
    [CustomEditor (typeof(Variable), true)]
    public class VariableEditor : CommandEditor
    {
        protected virtual void OnEnable()
        {
            Variable t = target as Variable;
            t.hideFlags = HideFlags.HideInInspector;
        }

        public static VariableInfoAttribute GetVariableInfo(System.Type variableType)
        {
            object[] attributes = variableType.GetCustomAttributes(typeof(VariableInfoAttribute), false);
            foreach (object obj in attributes)
            {
                VariableInfoAttribute variableInfoAttr = obj as VariableInfoAttribute;
                if (variableInfoAttr != null)
                {
                    return variableInfoAttr;
                }
            }
            
            return null;
        }

        public static void VariableField(SerializedProperty property, 
                                         GUIContent label, 
                                         Flowchart flowchart,
                                         string defaultText,
                                         Func<Variable, bool> filter, 
                                         Func<string, int, string[], int> drawer = null)
        {
            List<string> variableKeys = new List<string>();
            List<Variable> variableObjects = new List<Variable>();
            
            variableKeys.Add(defaultText);
            variableObjects.Add(null);
            
            List<Variable> variables = flowchart.Variables;
            int index = 0;
            int selectedIndex = 0;

            Variable selectedVariable = property.objectReferenceValue as Variable;

            // When there are multiple Flowcharts in a scene with variables, switching
            // between the Flowcharts can cause the wrong variable property
            // to be inspected for a single frame. This has the effect of causing private
            // variable references to be set to null when inspected. When this condition 
            // occurs we just skip displaying the property for this frame.
            if (selectedVariable != null &&
                selectedVariable.gameObject != flowchart.gameObject &&
                selectedVariable.Scope == VariableScope.Private)
            {
                property.objectReferenceValue = null;
                return;
            }

            foreach (Variable v in variables)
            {
                if (filter != null)
                {
                    if (!filter(v))
                    {
                        continue;
                    }
                }
                
                variableKeys.Add(v.Key);
                variableObjects.Add(v);
                
                index++;
                
                if (v == selectedVariable)
                {
                    selectedIndex = index;
                }
            }

            List<Flowchart> fsList = Flowchart.CachedFlowcharts;
            foreach (Flowchart fs in fsList)
            {
                if (fs == flowchart)
                {
                    continue;
                }

                List<Variable> publicVars = fs.GetPublicVariables();
                foreach (Variable v in publicVars)
                {
                    if (filter != null)
                    {
                        if (!filter(v))
                        {
                            continue;
                        }
                    }

                    variableKeys.Add(fs.name + " / " + v.Key);
                    variableObjects.Add(v);

                    index++;

                    if (v == selectedVariable)
                    {
                        selectedIndex = index;
                    }
                }
            }

            if (drawer == null)
            {
                selectedIndex = EditorGUILayout.Popup(label.text, selectedIndex, variableKeys.ToArray());
            }
            else
            {
                selectedIndex = drawer(label.text, selectedIndex, variableKeys.ToArray());
            }

            property.objectReferenceValue = variableObjects[selectedIndex];
        }
    }

    [CustomPropertyDrawer(typeof(VariablePropertyAttribute))]
    public class VariableDrawer : PropertyDrawer
    {   
        
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
        {
            VariablePropertyAttribute variableProperty = attribute as VariablePropertyAttribute;
            if (variableProperty == null)
            {
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // Filter the variables by the types listed in the VariableProperty attribute
            Func<Variable, bool> compare = v => 
            {
                if (v == null)
                {
                    return false;
                }

                if (variableProperty.VariableTypes.Length == 0)
                {
                    return true;
                }

                return variableProperty.VariableTypes.Contains<System.Type>(v.GetType());
            };

            VariableEditor.VariableField(property, 
                                         label,
                                         FlowchartWindow.GetFlowchart(),
                                         variableProperty.defaultText,
                                         compare,
                                         (s,t,u) => (EditorGUI.Popup(position, s, t, u)));

            EditorGUI.EndProperty();
        }
    }
    
    public class VariableDataDrawer<T> : PropertyDrawer where T : Variable
    {   

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) 
        {
            EditorGUI.BeginProperty(position, label, property);

            // The variable reference and data properties must follow the naming convention 'typeRef', 'typeVal'

            VariableInfoAttribute typeInfo = VariableEditor.GetVariableInfo(typeof(T));
            if (typeInfo == null)
            {
                return;
            }

            string propNameBase = typeInfo.VariableType;
            propNameBase = Char.ToLowerInvariant(propNameBase[0]) + propNameBase.Substring(1);

            SerializedProperty referenceProp = property.FindPropertyRelative(propNameBase + "Ref");
            SerializedProperty valueProp = property.FindPropertyRelative(propNameBase + "Val");

            if (referenceProp == null || valueProp == null)
            {
                return;
            }

            Command command = property.serializedObject.targetObject as Command;
            if (command == null)
            {
                return;
            }

            var flowchart = command.GetFlowchart() as Flowchart;
            if (flowchart == null)
            {
                return;
            }

            if (EditorGUI.GetPropertyHeight(valueProp, label) > EditorGUIUtility.singleLineHeight)
            {
                DrawMultiLineProperty(position, label, referenceProp, valueProp, flowchart);
            }
            else
            {
                DrawSingleLineProperty(position, label, referenceProp, valueProp, flowchart);
            }

            EditorGUI.EndProperty();
        }

        protected virtual void DrawSingleLineProperty(Rect rect, GUIContent label, SerializedProperty referenceProp, SerializedProperty valueProp, Flowchart flowchart)
        {
            const int popupWidth = 17;
            
            Rect controlRect = EditorGUI.PrefixLabel(rect, label);
            Rect valueRect = controlRect;
            valueRect.width = controlRect.width - popupWidth - 5;
            Rect popupRect = controlRect;
            
            if (referenceProp.objectReferenceValue == null)
            {
                EditorGUI.PropertyField(valueRect, valueProp, new GUIContent(""));
                popupRect.x += valueRect.width + 5;
                popupRect.width = popupWidth;
            }

            EditorGUI.PropertyField(popupRect, referenceProp, new GUIContent(""));
        }

        protected virtual void DrawMultiLineProperty(Rect rect, GUIContent label, SerializedProperty referenceProp, SerializedProperty valueProp, Flowchart flowchart)
        {
            const int popupWidth = 100;
            
            Rect controlRect = rect;
            Rect valueRect = controlRect;
            valueRect.width = controlRect.width - 5;
            Rect popupRect = controlRect;
            
            if (referenceProp.objectReferenceValue == null)
            {
                EditorGUI.PropertyField(valueRect, valueProp, label);
                popupRect.x = rect.width - popupWidth + 5;
                popupRect.width = popupWidth;
            }
            else
            {
                popupRect = EditorGUI.PrefixLabel(rect, label);
            }

            EditorGUI.PropertyField(popupRect, referenceProp, new GUIContent(""));
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            VariableInfoAttribute typeInfo = VariableEditor.GetVariableInfo(typeof(T));
            if (typeInfo == null)
            {
                return EditorGUIUtility.singleLineHeight;
            }
            
            string propNameBase = typeInfo.VariableType;
            propNameBase = Char.ToLowerInvariant(propNameBase[0]) + propNameBase.Substring(1);

            SerializedProperty referenceProp = property.FindPropertyRelative(propNameBase + "Ref");

            if (referenceProp.objectReferenceValue != null)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            SerializedProperty valueProp = property.FindPropertyRelative(propNameBase + "Val");
            return EditorGUI.GetPropertyHeight(valueProp, label);
        }
    }

    [CustomPropertyDrawer (typeof(BooleanData))]
    public class BooleanDataDrawer : VariableDataDrawer<BooleanVariable>
    {}

    [CustomPropertyDrawer (typeof(IntegerData))]
    public class IntegerDataDrawer : VariableDataDrawer<IntegerVariable>
    {}

    [CustomPropertyDrawer (typeof(FloatData))]
    public class FloatDataDrawer : VariableDataDrawer<FloatVariable>
    {}

    [CustomPropertyDrawer (typeof(StringData))]
    public class StringDataDrawer : VariableDataDrawer<StringVariable>
    {}

    [CustomPropertyDrawer (typeof(StringDataMulti))]
    public class StringDataMultiDrawer : VariableDataDrawer<StringVariable>
    {}

    [CustomPropertyDrawer (typeof(ColorData))]
    public class ColorDataDrawer : VariableDataDrawer<ColorVariable>
    {}

    [CustomPropertyDrawer (typeof(Vector2Data))]
    public class Vector2DataDrawer : VariableDataDrawer<Vector2Variable>
    {}

    [CustomPropertyDrawer (typeof(Vector3Data))]
    public class Vector3DataDrawer : VariableDataDrawer<Vector3Variable>
    {}
    
    [CustomPropertyDrawer (typeof(MaterialData))]
    public class MaterialDataDrawer : VariableDataDrawer<MaterialVariable>
    {}

    [CustomPropertyDrawer (typeof(TextureData))]
    public class TextureDataDrawer : VariableDataDrawer<TextureVariable>
    {}

    [CustomPropertyDrawer (typeof(SpriteData))]
    public class SpriteDataDrawer : VariableDataDrawer<SpriteVariable>
    {}

    [CustomPropertyDrawer (typeof(GameObjectData))]
    public class GameObjectDataDrawer : VariableDataDrawer<GameObjectVariable>
    {}
    
    [CustomPropertyDrawer (typeof(ObjectData))]
    public class ObjectDataDrawer : VariableDataDrawer<ObjectVariable>
    {}

    [CustomPropertyDrawer (typeof(AnimatorData))]
    public class AnimatorDataDrawer : VariableDataDrawer<AnimatorVariable>
    {}

    [CustomPropertyDrawer (typeof(TransformData))]
    public class TransformDataDrawer : VariableDataDrawer<TransformVariable>
    {}

    [CustomPropertyDrawer (typeof(AudioSourceData))]
    public class AudioSourceDrawer : VariableDataDrawer<AudioSourceVariable>
    { }

    [CustomPropertyDrawer(typeof(Rigidbody2DData))]
    public class Rigidbody2DDataDrawer : VariableDataDrawer<Rigidbody2DVariable>
    { }
}
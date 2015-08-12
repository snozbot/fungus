using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fungus
{

	[CustomEditor(typeof(InvokeMethod))]
	public class InvokeMethodEditor : CommandEditor
	{
	    InvokeMethod targetMethod;

	    public override void DrawCommandGUI()
	    {
	        base.DrawCommandGUI();

	        targetMethod = target as InvokeMethod;

	        if (targetMethod == null || targetMethod.targetObject == null)
	            return;

	        SerializedObject objSerializedTarget = new SerializedObject(targetMethod);

	        string component = ShowComponents(objSerializedTarget, targetMethod.targetObject);

	        // show component methods if selected
	        if (!string.IsNullOrEmpty(component))
	        {
	            var method = ShowMethods(objSerializedTarget, targetMethod.targetObject, component);

	            // show method parameters if selected
	            if (method != null)
	            {
	                objSerializedTarget.ApplyModifiedProperties();
	                ShowParameters(objSerializedTarget, targetMethod.targetObject, method);
	                ShowReturnValue(objSerializedTarget, method);
	            }
	        }
	    }

	    private string ShowComponents(SerializedObject objTarget, GameObject gameObject)
	    {
	        var targetComponentAssemblyName = objTarget.FindProperty("targetComponentAssemblyName");
	        var targetComponentFullname = objTarget.FindProperty("targetComponentFullname");
	        var targetComponentText = objTarget.FindProperty("targetComponentText");
	        var objComponents = gameObject.GetComponents<Component>();
	        var objTypesAssemblynames = (from objComp in objComponents select objComp.GetType().AssemblyQualifiedName).ToList();
	        var objTypesName = (from objComp in objComponents select objComp.GetType().Name).ToList();

	        int index = objTypesAssemblynames.IndexOf(targetComponentAssemblyName.stringValue);

	        index = EditorGUILayout.Popup("Target Component", index, objTypesName.ToArray());

	        if (index != -1)
	        {
	            targetComponentAssemblyName.stringValue = objTypesAssemblynames[index];
	            targetComponentFullname.stringValue = objComponents.GetType().FullName;
	            targetComponentText.stringValue = objTypesName[index];
	        }
	        else
	        {
	            targetComponentAssemblyName.stringValue = null;
	        }

	        objTarget.ApplyModifiedProperties();

	        return targetComponentAssemblyName.stringValue;
	    }

	    private MethodInfo ShowMethods(SerializedObject objTarget, GameObject gameObject, string component)
	    {
	        MethodInfo result = null;

	        var targetMethodProp = objTarget.FindProperty("targetMethod");
	        var targetMethodTextProp = objTarget.FindProperty("targetMethodText");
	        var methodParametersProp = objTarget.FindProperty("methodParameters");
			var showInheritedProp = objTarget.FindProperty("showInherited");
	        var saveReturnValueProp = objTarget.FindProperty("saveReturnValue");
	        var returnValueKeyProp = objTarget.FindProperty("returnValueVariableKey");

	        var objComponent = gameObject.GetComponent(ReflectionHelper.GetType(component));
	        var bindingFlags = BindingFlags.Default | BindingFlags.Public | BindingFlags.Instance;

	        if (!showInheritedProp.boolValue)
	        {
	            bindingFlags |= BindingFlags.DeclaredOnly;
	        }

	        if (objComponent != null)
	        {
	            var objMethods = objComponent.GetType().GetMethods(bindingFlags);
	            var methods = (from objMethod in objMethods where !objMethod.IsSpecialName select objMethod).ToList(); // filter out the getter/setter methods
	            var methodText = (from objMethod in methods select objMethod.Name + FormatParameters(objMethod.GetParameters()) + ": " + objMethod.ReturnType.Name).ToList();
	            int index = methodText.IndexOf(targetMethodTextProp.stringValue);

	            index = EditorGUILayout.Popup("Target Method", index, methodText.ToArray());

				EditorGUILayout.PropertyField(showInheritedProp);

	            if (index != -1)
	            {
	                if (targetMethodTextProp.stringValue != methodText[index])
	                {
	                    // reset
	                    methodParametersProp.ClearArray();
	                    methodParametersProp.arraySize = methods[index].GetParameters().Length;

	                    saveReturnValueProp.boolValue = false;
	                    returnValueKeyProp.stringValue = null;
	                }

	                targetMethodTextProp.stringValue = methodText[index];
	                targetMethodProp.stringValue = methods[index].Name;

	                result = methods[index];
	            }
	            else
	            {
	                targetMethodTextProp.stringValue = null;
	                targetMethodProp.stringValue = null;
	            }

	            objTarget.ApplyModifiedProperties();
	        }

	        return result;
	    }

	    private void ShowParameters(SerializedObject objTarget, GameObject gameObject, MethodInfo method)
	    {
	        var methodParametersProp = objTarget.FindProperty("methodParameters");
	        var objParams = method.GetParameters();

	        if (objParams.Length > 0)
	        {
	            GUILayout.Space(20);
	            EditorGUILayout.LabelField("Parameters", EditorStyles.boldLabel);
	            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

	            for (int i = 0; i < objParams.Length; i++)
	            {
	                var objParam = objParams[i];

	                GUILayout.BeginHorizontal();
	                string labelFormat = string.Format("{0}: {1}", objParam.ParameterType.Name, objParam.Name);

	                var objItemProp = methodParametersProp.GetArrayElementAtIndex(i);
	                var serObjValueProp = objItemProp.FindPropertyRelative("objValue");
	                var serVariableKeyProp = objItemProp.FindPropertyRelative("variableKey");
	                var serValueTypeAssemblynameProp = serObjValueProp.FindPropertyRelative("typeAssemblyname");
	                var serValueTypeFullnameProp = serObjValueProp.FindPropertyRelative("typeFullname");

	                serValueTypeAssemblynameProp.stringValue = objParam.ParameterType.AssemblyQualifiedName;
	                serValueTypeFullnameProp.stringValue = objParam.ParameterType.FullName;

	                bool isDrawn = true;

	                if (string.IsNullOrEmpty(serVariableKeyProp.stringValue))
	                {
	                    isDrawn = DrawTypedPropertyInput(labelFormat, serObjValueProp, objParam.ParameterType);
	                }

	                if (isDrawn)
	                {
	                    var vars = GetFungusVariablesByType(targetMethod.GetFlowchart().variables, objParam.ParameterType);
	                    var values = new string[] { "<Value>" };
	                    var displayValue = values.Concat(vars).ToList();

	                    int index = displayValue.IndexOf(serVariableKeyProp.stringValue);

	                    if (index == -1)
	                    {
	                        index = 0;
	                    }

	                    if (string.IsNullOrEmpty(serVariableKeyProp.stringValue))
	                    {
	                        index = EditorGUILayout.Popup(index, displayValue.ToArray(), GUILayout.MaxWidth(80));
	                    }
	                    else
	                    {
	                        index = EditorGUILayout.Popup(labelFormat, index, displayValue.ToArray());
	                    }

	                    if (index > 0)
	                    {
	                        serVariableKeyProp.stringValue = displayValue[index];
	                    }
	                    else
	                    {
	                        serVariableKeyProp.stringValue = null;
	                    }
	                }
	                else
	                {
	                    var style = EditorStyles.label;
	                    var prevColor = style.normal.textColor;
	                    style.normal.textColor = Color.red;
	                    EditorGUILayout.LabelField(new GUIContent(objParam.ParameterType.Name + " cannot be drawn, don´t use this method in the flowchart."), style);
	                    style.normal.textColor = prevColor;
	                }

	                GUILayout.EndHorizontal();
	            }
	            EditorGUILayout.EndVertical();

	            objTarget.ApplyModifiedProperties();
	        }
	    }

	    private void ShowReturnValue(SerializedObject objTarget, MethodInfo method)
	    {
	        var saveReturnValueProp = objTarget.FindProperty("saveReturnValue");
	        var returnValueKeyProp = objTarget.FindProperty("returnValueVariableKey");
	        var returnValueTypeProp = objTarget.FindProperty("returnValueType");
	        var callModeProp = objTarget.FindProperty("callMode");

	        returnValueTypeProp.stringValue = method.ReturnType.FullName;

	        if (method.ReturnType == typeof(IEnumerator))
	        {
	            GUILayout.Space(20);          
	            EditorGUILayout.PropertyField(callModeProp);           
	        }
	        else if (method.ReturnType != typeof(void))
	        {
	            GUILayout.Space(20);
	            EditorGUILayout.LabelField("Return Value", EditorStyles.boldLabel);
	            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
	            saveReturnValueProp.boolValue = EditorGUILayout.Toggle("Save return value", saveReturnValueProp.boolValue);

	            if (saveReturnValueProp.boolValue)
	            {
	                var vars = GetFungusVariablesByType(targetMethod.GetFlowchart().variables, method.ReturnType).ToList();
	                int index = vars.IndexOf(returnValueKeyProp.stringValue);
	                index = EditorGUILayout.Popup(method.ReturnType.Name, index, vars.ToArray());

	                if (index != -1)
	                {
	                    returnValueKeyProp.stringValue = vars[index];                    
	                }
	            }

	            EditorGUILayout.EndVertical();           
	        }
	        else
	        {
	            saveReturnValueProp.boolValue = false;
	        }

	        objTarget.ApplyModifiedProperties();
	    }

	    private bool DrawTypedPropertyInput(string label, SerializedProperty objProperty, Type type)
	    {
	        SerializedProperty objectValue = null;

	        if (type == typeof(int))
	        {
	            objectValue = objProperty.FindPropertyRelative("intValue");
	            objectValue.intValue = EditorGUILayout.IntField(new GUIContent(label), objectValue.intValue);

	            return true;
	        }
	        else if (type == typeof(bool))
	        {
	            objectValue = objProperty.FindPropertyRelative("boolValue");
	            objectValue.boolValue = EditorGUILayout.Toggle(new GUIContent(label), objectValue.boolValue);

	            return true;
	        }
	        else if (type == typeof(float))
	        {
	            objectValue = objProperty.FindPropertyRelative("floatValue");
	            objectValue.floatValue = EditorGUILayout.FloatField(new GUIContent(label), objectValue.floatValue);

	            return true;
	        }
	        else if (type == typeof(string))
	        {
	            objectValue = objProperty.FindPropertyRelative("stringValue");
	            objectValue.stringValue = EditorGUILayout.TextField(new GUIContent(label), objectValue.stringValue);

	            return true;
	        }
	        else if (type == typeof(Color))
	        {
	            objectValue = objProperty.FindPropertyRelative("colorValue");
	            objectValue.colorValue = EditorGUILayout.ColorField(new GUIContent(label), objectValue.colorValue);

	            return true;
	        }
	        else if (type == typeof(UnityEngine.GameObject))
	        {
	            objectValue = objProperty.FindPropertyRelative("gameObjectValue");
	            objectValue.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent(label), objectValue.objectReferenceValue, typeof(UnityEngine.GameObject), true);

	            return true;
	        }
	        else if (type == typeof(UnityEngine.Material))
	        {
	            objectValue = objProperty.FindPropertyRelative("materialValue");
	            objectValue.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent(label), objectValue.objectReferenceValue, typeof(UnityEngine.Material), true);

	            return true;
	        }
	        else if (type == typeof(UnityEngine.Sprite))
	        {
	            objectValue = objProperty.FindPropertyRelative("spriteValue");
	            objectValue.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent(label), objectValue.objectReferenceValue, typeof(UnityEngine.Sprite), true);

	            return true;
	        }
	        else if (type == typeof(UnityEngine.Texture))
	        {
	            objectValue = objProperty.FindPropertyRelative("textureValue");
	            objectValue.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent(label), objectValue.objectReferenceValue, typeof(UnityEngine.Texture), true);

	            return true;
	        }
	        else if (type == typeof(UnityEngine.Vector2))
	        {
	            objectValue = objProperty.FindPropertyRelative("vector2Value");
	            objectValue.vector2Value = EditorGUILayout.Vector2Field(new GUIContent(label), objectValue.vector2Value);

	            return true;
	        }
	        else if (type == typeof(UnityEngine.Vector3))
	        {
	            objectValue = objProperty.FindPropertyRelative("vector3Value");
	            objectValue.vector3Value = EditorGUILayout.Vector3Field(new GUIContent(label), objectValue.vector3Value);

	            return true;
	        }
	        else if (type.IsSubclassOf(typeof(UnityEngine.Object)))
	        {
	            objectValue = objProperty.FindPropertyRelative("objectValue");
	            objectValue.objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent(label), objectValue.objectReferenceValue, type, true);

	            return true;
	        }
	        else if (type.IsEnum)
	        {
	            var enumNames = Enum.GetNames(type);
	            objectValue = objProperty.FindPropertyRelative("intValue");
	            objectValue.intValue = EditorGUILayout.Popup(label, objectValue.intValue, enumNames);

	            return true;
	        }

	        return false;
	    }

	    private string[] GetFungusVariablesByType(List<Variable> variables, Type type)
	    {
	        string[] result = new string[0];

	        if (type == typeof(int))
	        {
	            result = (from v in variables where v.GetType() == typeof(IntegerVariable) select v.key).ToArray();
	        }
	        else if (type == typeof(bool))
	        {
	            result = (from v in variables where v.GetType() == typeof(BooleanVariable) select v.key).ToArray();
	        }
	        else if (type == typeof(float))
	        {
	            result = (from v in variables where v.GetType() == typeof(FloatVariable) select v.key).ToArray();
	        }
	        else if (type == typeof(string))
	        {
	            result = (from v in variables where v.GetType() == typeof(StringVariable) select v.key).ToArray();
	        }
	        else if (type == typeof(Color))
	        {
	            result = (from v in variables where v.GetType() == typeof(ColorVariable) select v.key).ToArray();
	        }
	        else if (type == typeof(UnityEngine.GameObject))
	        {
	            result = (from v in variables where v.GetType() == typeof(GameObjectVariable) select v.key).ToArray();
	        }
	        else if (type == typeof(UnityEngine.Material))
	        {
	            result = (from v in variables where v.GetType() == typeof(MaterialVariable) select v.key).ToArray();
	        }
	        else if (type == typeof(UnityEngine.Sprite))
	        {
	            result = (from v in variables where v.GetType() == typeof(SpriteVariable) select v.key).ToArray();
	        }
	        else if (type == typeof(UnityEngine.Texture))
	        {
	            result = (from v in variables where v.GetType() == typeof(TextureVariable) select v.key).ToArray();
	        }
	        else if (type == typeof(UnityEngine.Vector2))
	        {
	            result = (from v in variables where v.GetType() == typeof(Vector2Variable) select v.key).ToArray();
	        }
	        else if (type == typeof(UnityEngine.Vector3))
	        {
	            result = (from v in variables where v.GetType() == typeof(Vector3Variable) select v.key).ToArray();
	        }
	        else if (type.IsSubclassOf(typeof(UnityEngine.Object)))
	        {
	            result = (from v in variables where v.GetType() == typeof(ObjectVariable) select v.key).ToArray();
	        }

	        return result;
	    }

	    private string FormatParameters(ParameterInfo[] paramInfo)
	    {
	        string result = " (";

	        for (int i = 0; i < paramInfo.Length; i++)
	        {
	            var pi = paramInfo[i];
	            result += pi.ParameterType.Name; // " arg" + (i + 1); 

	            if (i < paramInfo.Length - 1)
	            {
	                result += ", ";
	            }
	        }

	        return result + ")";
	    }

	    private object GetDefaultValue(Type t)
	    {
	        if (t.IsValueType)
	            return Activator.CreateInstance(t);

	        return null;
	    }
	}

}
// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using MarkerMetro.Unity.WinLegacy.Reflection;

namespace Fungus
{
    /// <summary>
    /// Invokes a method of a component via reflection. Supports passing multiple parameters and storing returned values in a Fungus variable.
    /// </summary>
    [CommandInfo("Scripting", 
                 "Invoke Method", 
                 "Invokes a method of a component via reflection. Supports passing multiple parameters and storing returned values in a Fungus variable.")]
    public class InvokeMethod : Command
    {
        [Tooltip("GameObject containing the component method to be invoked")]
        [SerializeField] protected GameObject targetObject;

        [HideInInspector]
        [Tooltip("Name of assembly containing the target component")]
        [SerializeField] protected string targetComponentAssemblyName;

        [HideInInspector]
        [Tooltip("Full name of the target component")]
        [SerializeField] protected string targetComponentFullname;

        [HideInInspector]
        [Tooltip("Display name of the target component")]
        [SerializeField] protected string targetComponentText;

        [HideInInspector]
        [Tooltip("Name of target method to invoke on the target component")]
        [SerializeField] protected string targetMethod;

        [HideInInspector]
        [Tooltip("Display name of target method to invoke on the target component")]
        [SerializeField] protected string targetMethodText;

        [HideInInspector]
        [Tooltip("List of parameters to pass to the invoked method")]
        [SerializeField] protected InvokeMethodParameter[] methodParameters;

        [HideInInspector]
        [Tooltip("If true, store the return value in a flowchart variable of the same type.")]
        [SerializeField] protected bool saveReturnValue;

        [HideInInspector]
        [Tooltip("Name of Fungus variable to store the return value in")]
        [SerializeField] protected string returnValueVariableKey;

        [HideInInspector]
        [Tooltip("The type of the return value")]
        [SerializeField] protected string returnValueType;

        [HideInInspector]
        [Tooltip("If true, list all inherited methods for the component")]
        [SerializeField] protected bool showInherited;

        [HideInInspector]
        [Tooltip("The coroutine call behavior for methods that return IEnumerator")]
        [SerializeField] protected CallMode callMode;

        protected Type componentType;
        protected Component objComponent;
        protected Type[] parameterTypes = null;
        protected MethodInfo objMethod;

        protected virtual void Awake()
        {
            if (componentType == null)
            {
                componentType = ReflectionHelper.GetType(targetComponentAssemblyName);
            }

            if (objComponent == null)
            {
                objComponent = targetObject.GetComponent(componentType);
            }

            if (parameterTypes == null)
            {
                parameterTypes = GetParameterTypes();
            }

            if (objMethod == null)
            {
                objMethod = UnityEvent.GetValidMethodInfo(objComponent, targetMethod, parameterTypes);
            }
        }

        protected virtual IEnumerator ExecuteCoroutine()
        {
            yield return StartCoroutine((IEnumerator)objMethod.Invoke(objComponent, GetParameterValues()));

            if (callMode == CallMode.WaitUntilFinished)
            {
                Continue();
            }
        }

        protected virtual System.Type[] GetParameterTypes()
        {
            System.Type[] types = new System.Type[methodParameters.Length];

            for (int i = 0; i < methodParameters.Length; i++)
            {
                var item = methodParameters[i];
                var objType = ReflectionHelper.GetType(item.objValue.typeAssemblyname);

                types[i] = objType;
            }

            return types;
        }

        protected virtual object[] GetParameterValues()
        {
            object[] values = new object[methodParameters.Length];
            var flowChart = GetFlowchart();

            for (int i = 0; i < methodParameters.Length; i++)
            {
                var item = methodParameters[i];

                if (string.IsNullOrEmpty(item.variableKey))
                {
                    values[i] = item.objValue.GetValue();
                }
                else
                {
                    object objValue = null;

                    switch (item.objValue.typeFullname)
                    {
                        case "System.Int32":
                            var intvalue = flowChart.GetVariable<IntegerVariable>(item.variableKey);
                            if (intvalue != null)
                                objValue = intvalue.Value;
                            break;
                        case "System.Boolean":
                            var boolean = flowChart.GetVariable<BooleanVariable>(item.variableKey);
                            if (boolean != null)
                                objValue = boolean.Value;
                            break;
                        case "System.Single":
                            var floatvalue = flowChart.GetVariable<FloatVariable>(item.variableKey);
                            if (floatvalue != null)
                                objValue = floatvalue.Value;
                            break;
                        case "System.String":
                            var stringvalue = flowChart.GetVariable<StringVariable>(item.variableKey);
                            if (stringvalue != null)
                                objValue = stringvalue.Value;
                            break;
                        case "UnityEngine.Color":
                            var color = flowChart.GetVariable<ColorVariable>(item.variableKey);
                            if (color != null)
                                objValue = color.Value;
                            break;
                        case "UnityEngine.GameObject":
                            var gameObject = flowChart.GetVariable<GameObjectVariable>(item.variableKey);
                            if (gameObject != null)
                                objValue = gameObject.Value;
                            break;
                        case "UnityEngine.Material":
                            var material = flowChart.GetVariable<MaterialVariable>(item.variableKey);
                            if (material != null)
                                objValue = material.Value;
                            break;
                        case "UnityEngine.Sprite":
                            var sprite = flowChart.GetVariable<SpriteVariable>(item.variableKey);
                            if (sprite != null)
                                objValue = sprite.Value;
                            break;
                        case "UnityEngine.Texture":
                            var texture = flowChart.GetVariable<TextureVariable>(item.variableKey);
                            if (texture != null)
                                objValue = texture.Value;
                            break;
                        case "UnityEngine.Vector2":
                            var vector2 = flowChart.GetVariable<Vector2Variable>(item.variableKey);
                            if (vector2 != null)
                                objValue = vector2.Value;
                            break;
                        case "UnityEngine.Vector3":
                            var vector3 = flowChart.GetVariable<Vector3Variable>(item.variableKey);
                            if (vector3 != null)
                                objValue = vector3.Value;
                            break;
                        default:
                            var obj = flowChart.GetVariable<ObjectVariable>(item.variableKey);
                            if (obj != null)
                                objValue = obj.Value;
                            break;
                    }

                    values[i] = objValue;
                }
            }

            return values;
        }

        protected virtual void SetVariable(string key, object value, string returnType)
        {
            var flowChart = GetFlowchart();

            switch (returnType)
            {
                case "System.Int32":
                    flowChart.GetVariable<IntegerVariable>(key).Value = (int)value;
                    break;
                case "System.Boolean":
                    flowChart.GetVariable<BooleanVariable>(key).Value = (bool)value;
                    break;
                case "System.Single":
                    flowChart.GetVariable<FloatVariable>(key).Value = (float)value;
                    break;
                case "System.String":
                    flowChart.GetVariable<StringVariable>(key).Value = (string)value;
                    break;
                case "UnityEngine.Color":
                    flowChart.GetVariable<ColorVariable>(key).Value = (UnityEngine.Color)value;
                    break;
                case "UnityEngine.GameObject":
                    flowChart.GetVariable<GameObjectVariable>(key).Value = (UnityEngine.GameObject)value;
                    break;
                case "UnityEngine.Material":
                    flowChart.GetVariable<MaterialVariable>(key).Value = (UnityEngine.Material)value;
                    break;
                case "UnityEngine.Sprite":
                    flowChart.GetVariable<SpriteVariable>(key).Value = (UnityEngine.Sprite)value;
                    break;
                case "UnityEngine.Texture":
                    flowChart.GetVariable<TextureVariable>(key).Value = (UnityEngine.Texture)value;
                    break;
                case "UnityEngine.Vector2":
                    flowChart.GetVariable<Vector2Variable>(key).Value = (UnityEngine.Vector2)value;
                    break;
                case "UnityEngine.Vector3":
                    flowChart.GetVariable<Vector3Variable>(key).Value = (UnityEngine.Vector3)value;
                    break;
                default:
                    flowChart.GetVariable<ObjectVariable>(key).Value = (UnityEngine.Object)value;
                    break;
            }
        }

        #region Public members

        /// <summary>
        /// GameObject containing the component method to be invoked.
        /// </summary>
        public virtual GameObject TargetObject { get { return targetObject; } }

        public override void OnEnter()
        {
            try
            {
                if (targetObject == null || string.IsNullOrEmpty(targetComponentAssemblyName) || string.IsNullOrEmpty(targetMethod))
                {
                    Continue();
                    return;
                }

                if (returnValueType != "System.Collections.IEnumerator")
                {
                    var objReturnValue = objMethod.Invoke(objComponent, GetParameterValues());

                    if (saveReturnValue)
                    {
                        SetVariable(returnValueVariableKey, objReturnValue, returnValueType);
                    }

                    Continue();
                }
                else
                {
                    StartCoroutine(ExecuteCoroutine());

                    if (callMode == CallMode.Continue)
                    {
                        Continue();
                    }
                    else if(callMode == CallMode.Stop)
                    {
                        StopParentBlock();
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error: " + ex.Message);
            }      
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override string GetSummary()
        {
            if (targetObject == null)
            {
                return "Error: targetObject is not assigned";
            }

            return targetObject.name + "." + targetComponentText + "." + targetMethodText;
        }

        #endregion
    }

    [System.Serializable]
    public class InvokeMethodParameter
    {
        [SerializeField]
        public ObjectValue objValue;

        [SerializeField]
        public string variableKey;
    }

    [System.Serializable]
    public class ObjectValue
    {
        public string typeAssemblyname;
        public string typeFullname;

        public int intValue;
        public bool boolValue;
        public float floatValue;
        public string stringValue;

        public Color colorValue;
        public GameObject gameObjectValue;
        public Material materialValue;
        public UnityEngine.Object objectValue;
        public Sprite spriteValue;
        public Texture textureValue;
        public Vector2 vector2Value;
        public Vector3 vector3Value;

        public object GetValue()
        {
            switch (typeFullname)
            {
            case "System.Int32":
                return intValue;
            case "System.Boolean":
                return boolValue;
            case "System.Single":
                return floatValue;
            case "System.String":
                return stringValue;
            case "UnityEngine.Color":
                return colorValue;
            case "UnityEngine.GameObject":
                return gameObjectValue;
            case "UnityEngine.Material":
                return materialValue;
            case "UnityEngine.Sprite":
                return spriteValue;
            case "UnityEngine.Texture":
                return textureValue;
            case "UnityEngine.Vector2":
                return vector2Value;
            case "UnityEngine.Vector3":
                return vector3Value;
            default:
                var objType = ReflectionHelper.GetType(typeAssemblyname);

                if (objType.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    return objectValue;
                }
                else if (objType.IsEnum())
                    return System.Enum.ToObject(objType, intValue);

                break;
            }

            return null;
        }
    }

    public static class ReflectionHelper
    {
        static Dictionary<string, System.Type> types = new Dictionary<string, System.Type>();

        public static System.Type GetType(string typeName)
        {
            if (types.ContainsKey(typeName))
                return types[typeName];

            types[typeName] = System.Type.GetType(typeName);

            return types[typeName];
        }
    }
}

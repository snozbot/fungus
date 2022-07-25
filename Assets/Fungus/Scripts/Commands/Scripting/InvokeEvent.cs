// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System;
using UnityEngine.Events;

namespace Fungus
{
    /// <summary>
    /// Supported types of method invocation.
    /// </summary>
    public enum InvokeType
    {
        /// <summary> Call a method with an optional constant value parameter. </summary>
        Static,         // 
        /// <summary> Call a method with an optional boolean constant / variable parameter. </summary>
        DynamicBoolean,
        /// <summary> Call a method with an optional integer constant / variable parameter. </summary>
        DynamicInteger,
        /// <summary> Call a method with an optional float constant / variable parameter. </summary>
        DynamicFloat,
        /// <summary> Call a method with an optional string constant / variable parameter. </summary>
        DynamicString
    }

    /// <summary>
    /// Calls a list of component methods via the Unity Event System (as used in the Unity UI)
    /// This command is more efficient than the Invoke Method command but can only pass a single parameter and doesn't support return values.
    /// This command uses the UnityEvent system to call methods in script. http://docs.unity3d.com/Manual/UnityEvents.html
    /// </summary>
    [CommandInfo("Scripting", 
                 "Invoke Event", 
                 "Calls a list of component methods via the Unity Event System (as used in the Unity UI). " + 
                 "This command is more efficient than the Invoke Method command but can only pass a single parameter and doesn't support return values.")]
    [AddComponentMenu("")]
    public class InvokeEvent : Command
    {
        [Tooltip("A description of what this command does. Appears in the command summary.")]
        [SerializeField] protected string description = "";

        [Tooltip("Delay (in seconds) before the methods will be called")]
        [SerializeField] protected float delay;

        [Tooltip("Selects type of method parameter to pass")]
        [SerializeField] protected InvokeType invokeType;

        [Tooltip("List of methods to call. Supports methods with no parameters or exactly one string, int, float or object parameter.")]
        [SerializeField] protected UnityEvent staticEvent = new UnityEvent();

        [Tooltip("Boolean parameter to pass to the invoked methods.")]
        [SerializeField] protected BooleanData booleanParameter;

        [Tooltip("List of methods to call. Supports methods with one boolean parameter.")]
        [SerializeField] protected BooleanEvent booleanEvent = new BooleanEvent();

        [Tooltip("Integer parameter to pass to the invoked methods.")]
        [SerializeField] protected IntegerData integerParameter;
        
        [Tooltip("List of methods to call. Supports methods with one integer parameter.")]
        [SerializeField] protected IntegerEvent integerEvent = new IntegerEvent();

        [Tooltip("Float parameter to pass to the invoked methods.")]
        [SerializeField] protected FloatData floatParameter;
        
        [Tooltip("List of methods to call. Supports methods with one float parameter.")]
        [SerializeField] protected FloatEvent floatEvent = new FloatEvent();

        [Tooltip("String parameter to pass to the invoked methods.")]
        [SerializeField] protected StringDataMulti stringParameter;

        [Tooltip("List of methods to call. Supports methods with one string parameter.")]
        [SerializeField] protected StringEvent stringEvent = new StringEvent();

        protected virtual void DoInvoke()
        {
            switch (invokeType)
            {
                default:
                case InvokeType.Static:
                    staticEvent.Invoke();
                    break;
                case InvokeType.DynamicBoolean:
                    booleanEvent.Invoke(booleanParameter.Value);
                    break;
                case InvokeType.DynamicInteger:
                    integerEvent.Invoke(integerParameter.Value);
                    break;
                case InvokeType.DynamicFloat:
                    floatEvent.Invoke(floatParameter.Value);
                    break;
                case InvokeType.DynamicString:
                    stringEvent.Invoke(stringParameter.Value);
                    break;
            }
        }

        #region Public members

        [Serializable] public class BooleanEvent : UnityEvent<bool> {}
        [Serializable] public class IntegerEvent : UnityEvent<int> {}
        [Serializable] public class FloatEvent : UnityEvent<float> {}
        [Serializable] public class StringEvent : UnityEvent<string> {}

        public override void OnEnter()
        {
            if (Mathf.Approximately(delay, 0f))
            {
                DoInvoke();
            }
            else
            {
                Invoke("DoInvoke", delay);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (!string.IsNullOrEmpty(description))
            {
                return description;
            }

            string summary = invokeType.ToString() + " ";

            switch (invokeType)
            {
            default:
            case InvokeType.Static:
                summary += staticEvent.GetPersistentEventCount();
                break;
            case InvokeType.DynamicBoolean:
                summary += booleanEvent.GetPersistentEventCount();
                break;
            case InvokeType.DynamicInteger:
                summary += integerEvent.GetPersistentEventCount();
                break;
            case InvokeType.DynamicFloat:
                summary += floatEvent.GetPersistentEventCount();
                break;
            case InvokeType.DynamicString:
                summary += stringEvent.GetPersistentEventCount();
                break;
            }

            return summary + " methods";
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return booleanParameter.booleanRef == variable || integerParameter.integerRef == variable ||
                floatParameter.floatRef == variable || stringParameter.stringRef == variable ||
                base.HasReference(variable);
        }

        #endregion
    }
}
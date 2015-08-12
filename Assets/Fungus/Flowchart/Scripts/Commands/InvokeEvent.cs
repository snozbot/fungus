using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Invoke Event", 
	             "Calls a list of component methods via the Unity Event System (as used in the Unity UI). " + 
	             "This command is more efficient than the Invoke Method command but can only pass a single parameter and doesn't support return values.")]
	[AddComponentMenu("")]

	// This command uses the UnityEvent system to call methods in script.
	// http://docs.unity3d.com/Manual/UnityEvents.html
	public class InvokeEvent : Command
	{
		[Serializable] public class BooleanEvent : UnityEvent<bool> {}
		[Serializable] public class IntegerEvent : UnityEvent<int> {}
		[Serializable] public class FloatEvent : UnityEvent<float> {}
		[Serializable] public class StringEvent : UnityEvent<string> {}

		public enum InvokeType
		{
			Static,			// Call a method with an optional constant value parameter
			DynamicBoolean,	// Call a method with an optional boolean constant / variable parameter
			DynamicInteger,	// Call a method with an optional integer constant / variable parameter
			DynamicFloat,	// Call a method with an optional float constant / variable parameter
			DynamicString	// Call a method with an optional string constant / variable parameter
		}

		[Tooltip("Delay (in seconds) before the methods will be called")]
		public float delay;

		public InvokeType invokeType;

		[Tooltip("List of methods to call. Supports methods with no parameters or exactly one string, int, float or object parameter.")]
		public UnityEvent staticEvent = new UnityEvent();

		[Tooltip("Boolean parameter to pass to the invoked methods.")]
		public BooleanData booleanParameter;

		[Tooltip("List of methods to call. Supports methods with one boolean parameter.")]
		public BooleanEvent booleanEvent = new BooleanEvent();

		[Tooltip("Integer parameter to pass to the invoked methods.")]
		public IntegerData integerParameter;
		
		[Tooltip("List of methods to call. Supports methods with one integer parameter.")]
		public IntegerEvent integerEvent = new IntegerEvent();

		[Tooltip("Float parameter to pass to the invoked methods.")]
		public FloatData floatParameter;
		
		[Tooltip("List of methods to call. Supports methods with one float parameter.")]
		public FloatEvent floatEvent = new FloatEvent();

		[Tooltip("String parameter to pass to the invoked methods.")]
		public StringData stringParameter;

		[Tooltip("List of methods to call. Supports methods with one string parameter.")]
		public StringEvent stringEvent = new StringEvent();

		public override void OnEnter()
		{
			if (delay == 0f)
			{
				DoInvoke();
			}
			else
			{
				Invoke("DoInvoke", delay);
			}

			Continue();
		}

		protected virtual void DoInvoke()
		{
			switch (invokeType)
			{
			default:
			case InvokeType.Static:
				staticEvent.Invoke();
				break;
			case InvokeType.DynamicBoolean:
				booleanEvent.Invoke(booleanParameter);
				break;
			case InvokeType.DynamicInteger:
				integerEvent.Invoke(integerParameter);
				break;
			case InvokeType.DynamicFloat:
				floatEvent.Invoke(floatParameter);
				break;
			case InvokeType.DynamicString:
				stringEvent.Invoke(stringParameter);
				break;
			}
		}

		public override string GetSummary()
		{
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
	}

}
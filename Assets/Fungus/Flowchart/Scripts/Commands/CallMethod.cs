/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections;

namespace Fungus
{
	// This command is called "Call Method" because a) it's more descriptive than Send Message and we're already have
	// a Send Message command for sending messages to trigger block execution.

	[CommandInfo("Scripting", 
	             "Call Method", 
	             "Calls a named method on a GameObject using the GameObject.SendMessage() system.")]
	[AddComponentMenu("")]
	public class CallMethod : Command
	{
		[Tooltip("Target monobehavior which contains the method we want to call")]
		public GameObject targetObject;

		[Tooltip("Name of the method to call")]
		public string methodName = "";

		[Tooltip("Delay (in seconds) before the method will be called")]
		public float delay;

		public override void OnEnter()
		{
			if (targetObject == null ||
			    methodName.Length == 0)
			{
				Continue();
				return;
			}

			if (delay == 0f)
			{
				CallTheMethod();
			}
			else
			{
				Invoke("CallTheMethod", delay);
			}

			Continue();
		}

		protected virtual void CallTheMethod()
		{
			targetObject.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
		}

		public override string GetSummary()
		{
			if (targetObject == null)
			{
				return "Error: No target GameObject specified";
			}

			if (methodName.Length == 0)
			{
				return "Error: No named method specified";
			}

			return targetObject.name + " : " + methodName;
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}
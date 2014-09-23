using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Fungus
{
	/*
	[CommandInfo("Scripting", 
	             "Send Event", 
	             "Send an event to a gameobject in the scene. Use this to call any public method or set public properties on any game object.")]
	public class SendEvent : Command
	{	
		public UnityEvent targetEvent;

		public bool stopCurrentScript = false;
	
		public override void OnEnter()
		{
			if (targetEvent != null)
			{
				if (stopCurrentScript)
				{
					Stop();
				}

				targetEvent.Invoke();

				if (!stopCurrentScript)
				{
					Continue();
				}
			}
			else
			{		
				Continue();
			}
		}

		public override string GetSummary()
		{
			if (targetEvent == null ||
			    targetEvent.GetPersistentEventCount() == 0)
			{
				return "<Continue>";
			}

			UnityEngine.Object obj = targetEvent.GetPersistentTarget(0);
			if (obj == null)
			{
				return "Error: No target object selected";
			}

			return obj.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}
	*/
}
using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Scripting", 
	             "Send Message", 
	             "Sends a message to either the owner Fungus Script or all Fungus Scripts in the scene. Sequences can listen for this message to start execution.")]
	public class SendMessage : Command
	{
		public enum MessageTarget
		{
			SameScript,
			AllScripts
		}

		public MessageTarget messageTarget;
		public string message = "";

		public override void OnEnter()
		{
			if (message.Length == 0)
			{
				Continue();
			}

			FungusScript fungusScript = GetFungusScript();

			MessageReceived[] receivers = null;
			if (messageTarget == MessageTarget.SameScript)
			{
				receivers = fungusScript.GetComponentsInChildren<MessageReceived>();
			}
			else
			{
				receivers = GameObject.FindObjectsOfType<MessageReceived>();
			}

			if (receivers != null)
			{
				foreach (MessageReceived receiver in receivers)
				{
					receiver.OnSendFungusMessage(message);
				}
			}

			Continue();
		}

		public override string GetSummary()
		{
			if (message.Length == 0)
			{
				return "Error: No message specified";
			}
			
			return message;
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}
	}

}
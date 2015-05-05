using UnityEngine;
using System.Collections;

namespace Fungus
{
	[EventHandlerInfo("",
	                  "Message Received",
	                  "The block will execute when the specified message is received from a Send Message command.")]
	[AddComponentMenu("")]
	public class MessageReceived : EventHandler 
	{
		[Tooltip("Fungus message to listen for")]
		public string message = "";

		public void OnSendFungusMessage(string message)
		{
			if (this.message == message)
			{
				ExecuteBlock();
			}
		}

		public override string GetSummary()
		{
			return message;
		}
	}

}
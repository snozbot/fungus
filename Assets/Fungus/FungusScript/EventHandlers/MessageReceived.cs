using UnityEngine;
using System.Collections;

namespace Fungus
{
	[EventHandlerInfo("",
	                  "Message Received",
	                  "The sequence will execute when the specified message is received from a Send Message command.")]
	public class MessageReceived : EventHandler 
	{
		public string message;

		public void OnSendFungusMessage(string message)
		{
			if (this.message == message)
			{
				ExecuteSequence();
			}
		}
	}

}
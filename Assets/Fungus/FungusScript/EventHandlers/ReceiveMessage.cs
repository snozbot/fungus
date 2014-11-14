using UnityEngine;
using System.Collections;

namespace Fungus
{
	[EventHandlerInfo("",
	                  "Receive Message",
	                  "The sequence will execute when the specified message is received from a SendFungusMessage command.")]
	public class ReceiveMessage : EventHandler 
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
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Flow", 
	             "Send Message", 
	             "Sends a message to either the owner Flowchart or all Flowcharts in the scene. Blocks can listen for this message using a Message Received event handler.")]
	[AddComponentMenu("")]
	public class SendMessage : Command, ISerializationCallbackReceiver
	{
		#region Obsolete Properties
		[HideInInspector] [FormerlySerializedAs("message")] public string messageOLD;
		#endregion

		public enum MessageTarget
		{
			SameFlowchart,
			AllFlowcharts
		}

		[Tooltip("Target flowchart(s) to send the message to")]
		public MessageTarget messageTarget;

		[Tooltip("Name of the message to send")]
		public StringData _message = new StringData("");

		public override void OnEnter()
		{
			if (_message.Value.Length == 0)
			{
				Continue();
				return;
			}

			Flowchart flowchart = GetFlowchart();

			MessageReceived[] receivers = null;
			if (messageTarget == MessageTarget.SameFlowchart)
			{
				receivers = flowchart.GetComponentsInChildren<MessageReceived>();
			}
			else
			{
				receivers = GameObject.FindObjectsOfType<MessageReceived>();
			}

			if (receivers != null)
			{
				foreach (MessageReceived receiver in receivers)
				{
					receiver.OnSendFungusMessage(_message);
				}
			}

			Continue();
		}

		public override string GetSummary()
		{
			if (_message.Value.Length == 0)
			{
				return "Error: No message specified";
			}
			
			return _message;
		}
		
		public override Color GetButtonColor()
		{
			return new Color32(235, 191, 217, 255);
		}

		//
		// ISerializationCallbackReceiver implementation
		//

		public virtual void OnBeforeSerialize()
		{}

		public virtual void OnAfterDeserialize()
		{
			if (messageOLD != default(string))
			{
				_message.Value = messageOLD;
				messageOLD = default(string);
			}
		}
	}

}
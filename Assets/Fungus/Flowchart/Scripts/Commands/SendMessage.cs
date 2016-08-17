/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
    [CommandInfo("Flow", 
                 "Send Message", 
                 "Sends a message to either the owner Flowchart or all Flowcharts in the scene. Blocks can listen for this message using a Message Received event handler.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SendMessage : Command
    {
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
                receivers = flowchart.GetComponents<MessageReceived>();
            }
            else
            {
                receivers = GameObject.FindObjectsOfType<MessageReceived>();
            }

            if (receivers != null)
            {
                foreach (MessageReceived receiver in receivers)
                {
                    receiver.OnSendFungusMessage(_message.Value);
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
            
            return _message.Value;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("message")] public string messageOLD = "";

        protected virtual void OnEnable()
        {
            if (messageOLD != "")
            {
                _message.Value = messageOLD;
                messageOLD = "";
            }
        }

        #endregion
    }

}
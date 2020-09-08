// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Linq;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the specified message is received from a Send Message command.
    /// </summary>
    [EventHandlerInfo("Scene",
                      "Messages Received",
                      "Like Message Received, but works so this block is executed when it receives any in a group of messages from Send Message.")]
    [AddComponentMenu("")]
    public class MessagesReceived : EventHandler
    {
        [Tooltip("Fungus messages to listen for")]
        [SerializeField] protected string[] messages = null;

        #region Public members

        /// <summary>
        /// Called from Flowchart when a message is sent.
        /// </summary>
        /// <param name="messageToConsider">Message.</param>
        public void OnSendFungusMessage(string messageToConsider)
        {
            if (ShouldRespondToMessage(messageToConsider))
            {
                ExecuteBlock();
            }
        }

        protected virtual bool ShouldRespondToMessage(string messageToConsider)
        {
            for (int i = 0; i < this.messages.Length; i++)
                if (messages[i] == messageToConsider)
                    return true;

            return false;
        }

        /// <summary>
        /// Returns a newline-separated combined string of the messages this is set to respond to.
        /// </summary>
        public override string GetSummary()
        {
            string summary = "";

            for (int i = 0; i < this.messages.Length; i++)
            {
                summary.Concat(this.messages[i]);
                summary.Concat("\n");
            }

            return summary;
        }

        #endregion
    }
}
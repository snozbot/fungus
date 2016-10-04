// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the specified message is received from a Send Message command.
    /// </summary>
    [EventHandlerInfo("",
                      "Message Received",
                      "The block will execute when the specified message is received from a Send Message command.")]
    [AddComponentMenu("")]
    public class MessageReceived : EventHandler 
    {
        [Tooltip("Fungus message to listen for")]
        [SerializeField] protected string message = "";

        #region Public members

        /// <summary>
        /// Called from Flowchart when a message is sent.
        /// </summary>
        /// <param name="message">Message.</param>
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

        #endregion
    }
}
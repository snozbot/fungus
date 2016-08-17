/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
    [CommandInfo("Narrative", 
                 "Conversation", 
                 "Do multiple say and portrait commands in a single block of text. Format is: [character] [portrait] [stage position] [: Story text]")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class Conversation : Command
    {
        public StringDataMulti conversationText;

        public ConversationManager conversationManager = new ConversationManager();

        protected virtual void Start()
        {
            conversationManager.PopulateCharacterCache();
        }

        public override void OnEnter()
        {
            StartCoroutine(DoConversation());
        }

        protected virtual IEnumerator DoConversation()
        {
            Flowchart flowchart = GetFlowchart();
            string subbedText = flowchart.SubstituteVariables(conversationText.Value);

            yield return StartCoroutine(conversationManager.DoConversation(subbedText));

            Continue();
        }

        public override string GetSummary()
        {
            return conversationText.Value;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }
    }
}
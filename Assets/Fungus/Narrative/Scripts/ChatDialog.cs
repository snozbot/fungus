/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

	public class ChatDialog : SayDialog
	{

		//This will hold the horizontal layout that scrolls through the chat dialog
		public Canvas chatCanvas;

		public List<ChatBlock> chatBlocks;

		protected GameObject chatBlockPrefab;

		protected GameObject currentChatBlockObject;

		protected ChatBlock currentChatBlock;

		
		public void Awake()
		{
			Debug.Log("awake");
			//limit to how many to have at a time for now since it will scroll too far to show.
			//TODO prob don't need this.
			chatBlocks = new List<ChatBlock>(20);
			chatBlockPrefab = Resources.Load<GameObject>("ChatBlock");
		}

		public override IEnumerator SayInternal(string text, bool clearPrevious, bool waitForInput, bool fadeWhenDone, bool stopVoiceover, AudioClip voiceOverClip, Action onComplete)
		{
			Writer writer = GetWriter();

			if (writer.isWriting || writer.isWaitingForInput)
			{
				writer.Stop();
				while (writer.isWriting || writer.isWaitingForInput)
				{
					yield return null;
				}
			}
			
			//spawn a new chat block (prob make this a prefab) containing avatar, name and dialog text 
			if (chatBlockPrefab != null)
			{
				currentChatBlockObject = Instantiate(chatBlockPrefab) as GameObject;
				currentChatBlockObject.name = "ChatBlock";
				currentChatBlock = currentChatBlockObject.GetComponent<ChatBlock>();
				chatBlocks.Add(currentChatBlock);
			}

			//TODO move previous chat blocks up with smooth animation

			//TODO Get the most recent chat block spawned and find it's dialog text for the writer

			//TODO Set Writer.targetTextObject to that spawned text

			writer.targetTextObject = currentChatBlock.chatText;

			gameObject.SetActive(true);

			this.fadeWhenDone = fadeWhenDone;

			// Voice over clip takes precedence over a character sound effect if provided

			AudioClip soundEffectClip = null;
			if (voiceOverClip != null)
			{
				WriterAudio writerAudio = GetWriterAudio();
				writerAudio.PlayVoiceover(voiceOverClip);
			}
			else if (speakingCharacter != null)
			{
				soundEffectClip = speakingCharacter.soundEffect;
			}

			yield return StartCoroutine(writer.Write(text, clearPrevious, waitForInput, stopVoiceover, soundEffectClip, onComplete));
		}

	}

}

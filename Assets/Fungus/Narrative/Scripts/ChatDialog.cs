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

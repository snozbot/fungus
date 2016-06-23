/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Audio", 
	             "Play Sound",
	             "Plays a once-off sound effect. Multiple sound effects can be played at the same time.")]
	[AddComponentMenu("")]
	public class PlaySound : Command
	{
		[Tooltip("Sound effect clip to play")]
		public AudioClip soundClip;

		[Range(0,1)]
		[Tooltip("Volume level of the sound effect")]
		public float volume = 1;

		[Tooltip("Wait until the sound has finished playing before continuing execution.")]
		public bool waitUntilFinished;

		public override void OnEnter()
		{
			if (soundClip == null)
			{
				Continue();
				return;
			}

			MusicController musicController = MusicController.GetInstance();
			if (musicController != null)
			{
				musicController.PlaySound(soundClip, volume);
			}

			if (waitUntilFinished)
			{
				Invoke("DoWait", soundClip.length);
			}
			else
			{
				Continue();
			}
		}

		protected virtual void DoWait()
		{
			Continue();
		}

		public override string GetSummary()
		{
			if (soundClip == null)
			{
				return "Error: No sound clip selected";
			}

			return soundClip.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(242, 209, 176, 255);
		}
	}

}

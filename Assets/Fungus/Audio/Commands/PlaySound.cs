using UnityEngine;
using System.Collections;

namespace Fungus.Script
{
	[CommandName("Play Sound")]
	[HelpText("Plays a sound effect. Multiple sound effects can play at the same time.")]
	public class PlaySound : FungusCommand
	{
		public AudioClip soundClip;

		[Range(0,1)]
		public float volume = 1;

		public override void OnEnter()
		{
			MusicController musicController = MusicController.GetInstance();
			if (musicController != null)
			{
				musicController.PlaySound(soundClip, volume);
			}

			Continue();
		}

		public override string GetSummary()
		{
			if (soundClip == null)
			{
				return "Error: No music clip selected";
			}

			return soundClip.name;
		}
	}

}
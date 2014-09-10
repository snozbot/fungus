using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Audio", 
	             "Play Sound",
	             "Plays a sound effect. Multiple sound effects can play at the same time.")]
	public class PlaySound : Command
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

		public override Color GetButtonColor()
		{
			return new Color32(242, 209, 176, 255);
		}
	}

}
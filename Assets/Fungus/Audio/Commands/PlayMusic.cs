using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Audio",
	             "Play Music",
	             "Plays game music. If any game music is already playing, it is stopped. Music continues playing across scene loads.")]
	public class PlayMusic : Command
	{
		public AudioClip musicClip;

		public override void OnEnter()
		{
			MusicController musicController = MusicController.GetInstance();
			if (musicController != null)
			{
				musicController.PlayMusic(musicClip);
			}

			Continue();
		}

		public override string GetSummary()
		{
			if (musicClip == null)
			{
				return "Error: No music clip selected";
			}

			return musicClip.name;
		}

		public override Color GetButtonColor()
		{
			return new Color32(242, 209, 176, 255);
		}
	}

}
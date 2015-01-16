using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Audio",
	             "Set Audio Volume",
	             "Sets the global volume level for audio played with Play Music and Play Sound commands.")]
	[AddComponentMenu("")]
	public class SetAudioVolume : Command
	{
		[Range(0,1)]
		[Tooltip("Global volume level for audio played using Play Music and Play Sound")]
		public float volume = 1;

		[Range(0,30)]
		[Tooltip("Time to fade between current volume level and target volume level.")]
		public float fadeDuration; 

		public override void OnEnter()
		{
			MusicController musicController = MusicController.GetInstance();
			if (musicController != null)
			{
				musicController.SetAudioVolume(volume, fadeDuration);
			}

			Continue();
		}

		public override string GetSummary()
		{
			return "Set to " + volume + " over " + fadeDuration + " seconds.";
		}

		public override Color GetButtonColor()
		{
			return new Color32(242, 209, 176, 255);
		}
	}

}
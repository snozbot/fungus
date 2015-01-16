using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Audio",
	             "Play Music",
	             "Plays looping game music. If any game music is already playing, it is stopped. Game music will continue playing across scene loads.")]
	[AddComponentMenu("")]
	public class PlayMusic : Command
	{
		[Tooltip("Music sound clip to play")]
		public AudioClip musicClip;

		[Tooltip("Time to begin playing in seconds. If the audio file is compressed, the time index may be inaccurate.")]
		public float atTime;

		public override void OnEnter()
		{
			MusicController musicController = MusicController.GetInstance();
			if (musicController != null)
			{
				float startTime = Mathf.Max (0, atTime);
				musicController.PlayMusic(musicClip, startTime);
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
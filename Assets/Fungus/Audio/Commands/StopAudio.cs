using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Audio", 
	             "Stop Audio", 
	             "Stops all currently playing audio.")]
	public class StopAudio : Command
	{
		public override void OnEnter()
		{
			MusicController musicController = MusicController.GetInstance();
			if (musicController != null)
			{
				musicController.StopAllAudio();
			}

			Continue();
		}

		public override Color GetButtonColor()
		{
			return new Color32(242, 209, 176, 255);
		}
	}

}
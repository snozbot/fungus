using UnityEngine;
using System.Collections;

namespace Fungus
{
	[CommandInfo("Audio", 
	             "Stop Music", 
	             "Stops the currently playing game music.")]
	[AddComponentMenu("")]
	public class StopMusic : Command
	{
		public override void OnEnter()
		{
			MusicController musicController = MusicController.GetInstance();
			if (musicController != null)
			{
				musicController.StopMusic();
			}

			Continue();
		}

		public override Color GetButtonColor()
		{
			return new Color32(242, 209, 176, 255);
		}
	}

}
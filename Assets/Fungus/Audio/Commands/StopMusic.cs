using UnityEngine;
using System.Collections;

namespace Fungus.Script
{
	[CommandCategory("Audio")]
	[CommandName("Stop Music")]
	[HelpText("Stops the currently playing game music.")]
	public class StopMusic : FungusCommand
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
	}

}
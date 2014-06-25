using UnityEngine;
using System.Collections;

namespace Fungus.Example
{
	public class AudioRoom : Room 
	{
		public Room menuRoom;
		public AudioClip musicClip;
		public AudioClip effectClip;

		void OnEnter()
		{
			if (Variables.GetBoolean("music"))
			{
				AddOption("Stop the music", StopGameMusic);

				if (Variables.GetBoolean("quiet") == false)
				{
					AddOption("Shhh! Make it quieter", MakeQuiet);
				}
			}
			else
			{
				AddOption("Play some music", StartGameMusic);
			}
			AddOption("Play a sound effect", PlaySound);
			AddOption("Back to menu", MainMenu);

			Say("We are the music makers, and we are the dreamers of dreams.");
		}

		void StartGameMusic()
		{
			PlayMusic(musicClip);
			SetMusicVolume(1f);
			SetBoolean("music", true);
			Call(OnEnter);
		}

		void StopGameMusic()
		{
			StopMusic();
			SetBoolean("music", false);
			SetBoolean("quiet", false);
			Call(OnEnter);
		}

		void PlaySound()
		{
			PlaySound(effectClip, 1f);
			Call(OnEnter);
		}

		void MakeQuiet()
		{
			SetBoolean("quiet", true);
			SetMusicVolume(0.25f, 1f);
			Call(OnEnter);
		}

		void MainMenu()
		{
			MoveToRoom(menuRoom);
		}
	}
}

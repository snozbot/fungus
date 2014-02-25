using UnityEngine;
using System.Collections;
using Fungus;

public class AudioRoom : Room 
{
	public Room menuRoom;
	public AudioClip musicClip;
	public AudioClip effectClip;

	void OnEnter()
	{
		if (GetFlag("music"))
		{
			AddOption("Stop the music", StopMusic);

			if (GetFlag("quiet") == false)
			{
				AddOption("Shhh! Make it quieter", MakeQuiet);
			}
		}
		else
		{
			AddOption("Play some music", StartMusic);
		}
		AddOption("Play a sound effect", PlaySound);
		AddOption("Back to menu", MainMenu);

		if (IsFirstVisit())
		{
			Choose("We are the music makers, and we are the dreamers of dreams.");
		}
		else
		{
			Choose();
		}
	}

	void StartMusic()
	{
		PlayGameMusic(musicClip);
		SetMusicVolume(1f);
		SetFlag("music", true);
		Call(OnEnter);
	}

	void StopMusic()
	{
		StopGameMusic();
		SetFlag("music", false);
		SetFlag("quiet", false);
		Call(OnEnter);
	}

	void PlaySound()
	{
		PlaySound(effectClip, 1f);
		Call(OnEnter);
	}

	void MakeQuiet()
	{
		SetFlag("quiet", true);
		SetMusicVolume(0.25f, 1f);
		Call(OnEnter);
	}

	void MainMenu()
	{
		MoveToRoom(menuRoom);
	}
}

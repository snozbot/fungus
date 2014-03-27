Using Audio
===========

# How do I play music & sound effects?

1. Add an audio asset to your Unity project (e.g. MP3, WAV file). 
2. Add a public AudioClip property to your Room script and setup the reference to the audio asset in the inspector.
3. Use the [PlayMusic](@ref Fungus.GameController.PlayMusic) command to start music playing.
4. Use the [StopMusic](@ref Fungus.GameController.StopMusic) command to stop music playing.
5. Use [SetMusicVolume](@ref Fungus.GameController.SetMusicVolume) to set the music volume level.
6. Use [PlaySound](@ref Fungus.GameController.PlaySound) to play a one-off sound effect.

## C# code example
~~~~~~~~~~~~~~~~~~~~
using UnityEngine;
using System.Collections;
using Fungus;

public class MyRoom : Room 
{
	public AudioClip musicClip; // A music audio clip
	public AudioClip soundClip; // A sound effect audio clip

	void OnEnter() 
	{
		PlayMusic(musicClip); // Start the music
		Wait(5);
		PlaySound(soundClip); // Play a one-off sound effect
		Wait(5);
		SetMusicVolume(0.5f); // Reduce music volume
		Wait(5);
		StopMusic(); // Stop the music
	}
}
~~~~~~~~~~~~~~~~~~~~

## Notes
- Fungus only provides simple commands for playing AudioClips. 
- For more advanced control over audio you should use the AudioSource component in Unity directly.
- Uncheck the 3D Sound option for each audio asset in the property inspector. 
- 2D Sounds do not attenuate (grow quieter) with distance from the listener.

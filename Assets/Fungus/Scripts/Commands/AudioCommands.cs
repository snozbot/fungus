using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
	/**
	 *  Command classes have their own namespace to prevent them popping up in code completion
	 */
	namespace Command
	{
		/** 
		 * Plays a music clip
		 */
		public class PlayMusic : CommandQueue.Command
		{
			AudioClip audioClip;
			
			public PlayMusic(AudioClip _audioClip)
			{
				if (_audioClip == null)
				{
					Debug.LogError("Audio clip must not be null.");
					return;
				}
				
				audioClip = _audioClip;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game game = Game.GetInstance();
				
				game.audio.clip = audioClip;
				game.audio.Play();
				
				if (onComplete != null)
				{
					onComplete();
				}
			}
		}
		
		/** 
		 * Stops a music clip
		 */
		public class StopMusic : CommandQueue.Command
		{
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game game = Game.GetInstance();
				game.audio.Stop();
				
				if (onComplete != null)
				{
					onComplete();
				}
			}
		}
		
		/** 
		 * Fades music volume to required level over a period of time
		 */
		public class SetMusicVolume : CommandQueue.Command
		{
			float musicVolume;
			float duration;
			
			public SetMusicVolume(float _musicVolume, float _duration)
			{
				musicVolume = _musicVolume;
				duration = _duration;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game game = Game.GetInstance();
				iTween.AudioTo(game.gameObject, musicVolume, 1f, duration);
				
				if (onComplete != null)
				{
					onComplete();
				}
			}
		}
		
		/** 
		 * Plays a sound effect once
		 */
		public class PlaySound : CommandQueue.Command
		{
			AudioClip audioClip;
			float volume;
			
			public PlaySound(AudioClip _audioClip, float _volume)
			{
				audioClip = _audioClip;
				volume = _volume;
			}
			
			public override void Execute(CommandQueue commandQueue, Action onComplete)
			{
				Game game = Game.GetInstance();
				game.audio.PlayOneShot(audioClip, volume);
				
				if (onComplete != null)
				{
					onComplete();
				}
			}
		}
	}
}
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	/*
	 * Manages audio effects for Dialogs
	 */
	public class DialogAudio : MonoBehaviour, IWriterListener
	{
		// If none is specifed then we use any AudioSource on the gameobject, and if that doesn't exist we create one.
		[Tooltip("AudioSource to use for playing sound effects.")]
		public AudioSource audioSource;

		public enum AudioMode
		{
			Beeps,			// Use short beep sound effects
			SoundEffect,	// Use long looping sound effect
		}

		[Tooltip("Type of sound effect to play when writing text")]
		public AudioMode audioMode = AudioMode.Beeps;

		[Tooltip("List of beeps to randomly select when playing beep sound effects")]
		public List<AudioClip> beeps = new List<AudioClip>();

		[Tooltip("Long playing sound effect to play when writing text")]
		public AudioClip soundEffect;

		[Tooltip("Loop the sound effect")]
		public bool loop = true;

		[Tooltip("Volume level of writing sound effects")]
		[Range(0,1)]
		public float volume = 1f;

		protected float targetVolume = 0f;

		protected virtual void Start()
		{
			if (audioSource == null)
			{
				audioSource = GetComponent<AudioSource>();
				if (audioSource == null)
				{
					audioSource = gameObject.AddComponent<AudioSource>();
				}
			}

			audioSource.volume = 0f;
		}

		public virtual void Play(AudioClip audioClip)
		{
			if (audioSource == null ||
			    (soundEffect == null && audioClip == null))
			{
				return;
			}

			if (audioClip != null)
			{
				audioSource.clip = audioClip;
			}
			else
			{
				audioSource.clip = soundEffect;
			}

			audioSource.loop = loop;

			audioSource.volume = 0f;
			targetVolume = 1f;

			audioSource.Play();
		}

		public virtual void Pause()
		{
			if (audioSource == null)
			{
				return;
			}

			// There's an audible click if you call audioSource.Pause() so instead just drop the volume to 0.
			targetVolume = 0f;
		}

		public virtual void Stop()
		{
			if (audioSource == null)
			{
				return;
			}

			// There's an audible click if you call audioSource.Stop() so instead we just switch off
			// looping and let the audio stop automatically at the end of the clip
			targetVolume = 0f;
			audioSource.loop = false;
		}

		public virtual void Resume()
		{
			if (audioSource == null)
			{
				return;
			}

			targetVolume = 1f;
		}

		protected virtual void Update()
		{
			audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, Time.deltaTime * 5f);
		}

		//
		// IWriterListener implementation
		//

		public virtual void OnStart(AudioClip audioClip)
		{
			Play(audioClip);
		}
		
		public virtual void OnPause()
		{
			Pause();
		}
		
		public virtual void OnResume()
		{
			Resume();
		}
		
		public virtual void OnEnd()
		{
			Stop ();
		}
		
		public virtual void OnCharacter()
		{
		}
	}

}
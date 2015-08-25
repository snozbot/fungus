using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	/*
	 * Manages audio effects for Dialogs
	 */
	public class WriterAudio : MonoBehaviour, IWriterListener
	{
		[Tooltip("Volume level of writing sound effects")]
		[Range(0,1)]
		public float volume = 1f;

		[Tooltip("Loop the audio when in Sound Effect mode. Has no effect in Beeps mode.")]
		public bool loop = true;

		// If none is specifed then we use any AudioSource on the gameobject, and if that doesn't exist we create one.
		[Tooltip("AudioSource to use for playing sound effects. If none is selected then one will be created.")]
		public AudioSource audioSource;

		public enum AudioMode
		{
			Beeps,			// Use short beep sound effects
			SoundEffect,	// Use long looping sound effect
		}

		[Tooltip("Type of sound effect to play when writing text")]
		public AudioMode audioMode = AudioMode.Beeps;

		[Tooltip("List of beeps to randomly select when playing beep sound effects. Will play maximum of one beep per character, with only one beep playing at a time.")]
		public List<AudioClip> beepSounds = new List<AudioClip>();

		[Tooltip("Long playing sound effect to play when writing text")]
		public AudioClip soundEffect;

		protected float targetVolume = 0f;

		// When true, a beep will be played on every written character glyph
		protected bool playBeeps;

		public virtual void SetAudioMode(AudioMode mode)
		{
			audioMode = mode;
		}

		protected virtual void Awake()
		{
			// Need to do this in Awake rather than Start due to init order issues
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

			audioSource.volume = 0f;
			targetVolume = 1f;

			if (audioClip != null)
			{
				// Voice over clip provided
				audioSource.clip = audioClip;
				audioSource.loop = loop;
				audioSource.Play();
			}
			else if (audioMode == AudioMode.SoundEffect &&
			         soundEffect != null)
			{
				// Use sound effects defined in WriterAudio
				audioSource.clip = soundEffect;
				audioSource.loop = loop;
				audioSource.Play();
			}
			else if (audioMode == AudioMode.Beeps)
			{
				// Use beeps defined in WriterAudio
				audioSource.clip = null;
				audioSource.loop = false;
				playBeeps = true;
			}
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
			playBeeps = false;
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
			Stop();
		}
		
		public virtual void OnGlyph()
		{
			if (playBeeps && beepSounds.Count > 0)
			{
				if (!audioSource.isPlaying)
				{
					audioSource.clip = beepSounds[Random.Range(0, beepSounds.Count - 1)];
					audioSource.loop = false;
					audioSource.Play();
				}
			}
		}
	}

}
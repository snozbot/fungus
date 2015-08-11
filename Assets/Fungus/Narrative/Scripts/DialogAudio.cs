using UnityEngine;
using System.Collections;

namespace Fungus
{

	/*
	 * Helper class to manage play, pause & stop operations on voiceover and writing sound effects
	 */
	public class DialogAudio
	{
		public AudioSource audioSource;
		public AudioClip audioClip;
		public float volume;
		public bool loop;

		public virtual void Play()
		{
			if (audioSource == null ||
			    audioClip == null)
			{
				return;
			}

			audioSource.clip = audioClip;
			audioSource.loop = loop;

			// Fade in the audio at start
			LeanTween.value(audioSource.gameObject, 0f, volume, 0.1f).setOnUpdate( (value) => {
				audioSource.volume = value;
			});

			audioSource.Play();
		}

		public virtual void Pause()
		{
			if (audioSource == null)
			{
				return;
			}

			// Fade out the audio
			// There's an audible click if you call audioSource.Pause() so instead just
			// drop the volume to 0.
			LeanTween.value(audioSource.gameObject, volume, 0f, 0.1f).setOnUpdate( (value) => {
				audioSource.volume = value;
			});
		}

		public virtual void Stop()
		{
			if (audioSource == null)
			{
				return;
			}

			// Fade out the audio
			LeanTween.value(audioSource.gameObject, audioSource.volume, 0f, 0.1f).setOnUpdate( (value) => {
				audioSource.volume = value;
			}).setOnComplete( () => {
				// There's an audible click if you call audioSource.Stop() so instead we just switch off
				// looping and let the audio stop automatically at the end of the clip
				audioSource.loop = false;
			});
		}

		public virtual void Resume()
		{
			if (audioSource == null)
			{
				return;
			}

			audioSource.volume = volume;
			if (!audioSource.isPlaying)
			{
				audioSource.loop = loop;
				audioSource.Play();
			}
		}
	}

}
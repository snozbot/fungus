// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Music manager which provides basic music and sound effect functionality.
    /// Music playback persists across scene loads.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        protected AudioSource audioSource;

        protected virtual void Awake()
        {
            audioSource = GetComponent<AudioSource>();            
        }

        protected virtual void Start()
        {
            audioSource.playOnAwake = false;
            audioSource.loop = true;
        }

        #region Public members

        /// <summary>
        /// Plays game music using an audio clip.
        /// One music clip may be played at a time.
        /// </summary>
        public void PlayMusic(AudioClip musicClip, bool loop, float fadeDuration, float atTime)
        {
            if (audioSource == null || audioSource.clip == musicClip)
            {
                return;
            }

            if (Mathf.Approximately(fadeDuration, 0f))
            {
                audioSource.clip = musicClip;
                audioSource.loop = loop;
                audioSource.time = atTime;  // May be inaccurate if the audio source is compressed http://docs.unity3d.com/ScriptReference/AudioSource-time.html BK
                audioSource.Play();
            }
            else
            {
                float startVolume = audioSource.volume;

                LeanTween.value(gameObject, startVolume, 0f, fadeDuration)
                    .setOnUpdate( (v) => {
                        // Fade out current music
                        audioSource.volume = v;
                    }).setOnComplete( () => {
                        // Play new music
                        audioSource.volume = startVolume;
                        audioSource.clip = musicClip;
                        audioSource.loop = loop;
                        audioSource.time = atTime;  // May be inaccurate if the audio source is compressed http://docs.unity3d.com/ScriptReference/AudioSource-time.html BK
                        audioSource.Play();
                    });
            }
        }

        /// <summary>
        /// Plays a sound effect once, at the specified volume.
        /// </summary>
        /// <param name="soundClip">The sound effect clip to play.</param>
        /// <param name="volume">The volume level of the sound effect.</param>
        public virtual void PlaySound(AudioClip soundClip, float volume)
        {
            audioSource.PlayOneShot(soundClip, volume);
        }

        /// <summary>
        /// Shifts the game music pitch to required value over a period of time.
        /// </summary>
        /// <param name="pitch">The new music pitch value.</param>
        /// <param name="duration">The length of time in seconds needed to complete the pitch change.</param>
        /// <param name="onComplete">A delegate method to call when the pitch shift has completed.</param>
        public virtual void SetAudioPitch(float pitch, float duration, System.Action onComplete)
        {
            if (Mathf.Approximately(duration, 0f))
            {
                audioSource.pitch = pitch;
                if (onComplete != null)
                {
                    onComplete();
                }
                return;
            }

            LeanTween.value(gameObject, 
                audioSource.pitch, 
                pitch, 
                duration).setOnUpdate( (p) => {
                    audioSource.pitch = p;
                }).setOnComplete( () => {
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                });
        }

        /// <summary>
        /// Fades the game music volume to required level over a period of time.
        /// </summary>
        /// <param name="volume">The new music volume value [0..1]</param>
        /// <param name="duration">The length of time in seconds needed to complete the volume change.</param>
        /// <param name="onComplete">Delegate function to call when fade completes.</param>
        public virtual void SetAudioVolume(float volume, float duration, System.Action onComplete)
        {
            if (Mathf.Approximately(duration, 0f))
            {
				if (onComplete != null)
				{
					onComplete();
				}				
                audioSource.volume = volume;
                return;
            }

            LeanTween.value(gameObject, 
                audioSource.volume, 
                volume, 
                duration).setOnUpdate( (v) => {
                    audioSource.volume = v;
                }).setOnComplete( () => {
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                });
        }

        /// <summary>
        /// Stops playing game music.
        /// </summary>
        public virtual void StopMusic()
        {
            audioSource.Stop();
            audioSource.clip = null;
        }

        #endregion
    }
}
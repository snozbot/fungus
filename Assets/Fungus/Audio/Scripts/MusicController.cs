// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Singleton music manager component.
    /// Provides basic music and sound effect functionality.
    /// Music playback persists across scene loads.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class MusicController : MonoBehaviour 
    {
        static MusicController instance;

        // Returns the MusicController singleton instance.
        // Will create a MusicController game object if none currently exists.
        static public MusicController GetInstance()
        {
            if (instance == null)
            {
                GameObject go = new GameObject("MusicController");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<MusicController>();
            }

            return instance;
        }

        protected virtual void Start()
        {
            GetComponent<AudioSource>().playOnAwake = false;
            GetComponent<AudioSource>().loop = true;
        }

        /// <summary>
        /// // Plays game music using an audio clip.
        /// One music clip may be played at a time.
        /// </summary>
        public void PlayMusic(AudioClip musicClip, bool loop, float fadeDuration, float atTime)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource == null || audioSource.clip == musicClip)
            {
                return;
            }

            if (fadeDuration == 0f)
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
        /// Stops playing game music.
        /// </summary>
        public virtual void StopMusic()
        {
            GetComponent<AudioSource>().Stop();
        }

        /// <summary>
        /// Fades the game music volume to required level over a period of time.
        /// </summary>
        /// <param name="volume">The new music volume value [0..1]</param>
        /// <param name="duration">The length of time in seconds needed to complete the volume change.</param>
        /// <param name="onComplete">Delegate function to call when fade completes.</param>
        public virtual void SetAudioVolume(float volume, float duration, System.Action onComplete)
        {
            AudioSource audio = GetComponent<AudioSource>();

            if (Mathf.Approximately(duration, 0f))
            {
                audio.volume = volume;
                return;
            }

            LeanTween.value(gameObject, 
                audio.volume, 
                volume, 
                duration).setOnUpdate( (v) => {
                    audio.volume = v;
                }).setOnComplete( () => {
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                });
        }

        /// <summary>
        /// Shifts the game music pitch to required value over a period of time.
        /// </summary>
        /// <param name="pitch">The new music pitch value.</param>
        /// <param name="duration">The length of time in seconds needed to complete the pitch change.</param>
        /// <param name="onComplete">A delegate method to call when the pitch shift has completed.</param>
        public virtual void SetAudioPitch(float pitch, float duration, System.Action onComplete)
        {
            AudioSource audio = GetComponent<AudioSource>();

            if (duration == 0f)
            {
                audio.pitch = pitch;
                if (onComplete != null)
                {
                    onComplete();
                }
                return;
            }

            LeanTween.value(gameObject, 
                audio.pitch, 
                pitch, 
                duration).setOnUpdate( (p) => {
                    audio.pitch = p;
                }).setOnComplete( () => {
                    if (onComplete != null)
                    {
                        onComplete();
                    }
                });
        }

        /// <summary>
        /// Plays a sound effect once, at the specified volume.
        /// </summary>
        /// <param name="soundClip">The sound effect clip to play.</param>
        /// <param name="volume">The volume level of the sound effect.</param>
        public virtual void PlaySound(AudioClip soundClip, float volume)
        {
            GetComponent<AudioSource>().PlayOneShot(soundClip, volume);
        }

        public virtual void PlaySoundAtTime(AudioClip soundClip, float volume, float atTime)
        {
            GetComponent<AudioSource>().time = atTime;                      // This may not work BK
            GetComponent<AudioSource>().PlayOneShot(soundClip, volume);
        }
    }
}
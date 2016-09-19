// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    [RequireComponent(typeof(AudioSource))]
    public class MusicController : MonoBehaviour, IMusicController
    {
        static IMusicController instance;

        /// <summary>
        /// Returns the MusicController singleton instance.
        /// Will create a MusicController game object if none currently exists.
        /// </summary>
        static public IMusicController GetInstance()
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
            
        #region IMusicController implementation

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

        public virtual void PlaySound(AudioClip soundClip, float volume)
        {
            GetComponent<AudioSource>().PlayOneShot(soundClip, volume);
        }

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

        public virtual void StopMusic()
        {
            GetComponent<AudioSource>().Stop();
        }

        #endregion
    }
}
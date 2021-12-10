// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Type of audio effect to play.
    /// </summary>
    public enum AudioMode
    {
        /// <summary> Use short beep sound effects. </summary>
        Beeps,
        /// <summary> Use long looping sound effect. </summary>
        SoundEffect,
    }

    /// <summary>
    /// Manages audio effects for Dialogs.
    /// </summary>
    public class WriterAudio : MonoBehaviour, IWriterListener
    {
        [Tooltip("Volume level of writing sound effects")]
        [Range(0,1)]
        [SerializeField] protected float volume = 1f;

        [Tooltip("Loop the audio when in Sound Effect mode. Has no effect in Beeps mode.")]
        [SerializeField] protected bool loop = true;

        // If none is specifed then we use any AudioSource on the gameobject, and if that doesn't exist we create one.
        [Tooltip("AudioSource to use for playing sound effects. If none is selected then one will be created.")]
        [SerializeField] protected AudioSource targetAudioSource;

        [Tooltip("Type of sound effect to play when writing text")]
        [SerializeField] protected AudioMode audioMode = AudioMode.Beeps;

        [Tooltip("List of beeps to randomly select when playing beep sound effects. Will play maximum of one beep per character, with only one beep playing at a time.")]
        [SerializeField] protected List<AudioClip> beepSounds = new List<AudioClip>();

        [Tooltip("Long playing sound effect to play when writing text")]
        [SerializeField] protected AudioClip soundEffect;

        [Tooltip("Sound effect to play on user input (e.g. a click)")]
        [SerializeField] protected AudioClip inputSound;

        protected float targetVolume = 0f;

        // When true, a beep will be played on every written character glyph
        protected bool playBeeps;

        // True when a voiceover clip is playing
        protected bool playingVoiceover = false;

        protected AudioSource lastUsedAudioSource;
        protected SayDialog attachedSayDialog;

        // Time when current beep will have finished playing
        protected float nextBeepTime;

        [Tooltip("If true, legacy voiceover logic used and any audio clips will be played through the targetAudioSource," +
            " same one that sfx and beeps are played through.")]
        [SerializeField] protected bool useLegacyAudioLogic = false;

        protected virtual AudioSource VoiceOverAudioSource
        {
            get
            {
                if (useLegacyAudioLogic)
                    return targetAudioSource;

                AudioSource voiceOverSource = FungusManager.Instance.MusicManager.DefaultVoiceAudioSource;

                if (attachedSayDialog != null &&
                    attachedSayDialog.SpeakingCharacter != null &&
                    attachedSayDialog.SpeakingCharacter.VoiceAudioSource != null)
                {
                    voiceOverSource = attachedSayDialog.SpeakingCharacter.VoiceAudioSource;
                }

                return voiceOverSource;
            }
        }
        protected virtual AudioSource EffectAudioSource
        {
            get
            {
                if (useLegacyAudioLogic)
                    return targetAudioSource;

                AudioSource voiceOverSource = FungusManager.Instance.MusicManager.WriterSoundEffectAudioSource;

                if (attachedSayDialog != null &&
                    attachedSayDialog.SpeakingCharacter != null &&
                    attachedSayDialog.SpeakingCharacter.EffectAudioSource != null)
                {
                    voiceOverSource = attachedSayDialog.SpeakingCharacter.EffectAudioSource;
                }

                return voiceOverSource;
            }
        }

        public float GetSecondsRemaining()
        {
            if (playingVoiceover)
            {
                return targetAudioSource.isPlaying ? targetAudioSource.clip.length - targetAudioSource.time : 0f;
            }
            else
            {
                return 0F;
            }
        }

        protected virtual void SetAudioMode(AudioMode mode)
        {
            audioMode = mode;
        }

        protected virtual void Awake()
        {
            // Need to do this in Awake rather than Start due to init order issues
            if (useLegacyAudioLogic)
            {
                if (targetAudioSource == null)
                {
                    targetAudioSource = GetComponent<AudioSource>();
                    if (targetAudioSource == null)
                    {
                        targetAudioSource = gameObject.AddComponent<AudioSource>();
                        targetAudioSource.outputAudioMixerGroup = FungusManager.Instance.MainAudioMixer.SFXGroup;
                    }
                }

                targetAudioSource.volume = 0f;
            }

            attachedSayDialog = GetComponent<SayDialog>();
        }

        protected virtual void Play(AudioClip audioClip)
        {
            if (EffectAudioSource == null ||
                (audioMode == AudioMode.SoundEffect && soundEffect == null && audioClip == null) ||
                (audioMode == AudioMode.Beeps && beepSounds.Count == 0))
            {
                return;
            }

            lastUsedAudioSource = EffectAudioSource;

            playingVoiceover = false;
            lastUsedAudioSource.volume = 0f;
            targetVolume = volume;

            if (audioClip != null)
            {
                // Voice over clip provided
                lastUsedAudioSource.clip = audioClip;
                lastUsedAudioSource.loop = loop;
                lastUsedAudioSource.Play();
            }
            else if (audioMode == AudioMode.SoundEffect &&
                     soundEffect != null)
            {
                // Use sound effects defined in WriterAudio
                lastUsedAudioSource.clip = soundEffect;
                lastUsedAudioSource.loop = loop;
                lastUsedAudioSource.Play();
            }
            else if (audioMode == AudioMode.Beeps)
            {
                // Use beeps defined in WriterAudio
                lastUsedAudioSource.clip = null;
                lastUsedAudioSource.loop = false;
                playBeeps = true;
            }
        }

        protected virtual void Pause()
        {
            if (lastUsedAudioSource == null)
            {
                return;
            }

            // There's an audible click if you call audioSource.Pause() so instead just drop the volume to 0.
            targetVolume = 0f;
        }

        protected virtual void Stop()
        {
            if (lastUsedAudioSource == null)
            {
                return;
            }

            // There's an audible click if you call audioSource.Stop() so instead we just switch off
            // looping and let the audio stop automatically at the end of the clip
            targetVolume = 0f;
            lastUsedAudioSource.loop = false;
            playBeeps = false;
            playingVoiceover = false;

            //TODO force speaking character to stop
        }

        protected virtual void Resume()
        {
            if (lastUsedAudioSource == null)
            {
                return;
            }

            targetVolume = volume;
        }

        protected virtual void Update()
        {
            if(lastUsedAudioSource != null)
                lastUsedAudioSource.volume = Mathf.MoveTowards(lastUsedAudioSource.volume, targetVolume, Time.deltaTime * 5f);
        }

        #region IWriterListener implementation

        public virtual void OnInput()
        {
            if (inputSound != null)
            {
                // Assumes we're playing a 2D sound
                AudioSource.PlayClipAtPoint(inputSound, Vector3.zero);
            }
        }

        public virtual void OnStart(AudioClip audioClip)
        {
            if (playingVoiceover)
            {
                return;
            }
            Play(audioClip);
        }
        
        public virtual void OnPause()
        {
            if (playingVoiceover)
            {
                return;
            }
            Pause();
        }
        
        public virtual void OnResume()
        {
            if (playingVoiceover)
            {
                return;
            }
            Resume();
        }
        
        public virtual void OnEnd(bool stopAudio)
        {
            if (stopAudio)
            {
                Stop();
            }
        }

        public virtual void OnGlyph()
        {
            if (playingVoiceover)
            {
                return;
            }

            if (playBeeps && beepSounds.Count > 0)
            {
                lastUsedAudioSource = EffectAudioSource;

                if (!lastUsedAudioSource.isPlaying)
                {
                    if (nextBeepTime < Time.realtimeSinceStartup)
                    {
                        lastUsedAudioSource.clip = beepSounds[Random.Range(0, beepSounds.Count)];

                        if (lastUsedAudioSource.clip != null)
                        {
                            lastUsedAudioSource.loop = false;
                            targetVolume = volume;
                            lastUsedAudioSource.Play();

                            float extend = lastUsedAudioSource.clip.length;
                            nextBeepTime = Time.realtimeSinceStartup + extend;
                        }
                    }
                }
            }
        }

        public virtual void OnVoiceover(AudioClip voiceoverClip)
        {
            if (VoiceOverAudioSource == null)
            {
                return;
            }

            playingVoiceover = true;

            lastUsedAudioSource = VoiceOverAudioSource;

            lastUsedAudioSource.volume = volume;
            targetVolume = volume;
            lastUsedAudioSource.loop = false;
            lastUsedAudioSource.clip = voiceoverClip;
            lastUsedAudioSource.Play();
        }

        public void OnAllWordsWritten()
        {
        }

        #endregion
    }
}

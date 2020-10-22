// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Music manager which provides basic music and sound effect functionality.
    /// Music playback persists across scene loads.
    /// </summary>
    //[RequireComponent(typeof(AudioSource))]
    public class MusicManager : MonoBehaviour
    {
        public float TargetMusicVolume { get; protected set; }
        public float TargetMusicPitch { get; protected set; }
        public float TargetAmbVolume { get; protected set; }
        public float TargetAmbPitch { get; protected set; }
        public AudioClip TargetMusicClip { get; protected set; }
        public AudioClip TargetAmbClip { get; protected set; }
        public AudioSource AudioSourceMusic { get; protected set; }
        public AudioSource AudioSourceAmbiance { get; protected set; }
        public AudioSource AudioSourceSoundEffect { get; protected set; }
        public AudioSource DefaultVoiceAudioSource { get; protected set; }
        public AudioSource WriterSoundEffectAudioSource { get; protected set; }

        private const int RequiredAudioSources = 5;

        private void Reset()
        {
            int audioSourceCount = this.GetComponents<AudioSource>().Length;
            for (int i = 0; i < RequiredAudioSources - audioSourceCount; i++)
                gameObject.AddComponent<AudioSource>();
        }

        public virtual void Init()
        {
            Reset();
            AudioSource[] audioSources = GetComponents<AudioSource>();
            AudioSourceMusic = audioSources[0];
            AudioSourceAmbiance = audioSources[1];
            AudioSourceSoundEffect = audioSources[2];
            DefaultVoiceAudioSource = audioSources[3];
            WriterSoundEffectAudioSource = audioSources[4];

            AudioSourceMusic.outputAudioMixerGroup = FungusManager.Instance.MainAudioMixer.MusicGroup;
            AudioSourceSoundEffect.outputAudioMixerGroup = FungusManager.Instance.MainAudioMixer.SFXGroup;
            AudioSourceAmbiance.outputAudioMixerGroup = AudioSourceSoundEffect.outputAudioMixerGroup;
            DefaultVoiceAudioSource.outputAudioMixerGroup = FungusManager.Instance.MainAudioMixer.VoiceGroup;
            WriterSoundEffectAudioSource.outputAudioMixerGroup = AudioSourceSoundEffect.outputAudioMixerGroup;
        }

        protected virtual void Start()
        {
            AudioSourceMusic.playOnAwake = false;
            AudioSourceMusic.loop = true;

            TargetMusicVolume = AudioSourceMusic.volume;
            TargetAmbVolume = AudioSourceAmbiance.volume;
            TargetMusicPitch = AudioSourceMusic.pitch;
            TargetAmbPitch = AudioSourceAmbiance.pitch;
        }

        /// <summary>
        /// Plays game music using an audio clip.
        /// One music clip may be played at a time.
        /// </summary>
        public void PlayMusic(AudioClip musicClip, bool loop, float fadeDuration, float atTime)
        {
            TargetMusicClip = musicClip;

            if (AudioSourceMusic == null || AudioSourceMusic.clip == musicClip)
            {
                return;
            }

            if (Mathf.Approximately(fadeDuration, 0f))
            {
                AudioSourceMusic.clip = musicClip;
                AudioSourceMusic.loop = loop;
                AudioSourceMusic.time = atTime;  // May be inaccurate if the audio source is compressed http://docs.unity3d.com/ScriptReference/AudioSource-time.html BK
                AudioSourceMusic.Play();
            }
            else
            {
                float startVolume = AudioSourceMusic.volume;

                LeanTween.value(gameObject, startVolume, 0f, fadeDuration)
                    .setOnUpdate((v) =>
                    {
                        // Fade out current music
                        AudioSourceMusic.volume = v;
                    }).setOnComplete(() =>
                    {
                        // Play new music
                        AudioSourceMusic.volume = startVolume;
                        AudioSourceMusic.clip = musicClip;
                        AudioSourceMusic.loop = loop;
                        AudioSourceMusic.time = atTime;  // May be inaccurate if the audio source is compressed http://docs.unity3d.com/ScriptReference/AudioSource-time.html BK
                        AudioSourceMusic.Play();
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
            AudioSourceSoundEffect.PlayOneShot(soundClip, volume);
        }

        /// <summary>
        /// Plays a sound effect with optional looping values, at the specified volume.
        /// </summary>
        /// <param name="soundClip">The sound effect clip to play.</param>
        /// <param name="loop">If the audioclip should loop or not.</param>
        /// <param name="volume">The volume level of the sound effect.</param>
        public virtual void PlayAmbianceSound(AudioClip soundClip, bool loop, float volume)
        {
            TargetAmbClip = soundClip;
            AudioSourceAmbiance.loop = loop;
            AudioSourceAmbiance.clip = soundClip;
            AudioSourceAmbiance.volume = volume;
            AudioSourceAmbiance.Play();
            TargetAmbVolume = volume;
        }

        /// <summary>
        /// Shifts the game music pitch to required value over a period of time.
        /// </summary>
        /// <param name="pitch">The new music pitch value.</param>
        /// <param name="duration">The length of time in seconds needed to complete the pitch change.</param>
        /// <param name="onComplete">A delegate method to call when the pitch shift has completed.</param>
        public virtual void SetAudioPitch(float pitch, float duration, System.Action onComplete)
        {
            TargetAmbPitch = pitch;
            TargetMusicPitch = pitch;

            if (Mathf.Approximately(duration, 0f))
            {
                AudioSourceMusic.pitch = pitch;
                AudioSourceAmbiance.pitch = pitch;
                if (onComplete != null)
                {
                    onComplete();
                }
                return;
            }

            LeanTween.value(gameObject,
                AudioSourceMusic.pitch,
                pitch,
                duration).setOnUpdate((p) =>
                {
                    AudioSourceMusic.pitch = p;
                    AudioSourceAmbiance.pitch = p;
                }).setOnComplete(() =>
                {
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
            TargetMusicVolume = volume;
            TargetAmbVolume = volume;

            if (Mathf.Approximately(duration, 0f))
            {
                if (onComplete != null)
                {
                    onComplete();
                }
                AudioSourceMusic.volume = volume;
                AudioSourceAmbiance.volume = volume;
                return;
            }

            LeanTween.value(gameObject,
                AudioSourceMusic.volume,
                volume,
                duration).setOnUpdate((v) =>
                {
                    AudioSourceMusic.volume = v;
                    AudioSourceAmbiance.volume = v;
                }).setOnComplete(() =>
                {
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
            AudioSourceMusic.Stop();
            AudioSourceMusic.clip = null;
        }

        /// <summary>
        /// Stops playing game ambiance.
        /// </summary>
        public virtual void StopAmbiance()
        {
            AudioSourceAmbiance.Stop();
            AudioSourceAmbiance.clip = null;
        }
    }
}

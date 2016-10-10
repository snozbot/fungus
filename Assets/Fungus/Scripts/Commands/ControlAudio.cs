// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// The type of audio control to perform.
    /// </summary>
    public enum ControlAudioType
    {
        /// <summary> Play the audiosource once. </summary>
        PlayOnce,
        /// <summary> Play the audiosource in a loop. </summary>
        PlayLoop,
        /// <summary> Pause a looping audiosource. </summary>
        PauseLoop,
        /// <summary> Stop a looping audiosource. </summary>
        StopLoop,
        /// <summary> Change the volume level of an audiosource. </summary>
        ChangeVolume
    }

    /// <summary>
    /// Plays, loops, or stops an audiosource. Any AudioSources with the same tag as the target Audio Source will automatically be stoped.
    /// </summary>
    [CommandInfo("Audio", 
                 "Control Audio",
                 "Plays, loops, or stops an audiosource. Any AudioSources with the same tag as the target Audio Source will automatically be stoped.")]
    [ExecuteInEditMode]
    public class ControlAudio : Command
    {
        [Tooltip("What to do to audio")]
        [SerializeField] protected ControlAudioType control;
        public virtual ControlAudioType Control { get { return control; } }

        [Tooltip("Audio clip to play")]
        [SerializeField] protected AudioSourceData _audioSource;

        [Range(0,1)]
        [Tooltip("Start audio at this volume")]
        [SerializeField] protected float startVolume = 1;

        [Range(0,1)]
        [Tooltip("End audio at this volume")]
        [SerializeField] protected float endVolume = 1;
        
        [Tooltip("Time to fade between current volume level and target volume level.")]
        [SerializeField] protected float fadeDuration; 

        [Tooltip("Wait until this command has finished before executing the next command.")]
        [SerializeField] protected bool waitUntilFinished = false;

        // If there's other music playing in the scene, assign it the same tag as the new music you want to play and
        // the old music will be automatically stopped.
        protected virtual void StopAudioWithSameTag()
        {
            // Don't stop audio if there's no tag assigned
            if (_audioSource.Value == null ||
                _audioSource.Value.tag == "Untagged")
            {
                return;
            }

            var audioSources = GameObject.FindObjectsOfType<AudioSource>();
            for (int i = 0; i < audioSources.Length; i++)
            {
                var a = audioSources[i];
                if (a != _audioSource.Value && a.tag == _audioSource.Value.tag)
                {
                    StopLoop(a);
                }
            }
        }

        protected virtual void PlayOnce() 
        {
            if (fadeDuration > 0)
            {
                // Fade volume in
                LeanTween.value(_audioSource.Value.gameObject, 
                    _audioSource.Value.volume, 
                    endVolume,
                    fadeDuration
                ).setOnUpdate(
                    (float updateVolume)=>{
                    _audioSource.Value.volume = updateVolume;
                });
            }

            _audioSource.Value.PlayOneShot(_audioSource.Value.clip);

            if (waitUntilFinished)
            {
                StartCoroutine(WaitAndContinue());
            }
        }

        protected virtual IEnumerator WaitAndContinue()
        {
            // Poll the audiosource until playing has finished
            // This allows for things like effects added to the audiosource.
            while (_audioSource.Value.isPlaying)
            {
                yield return null;
            }

            Continue();
        }

        protected virtual void PlayLoop()
        {
            if (fadeDuration > 0)
            {
                _audioSource.Value.volume = 0;
                _audioSource.Value.loop = true;
                _audioSource.Value.GetComponent<AudioSource>().Play();
                LeanTween.value(_audioSource.Value.gameObject,0,endVolume,fadeDuration
                ).setOnUpdate(
                    (float updateVolume)=>{
                    _audioSource.Value.volume = updateVolume;
                }
                ).setOnComplete(
                    ()=>{
                    if (waitUntilFinished)
                    {
                        Continue();
                    }
                }
                );
            }
            else
            {
                _audioSource.Value.volume = 1;
                _audioSource.Value.loop = true;
                _audioSource.Value.GetComponent<AudioSource>().Play();
            }
        }

        protected virtual void PauseLoop()
        {
            if (fadeDuration > 0)
            {
                LeanTween.value(_audioSource.Value.gameObject,_audioSource.Value.volume,0,fadeDuration
                ).setOnUpdate(
                    (float updateVolume)=>{
                    _audioSource.Value.volume = updateVolume;
                }
                ).setOnComplete(
                    ()=>{

                    _audioSource.Value.GetComponent<AudioSource>().Pause();
                    if (waitUntilFinished)
                    {
                        Continue();
                    }
                }
                );
            }
            else
            {
                _audioSource.Value.GetComponent<AudioSource>().Pause();
            }
        }

        protected virtual void StopLoop(AudioSource source)
        {
            if (fadeDuration > 0)
            {
                LeanTween.value(source.gameObject,_audioSource.Value.volume,0,fadeDuration
                ).setOnUpdate(
                    (float updateVolume)=>{
                    source.volume = updateVolume;
                }
                ).setOnComplete(
                    ()=>{

                    source.GetComponent<AudioSource>().Stop();
                    if (waitUntilFinished)
                    {
                        Continue();
                    }
                }
                );
            }
            else
            {
                source.GetComponent<AudioSource>().Stop();
            }
        }

        protected virtual void ChangeVolume()
        {
            LeanTween.value(_audioSource.Value.gameObject,_audioSource.Value.volume,endVolume,fadeDuration
            ).setOnUpdate(
                (float updateVolume)=>{
                _audioSource.Value.volume = updateVolume;
            }).setOnComplete(
                ()=>{
                if (waitUntilFinished)
                {
                    Continue();
                }
            });
        }

        protected virtual void AudioFinished()
        {
            if (waitUntilFinished)
            {
                Continue();
            }
        }

        #region Public members

        public override void OnEnter()
        {
            if (_audioSource.Value == null)
            {
                Continue();
                return;
            }

            if (control != ControlAudioType.ChangeVolume)
            {
                _audioSource.Value.volume = endVolume;
            }

            switch(control)
            {
                case ControlAudioType.PlayOnce:
                    StopAudioWithSameTag();
                    PlayOnce();
                    break;
                case ControlAudioType.PlayLoop:
                    StopAudioWithSameTag();
                    PlayLoop();
                    break;
                case ControlAudioType.PauseLoop:
                    PauseLoop();
                    break;
                case ControlAudioType.StopLoop:
                    StopLoop(_audioSource.Value);
                    break;
                case ControlAudioType.ChangeVolume:
                    ChangeVolume(); 
                    break;
            }
            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            if (_audioSource.Value == null)
            {
                return "Error: No sound clip selected";
            }
            string fadeType = "";
            if (fadeDuration > 0)
            {
                fadeType = " Fade out";
                if (control != ControlAudioType.StopLoop)
                {
                    fadeType = " Fade in volume to " + endVolume;
                }
                if (control == ControlAudioType.ChangeVolume)
                {
                    fadeType = " to " + endVolume;
                }
                fadeType += " over " + fadeDuration + " seconds.";
            }
            return control.ToString() + " \"" + _audioSource.Value.name + "\"" + fadeType;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("audioSource")] public AudioSource audioSourceOLD;

        protected virtual void OnEnable()
        {
            if (audioSourceOLD != null)
            {
                _audioSource.Value = audioSourceOLD;
                audioSourceOLD = null;
            }
        }

        #endregion
    }    
}
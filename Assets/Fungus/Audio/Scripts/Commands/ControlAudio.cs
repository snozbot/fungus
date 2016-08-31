// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Plays, loops, or stops an audiosource. Any AudioSources with the same tag as the target Audio Source will automatically be stoped.
    /// </summary>
    [CommandInfo("Audio", 
                 "Control Audio",
                 "Plays, loops, or stops an audiosource. Any AudioSources with the same tag as the target Audio Source will automatically be stoped.")]
    [ExecuteInEditMode]
    public class ControlAudio : Command
    {
        public enum ControlType
        {
            PlayOnce,
            PlayLoop,
            PauseLoop,
            StopLoop,
            ChangeVolume
        }

        [Tooltip("What to do to audio")]
        [SerializeField] protected ControlType control;
        public virtual ControlType Control { get { return control; } }

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
        
        public override void OnEnter()
        {
            if (_audioSource.Value == null)
            {
                Continue();
                return;
            }

            if (control != ControlType.ChangeVolume)
            {
                _audioSource.Value.volume = endVolume;
            }

            switch(control)
            {
                case ControlType.PlayOnce:
                    StopAudioWithSameTag();
                    PlayOnce();
                    break;
                case ControlType.PlayLoop:
                    StopAudioWithSameTag();
                    PlayLoop();
                    break;
                case ControlType.PauseLoop:
                    PauseLoop();
                    break;
                case ControlType.StopLoop:
                    StopLoop(_audioSource.Value);
                    break;
                case ControlType.ChangeVolume:
                    ChangeVolume(); 
                    break;
            }
            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        // If there's other music playing in the scene, assign it the same tag as the new music you want to play and
        // the old music will be automatically stopped.
        protected void StopAudioWithSameTag()
        {
            // Don't stop audio if there's no tag assigned
            if (_audioSource.Value == null ||
                _audioSource.Value.tag == "Untagged")
            {
                return;
            }

            AudioSource[] audioSources = GameObject.FindObjectsOfType<AudioSource>();
            foreach (AudioSource a in audioSources)
            {
                if ((a.GetComponent<AudioSource>() != _audioSource.Value) && (a.tag == _audioSource.Value.tag))
                {
                    StopLoop(a.GetComponent<AudioSource>());
                }
            }
        }

        protected void PlayOnce() 
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

        protected void PlayLoop()
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

        protected void PauseLoop()
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

        protected void StopLoop(AudioSource source)
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

        protected void ChangeVolume()
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

        void AudioFinished()
        {
            if (waitUntilFinished)
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
                if (control != ControlType.StopLoop)
                {
                    fadeType = " Fade in volume to " + endVolume;
                }
                if (control == ControlType.ChangeVolume)
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
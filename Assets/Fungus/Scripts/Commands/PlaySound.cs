// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Localization;

namespace Fungus
{
    /// <summary>
    /// Plays a once-off sound effect. Multiple sound effects can be played at the same time.
    /// </summary>
    [CommandInfo("Audio", 
                 "Play Sound",
                 "Plays a once-off sound effect. Multiple sound effects can be played at the same time.")]
    [AddComponentMenu("")]
    public class PlaySound : Command
    {
#if UNITY_LOCALIZATION
        [Tooltip("Sound effect clip to play. Ignored if LocalizedSoundClip is not empty.")]
#else
        [Tooltip("Sound effect clip to play")]
#endif
        [SerializeField] protected AudioClip soundClip;

#if UNITY_LOCALIZATION
        [SerializeField] protected LocalizedAsset<AudioClip> localizedSoundClip;
#endif
        
        [Range(0,1)]
        [Tooltip("Volume level of the sound effect")]
        [SerializeField] protected float volume = 1;

        [Tooltip("Wait until the sound has finished playing before continuing execution.")]
        [SerializeField] protected bool waitUntilFinished;

        protected virtual void DoWait()
        {
            Continue();
        }

        #region Public members

        public override void OnEnter()
        {
            var musicManager = FungusManager.Instance.MusicManager;

#if UNITY_LOCALIZATION
            if (soundClip == null && localizedSoundClip.IsEmpty)
            {
                Continue();
                return;
            }
            
            musicManager.PlaySound(localizedSoundClip.IsEmpty ? soundClip : localizedSoundClip.LoadAsset(), volume);
#else
            if (soundClip == null)
            {
                Continue();
                return;
            }

            musicManager.PlaySound(soundClip, volume);
#endif

            if (waitUntilFinished)
            {
                Invoke("DoWait", soundClip.length);
            }
            else
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            if (soundClip == null && localizedSoundClip.IsEmpty)
            {
                return "Error: No sound clip selected";
            }

            return soundClip.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}

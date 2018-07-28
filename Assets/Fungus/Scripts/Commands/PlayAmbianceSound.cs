// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays a once-off sound effect. Multiple sound effects can be played at the same time.
    /// </summary>
    [CommandInfo("Audio",
                 "Play Ambiance Sound",
                 "Plays a background sound to be overlayed on top of the music. Only one Ambiance can be played at a time.")]
    [AddComponentMenu("")]
    public class PlayAmbianceSound : Command
    {
        [Tooltip("Sound effect clip to play")]
        [SerializeField]
        protected AudioClip soundClip;

        [Range(0, 1)]
        [Tooltip("Volume level of the sound effect")]
        [SerializeField]
        protected float volume = 1;
        
        [Tooltip("Sound effect clip to play")]
        [SerializeField]
        protected bool loop;

        protected virtual void DoWait()
        {
            Continue();
        }

        #region Public members

        public override void OnEnter()
        {
            if (soundClip == null)
            {
                Continue();
                return;
            }

            var musicManager = FungusManager.Instance.MusicManager;

            musicManager.PlayAmbianceSound(soundClip, loop, volume);

            Continue();
        }

        public override string GetSummary()
        {
            if (soundClip == null)
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

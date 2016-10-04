// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets the global pitch level for audio played with Play Music and Play Sound commands.
    /// </summary>
    [CommandInfo("Audio",
                 "Set Audio Pitch",
                 "Sets the global pitch level for audio played with Play Music and Play Sound commands.")]
    [AddComponentMenu("")]
    public class SetAudioPitch : Command
    {
        [Range(0,1)]
        [Tooltip("Global pitch level for audio played using the Play Music and Play Sound commands")]
        [SerializeField] protected float pitch = 1;

        [Range(0,30)]
        [Tooltip("Time to fade between current pitch level and target pitch level.")]
        [SerializeField] protected float fadeDuration; 

        [Tooltip("Wait until the pitch change has finished before executing next command")]
        [SerializeField] protected bool waitUntilFinished = true;

        #region Public members

        public override void OnEnter()
        {
            System.Action onComplete = () => {
                if (waitUntilFinished)
                {
                    Continue();
                }
            };

            var musicManager = FungusManager.Instance.MusicManager;

            musicManager.SetAudioPitch(pitch, fadeDuration, onComplete);

            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            return "Set to " + pitch + " over " + fadeDuration + " seconds.";
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}
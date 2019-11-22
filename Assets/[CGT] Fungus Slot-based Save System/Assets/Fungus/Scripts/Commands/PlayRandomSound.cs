// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays a once-off sound effect. Multiple sound effects can be played at the same time.
    /// </summary>
    [CommandInfo("Audio",
                 "Play Random Sound",
                 "Plays a once-off sound effect from a list of available sound effects. Multiple sound effects can be played at the same time.")]
    [AddComponentMenu("")]
    public class PlayRandomSound : Command
    {
        [Tooltip("Sound effect clip to play")]
        [SerializeField]
        protected AudioClip[] soundClip;

        [Range(0, 1)]
        [Tooltip("Volume level of the sound effect")]
        [SerializeField]
        protected float volume = 1;

        [Tooltip("Wait until the sound has finished playing before continuing execution.")]
        [SerializeField]
        protected bool waitUntilFinished;

        protected virtual void DoWait()
        {
            Continue();
        }

        #region Public members

        public override void OnEnter()
        {
            int rand = Random.Range(0, soundClip.Length);
            if (soundClip == null)
            {
                Continue();
                return;
            }

            var musicManager = FungusManager.Instance.MusicManager;

            musicManager.PlaySound(soundClip[rand], volume);

            if (waitUntilFinished)
            {
                Invoke("DoWait", soundClip[rand].length);
            }
            else
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            if (soundClip == null)
            {
                return "Error: No sound clip selected";
            }
            
            string sounds = "[";
            foreach (AudioClip ac in soundClip) {
				if(ac!=null)
					sounds+=ac.name+" ,";
            }
            sounds = sounds.TrimEnd(' ', ',');
            sounds += "]";
            return "Random sounds "+sounds;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}

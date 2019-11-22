// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays looping game music. If any game music is already playing, it is stopped. Game music will continue playing across scene loads.
    /// </summary>
    [CommandInfo("Audio",
                 "Play Music",
                 "Plays looping game music. If any game music is already playing, it is stopped. Game music will continue playing across scene loads.")]
    [AddComponentMenu("")]
    public class PlayMusic : Command
    {
        [Tooltip("Music sound clip to play")]
        [SerializeField] protected AudioClip musicClip;

        [Tooltip("Time to begin playing in seconds. If the audio file is compressed, the time index may be inaccurate.")]
        [SerializeField] protected float atTime;

        [Tooltip("The music will start playing again at end.")]
        [SerializeField] protected bool loop = true;
    
        [Tooltip("Length of time to fade out previous playing music.")]
        [SerializeField] protected float fadeDuration = 1f;

        #region Public members

        public override void OnEnter()
        {
            var musicManager = FungusManager.Instance.MusicManager;

            float startTime = Mathf.Max(0, atTime);
            musicManager.PlayMusic(musicClip, loop, fadeDuration, startTime);
                
            Continue();
        }
                    
        public override string GetSummary()
        {
            if (musicClip == null)
            {
                return "Error: No music clip selected";
            }

            return musicClip.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}
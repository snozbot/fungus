// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Stops the currently playing game music.
    /// </summary>
    [CommandInfo("Audio", 
                 "Stop Music", 
                 "Stops the currently playing game music.")]
    [AddComponentMenu("")]
    public class StopMusic : Command
    {
        #region Public members

        public override void OnEnter()
        {
            var musicManager = FungusManager.Instance.MusicManager;

            musicManager.StopMusic();

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}
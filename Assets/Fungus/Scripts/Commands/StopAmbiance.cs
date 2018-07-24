// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Stops the currently playing game music.
    /// </summary>
    [CommandInfo("Audio", 
                 "Stop Ambiance", 
                 "Stops the currently playing game ambiance.")]
    [AddComponentMenu("")]
    public class StopAmbiance : Command
    {
        #region Public members

        public override void OnEnter()
        {
            var musicManager = FungusManager.Instance.MusicManager;

            musicManager.StopAmbiance();

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(242, 209, 176, 255);
        }

        #endregion
    }
}
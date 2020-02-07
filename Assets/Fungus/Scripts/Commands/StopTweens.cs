// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Stop all active iTweens in the current scene.
    /// </summary>
    [CommandInfo("iTween", 
                 "Stop Tweens", 
                 "Stop all active iTweens in the current scene.")]
    [AddComponentMenu("")]
    public class StopTweens : Command
    {
        #region Public members

        public override void OnEnter()
        {
            iTween.Stop();
            Continue();
        }

        #endregion
    }
}
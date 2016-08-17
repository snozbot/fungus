/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using System;
using System.Collections;

namespace Fungus
{
    [CommandInfo("Camera", 
                 "Fullscreen", 
                 "Sets the application to fullscreen, windowed or toggles the current state.")]
    [AddComponentMenu("")]
    public class Fullscreen : Command 
    {
        public enum FullscreenMode
        {
            Toggle,
            Fullscreen,
            Windowed
        }

        public FullscreenMode fullscreenMode;

        public override void OnEnter()
        {
            switch (fullscreenMode)
            {
            case FullscreenMode.Toggle:
                Screen.fullScreen = !Screen.fullScreen;
                break;
            case FullscreenMode.Fullscreen:
                Screen.fullScreen = true;
                break;
            case FullscreenMode.Windowed:
                Screen.fullScreen = false;
                break;
            }

            Continue();
        }

        public override string GetSummary()
        {
            return fullscreenMode.ToString();
        }

        public override Color GetButtonColor()
        {
            return new Color32(216, 228, 170, 255);
        }
    }

}
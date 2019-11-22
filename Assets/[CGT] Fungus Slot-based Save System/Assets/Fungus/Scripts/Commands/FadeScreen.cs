// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Draws a fullscreen texture over the scene to give a fade effect. Setting Target Alpha to 1 will obscure the screen, alpha 0 will reveal the screen.
    /// If no Fade Texture is provided then a default flat color texture is used.
    /// </summary>
    [CommandInfo("Camera", 
                 "Fade Screen", 
                 "Draws a fullscreen texture over the scene to give a fade effect. Setting Target Alpha to 1 will obscure the screen, alpha 0 will reveal the screen. " +
                 "If no Fade Texture is provided then a default flat color texture is used.")]
    [AddComponentMenu("")]
    public class FadeScreen : Command 
    {
        [Tooltip("Time for fade effect to complete")]
        [SerializeField] protected float duration = 1f;

        [Tooltip("Current target alpha transparency value. The fade gradually adjusts the alpha to approach this target value.")]
        [SerializeField] protected float targetAlpha = 1f;

        [Tooltip("Wait until the fade has finished before executing next command")]
        [SerializeField] protected bool waitUntilFinished = true;

        [Tooltip("Color to render fullscreen fade texture with when screen is obscured.")]
        [SerializeField] protected Color fadeColor = Color.black;

        [Tooltip("Optional texture to use when rendering the fullscreen fade effect.")]
        [SerializeField] protected Texture2D fadeTexture;

        #region Public members

        public override void OnEnter()
        {
            var cameraManager = FungusManager.Instance.CameraManager;
            
            if (fadeTexture)
            {
                cameraManager.ScreenFadeTexture = fadeTexture;
            }
            else
            {
                cameraManager.ScreenFadeTexture = CameraManager.CreateColorTexture(fadeColor, 32, 32);
            }
            
            cameraManager.Fade(targetAlpha, duration, delegate { 
                if (waitUntilFinished)
                {
                    Continue();
                }
            });
            
            if (!waitUntilFinished)
            {
                Continue();
            }
        }
        
        public override string GetSummary()
        {
            return "Fade to " + targetAlpha + " over " + duration + " seconds";
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(216, 228, 170, 255);
        }

        #endregion
    }    
}
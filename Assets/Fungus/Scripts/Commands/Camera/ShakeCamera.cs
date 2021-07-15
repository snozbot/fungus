// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Applies a camera shake effect to the main camera.
    /// </summary>
    [CommandInfo("Camera", 
                 "Shake Camera", 
                 "Applies a camera shake effect to the main camera.")]
    [AddComponentMenu("")]
    [System.Obsolete("Deprecated, consider using the LeanTween based Tween command instead.")]
    public class ShakeCamera : Command 
    {
        [Tooltip("Time for camera shake effect to complete")]
        [SerializeField] protected float duration = 0.5f;
        
        [Tooltip("Magnitude of shake effect in x & y axes")]
        [SerializeField] protected Vector2 amount = new Vector2(1, 1);
        
        [Tooltip("Wait until the shake effect has finished before executing next command")]
        [SerializeField] protected bool waitUntilFinished;

        protected virtual void OniTweenComplete(object param)
        {
            Command command = param as Command;
            if (command != null && command.Equals(this))
            {
                if (waitUntilFinished)
                {
                    Continue();
                }
            }
        }

        #region Public members

        public override void OnEnter()
        {
            Vector3 v = new Vector3();
            v = amount;

            Hashtable tweenParams = new Hashtable();
            tweenParams.Add("amount", v);
            tweenParams.Add("time", duration);
            tweenParams.Add("oncomplete", "OniTweenComplete");
            tweenParams.Add("oncompletetarget", gameObject);
            tweenParams.Add("oncompleteparams", this);
            iTween.ShakePosition(Camera.main.gameObject, tweenParams);
            
            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            return "For " + duration + " seconds.";
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(216, 228, 170, 255);
        }

        #endregion
    }    
}
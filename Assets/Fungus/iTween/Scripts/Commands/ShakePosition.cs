/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
    [CommandInfo("iTween", 
                 "Shake Position", 
                 "Randomly shakes a GameObject's position by a diminishing amount over time.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class ShakePosition : iTweenCommand
    {
        [Tooltip("A translation offset in space the GameObject will animate to")]
        public Vector3Data _amount;

        [Tooltip("Whether to animate in world space or relative to the parent. False by default.")]
        public bool isLocal;

        [Tooltip("Restricts rotation to the supplied axis only")]
        public iTweenAxis axis;
        
        public override void DoTween()
        {
            Hashtable tweenParams = new Hashtable();
            tweenParams.Add("name", _tweenName.Value);
            tweenParams.Add("amount", _amount.Value);
            switch (axis)
            {
            case iTweenAxis.X:
                tweenParams.Add("axis", "x");
                break;
            case iTweenAxis.Y:
                tweenParams.Add("axis", "y");
                break;
            case iTweenAxis.Z:
                tweenParams.Add("axis", "z");
                break;
            }
            tweenParams.Add("time", _duration.Value);
            tweenParams.Add("easetype", easeType);
            tweenParams.Add("looptype", loopType);
            tweenParams.Add("isLocal", isLocal);
            tweenParams.Add("oncomplete", "OniTweenComplete");
            tweenParams.Add("oncompletetarget", gameObject);
            tweenParams.Add("oncompleteparams", this);
            iTween.ShakePosition(_targetObject.Value, tweenParams);
        }

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("amount")] public Vector3 amountOLD;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (amountOLD != default(Vector3))
            {
                _amount.Value = amountOLD;
                amountOLD = default(Vector3);
            }
        }

        #endregion
    }
    
}
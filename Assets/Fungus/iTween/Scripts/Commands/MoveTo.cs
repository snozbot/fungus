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
                 "Move To", 
                 "Moves a game object to a specified position over time. The position can be defined by a transform in another object (using To Transform) or by setting an absolute position (using To Position, if To Transform is set to None).")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class MoveTo : iTweenCommand
    {
        [Tooltip("Target transform that the GameObject will move to")]
        public TransformData _toTransform;

        [Tooltip("Target world position that the GameObject will move to, if no From Transform is set")]
        public Vector3Data _toPosition;

        [Tooltip("Whether to animate in world space or relative to the parent. False by default.")]
        public bool isLocal;

        public override void DoTween()
        {
            Hashtable tweenParams = new Hashtable();
            tweenParams.Add("name", _tweenName.Value);
            if (_toTransform.Value == null)
            {
                tweenParams.Add("position", _toPosition.Value);
            }
            else
            {
                tweenParams.Add("position", _toTransform.Value);
            }
            tweenParams.Add("time", _duration.Value);
            tweenParams.Add("easetype", easeType);
            tweenParams.Add("looptype", loopType);
            tweenParams.Add("isLocal", isLocal);
            tweenParams.Add("oncomplete", "OniTweenComplete");
            tweenParams.Add("oncompletetarget", gameObject);
            tweenParams.Add("oncompleteparams", this);
            iTween.MoveTo(_targetObject.Value, tweenParams);
        }

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("toTransform")] public Transform toTransformOLD;
        [HideInInspector] [FormerlySerializedAs("toPosition")] public Vector3 toPositionOLD;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (toTransformOLD != null)
            {
                _toTransform.Value = toTransformOLD;
                toTransformOLD = null;
            }

            if (toPositionOLD != default(Vector3))
            {
                _toPosition.Value = toPositionOLD;
                toPositionOLD = default(Vector3);
            }
        }

        #endregion
    }

}
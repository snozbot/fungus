// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Rotates a game object to the specified angles over time.
    /// </summary>
    [CommandInfo("iTween", 
                 "Rotate To", 
                 "Rotates a game object to the specified angles over time.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class RotateTo : iTweenCommand
    {
        [Tooltip("Target transform that the GameObject will rotate to")]
        [SerializeField] protected TransformData _toTransform;

        [Tooltip("Target rotation that the GameObject will rotate to, if no To Transform is set")]
        [SerializeField] protected Vector3Data _toRotation;

        [Tooltip("Whether to animate in world space or relative to the parent. False by default.")]
        [SerializeField] protected bool isLocal;

        #region Public members

        public override void DoTween()
        {
            Hashtable tweenParams = new Hashtable();
            tweenParams.Add("name", _tweenName.Value);
            if (_toTransform.Value == null)
            {
                tweenParams.Add("rotation", _toRotation.Value);
            }
            else
            {
                tweenParams.Add("rotation", _toTransform.Value);
            }
            tweenParams.Add("time", _duration.Value);
            tweenParams.Add("easetype", easeType);
            tweenParams.Add("looptype", loopType);
            tweenParams.Add("isLocal", isLocal);
            tweenParams.Add("oncomplete", "OniTweenComplete");
            tweenParams.Add("oncompletetarget", gameObject);
            tweenParams.Add("oncompleteparams", this);
            iTween.RotateTo(_targetObject.Value, tweenParams);
        }

        public override bool HasReference(Variable variable)
        {
            return _toTransform.transformRef == variable || _toRotation.vector3Ref == variable ||
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("toTransform")] public Transform toTransformOLD;
        [HideInInspector] [FormerlySerializedAs("toRotation")] public Vector3 toRotationOLD;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (toTransformOLD != null)
            {
                _toTransform.Value = toTransformOLD;
                toTransformOLD = null;
            }

            if (toRotationOLD != default(Vector3))
            {
                _toRotation.Value = toRotationOLD;
                toRotationOLD = default(Vector3);
            }
        }

        #endregion
    }
}
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Rotates a game object from the specified angles back to its starting orientation over time.
    /// </summary>
    [CommandInfo("iTween", 
                 "Rotate From", 
                 "Rotates a game object from the specified angles back to its starting orientation over time.")]
    [AddComponentMenu("")]
    [System.Obsolete("Deprecated, consider using the LeanTween based Tween command instead.")]
    [ExecuteInEditMode]
    public class RotateFrom : iTweenCommand
    {
        [Tooltip("Target transform that the GameObject will rotate from")]
        [SerializeField] protected TransformData _fromTransform;

        [Tooltip("Target rotation that the GameObject will rotate from, if no From Transform is set")]
        [SerializeField] protected Vector3Data _fromRotation;

        [Tooltip("Whether to animate in world space or relative to the parent. False by default.")]
        [SerializeField] protected bool isLocal;

        #region Public members

        public override void DoTween()
        {
            Hashtable tweenParams = new Hashtable();
            tweenParams.Add("name", _tweenName.Value);
            if (_fromTransform.Value == null)
            {
                tweenParams.Add("rotation", _fromRotation.Value);
            }
            else
            {
                tweenParams.Add("rotation", _fromTransform.Value);
            }
            tweenParams.Add("time", _duration.Value);
            tweenParams.Add("easetype", easeType);
            tweenParams.Add("looptype", loopType);
            tweenParams.Add("isLocal", isLocal);
            tweenParams.Add("oncomplete", "OniTweenComplete");
            tweenParams.Add("oncompletetarget", gameObject);
            tweenParams.Add("oncompleteparams", this);
            iTween.RotateFrom(_targetObject.Value, tweenParams);
        }

        public override bool HasReference(Variable variable)
        {
            return _fromTransform.transformRef == variable || _fromRotation.vector3Ref == variable ||
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("fromTransform")] public Transform fromTransformOLD;
        [HideInInspector] [FormerlySerializedAs("fromRotation")] public Vector3 fromRotationOLD;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (fromTransformOLD != null)
            {
                _fromTransform.Value = fromTransformOLD;
                fromTransformOLD = null;
            }

            if (fromRotationOLD != default(Vector3))
            {
                _fromRotation.Value = fromRotationOLD;
                fromRotationOLD = default(Vector3);
            }
        }

        #endregion
    }
}
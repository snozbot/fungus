// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Rotates a GameObject to look at a supplied Transform or Vector3 over time.
    /// </summary>
    [CommandInfo("iTween", 
                 "Look To", 
                 "Rotates a GameObject to look at a supplied Transform or Vector3 over time.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class LookTo : iTweenCommand
    {
        [Tooltip("Target transform that the GameObject will look at")]
        [SerializeField] protected TransformData _toTransform;

        [Tooltip("Target world position that the GameObject will look at, if no From Transform is set")]
        [SerializeField] protected Vector3Data _toPosition;

        [Tooltip("Restricts rotation to the supplied axis only")]
        [SerializeField] protected iTweenAxis axis;

        #region Public members

        public override void DoTween()
        {
            Hashtable tweenParams = new Hashtable();
            tweenParams.Add("name", _tweenName.Value);
            if (_toTransform.Value == null)
            {
                tweenParams.Add("looktarget", _toPosition.Value);
            }
            else
            {
                tweenParams.Add("looktarget", _toTransform.Value);
            }
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
            tweenParams.Add("oncomplete", "OniTweenComplete");
            tweenParams.Add("oncompletetarget", gameObject);
            tweenParams.Add("oncompleteparams", this);
            iTween.LookTo(_targetObject.Value, tweenParams);
        }

        public override bool HasReference(Variable variable)
        {
            return _toTransform.transformRef == variable || _toPosition.vector3Ref == variable ||
                base.HasReference(variable);
        }

        #endregion

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
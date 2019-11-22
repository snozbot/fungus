// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Changes a game object's scale to the specified value and back to its original scale over time.
    /// </summary>
    [CommandInfo("iTween", 
                 "Scale From", 
                 "Changes a game object's scale to the specified value and back to its original scale over time.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class ScaleFrom : iTweenCommand
    {
        [Tooltip("Target transform that the GameObject will scale from")]
        [SerializeField] protected TransformData _fromTransform;

        [Tooltip("Target scale that the GameObject will scale from, if no From Transform is set")]
        [SerializeField] protected Vector3Data _fromScale;

        #region Public members

        public override void DoTween()
        {
            Hashtable tweenParams = new Hashtable();
            tweenParams.Add("name", _tweenName.Value);
            if (_fromTransform.Value == null)
            {
                tweenParams.Add("scale", _fromScale.Value);
            }
            else
            {
                tweenParams.Add("scale", _fromTransform.Value);
            }
            tweenParams.Add("time", _duration.Value);
            tweenParams.Add("easetype", easeType);
            tweenParams.Add("looptype", loopType);
            tweenParams.Add("oncomplete", "OniTweenComplete");
            tweenParams.Add("oncompletetarget", gameObject);
            tweenParams.Add("oncompleteparams", this);
            iTween.ScaleFrom(_targetObject.Value, tweenParams);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("fromTransform")] public Transform fromTransformOLD;
        [HideInInspector] [FormerlySerializedAs("fromScale")] public Vector3 fromScaleOLD;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (fromTransformOLD != null)
            {
                _fromTransform.Value = fromTransformOLD;
                fromTransformOLD = null;
            }

            if (fromScaleOLD != default(Vector3))
            {
                _fromScale.Value = fromScaleOLD;
                fromScaleOLD = default(Vector3);
            }
        }

        #endregion
    }
}
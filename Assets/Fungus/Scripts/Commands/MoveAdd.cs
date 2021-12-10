// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// Moves a game object by a specified offset over time.
    /// </summary>
    [CommandInfo("iTween", 
                 "Move Add", 
                 "Moves a game object by a specified offset over time.")]
    [AddComponentMenu("")]
    [System.Obsolete("Deprecated, consider using the LeanTween based Tween command instead.")]
    [ExecuteInEditMode]
    public class MoveAdd : iTweenCommand
    {
        [Tooltip("A translation offset in space the GameObject will animate to")]
        [SerializeField] protected Vector3Data _offset;

        [Tooltip("Apply the transformation in either the world coordinate or local cordinate system")]
        [SerializeField] protected Space space = Space.Self;

        #region Public members

        public override void DoTween()
        {
            Hashtable tweenParams = new Hashtable();
            tweenParams.Add("name", _tweenName.Value);
            tweenParams.Add("amount", _offset.Value);
            tweenParams.Add("space", space);
            tweenParams.Add("time", _duration.Value);
            tweenParams.Add("easetype", easeType);
            tweenParams.Add("looptype", loopType);
            tweenParams.Add("oncomplete", "OniTweenComplete");
            tweenParams.Add("oncompletetarget", gameObject);
            tweenParams.Add("oncompleteparams", this);
            iTween.MoveAdd(_targetObject.Value, tweenParams);
        }

        public override bool HasReference(Variable variable)
        {
            return _offset.vector3Ref == variable ||
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("offset")] public Vector3 offsetOLD;

        protected override void OnEnable()
        {
            base.OnEnable();

            if (offsetOLD != default(Vector3))
            {
                _offset.Value = offsetOLD;
                offsetOLD = default(Vector3);
            }
        }

        #endregion
    }
}
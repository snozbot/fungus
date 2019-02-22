// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Axis to apply the tween on.
    /// </summary>
    public enum iTweenAxis
    {
        /// <summary> Don't specify an axis. </summary>
        None,
        /// <summary> Apply the tween on the X axis only. </summary>
        X,
        /// <summary> Apply the tween on the Y axis only. </summary>
        Y,
        /// <summary> Apply the tween on the Z axis only. </summary>
        Z
    }

    /// <summary>
    /// Abstract base class for iTween commands.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class iTweenCommand : Command
    {
        [Tooltip("Target game object to apply the Tween to")]
        [SerializeField] protected GameObjectData _targetObject;

        [Tooltip("An individual name useful for stopping iTweens by name")]
        [SerializeField] protected StringData _tweenName;

        [Tooltip("The time in seconds the animation will take to complete")]
        [SerializeField] protected FloatData _duration = new FloatData(1f);

        [Tooltip("The shape of the easing curve applied to the animation")]
        [SerializeField] protected iTween.EaseType easeType = iTween.EaseType.easeInOutQuad;

        [Tooltip("The type of loop to apply once the animation has completed")]
        [SerializeField] protected iTween.LoopType loopType = iTween.LoopType.none;

        [Tooltip("Stop any previously added iTweens on this object before adding this iTween")]
        [SerializeField] protected bool stopPreviousTweens = false;

        [Tooltip("Wait until the tween has finished before executing the next command")]
        [SerializeField] protected bool waitUntilFinished = true;

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
            if (_targetObject.Value == null)
            {
                Continue();
                return;
            }

            if (stopPreviousTweens)
            {
                // Force any existing iTweens on this target object to complete immediately
                var tweens = _targetObject.Value.GetComponents<iTween>();
                for (int i = 0; i < tweens.Length; i++)
                {
                    var tween = tweens[i];
                    tween.time = 0;
                    tween.SendMessage("Update");
                }
            }

            DoTween();

            if (!waitUntilFinished)
            {
                Continue();
            }
        }

        public virtual void DoTween()
        {}

        public override string GetSummary()
        {
            if (_targetObject.Value == null)
            {
                return "Error: No target object selected";
            }

            return _targetObject.Value.name + " over " + _duration.Value + " seconds";
        }

        public override Color GetButtonColor()
        {
            return new Color32(233, 163, 180, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return _targetObject.gameObjectRef == variable || _tweenName.stringRef == variable ||
                base.HasReference(variable);
        }

        #endregion

        #region Backwards compatibility

        [HideInInspector] [FormerlySerializedAs("target")] [FormerlySerializedAs("targetObject")] public GameObject targetObjectOLD;
        [HideInInspector] [FormerlySerializedAs("tweenName")] public string tweenNameOLD = "";
        [HideInInspector] [FormerlySerializedAs("duration")] public float durationOLD;

        protected virtual void OnEnable()
        {
            if (targetObjectOLD != null)
            {
                _targetObject.Value = targetObjectOLD;
                targetObjectOLD = null;
            }

            if (tweenNameOLD != "")
            {
                _tweenName.Value = tweenNameOLD;
                tweenNameOLD = "";
            }

            if (durationOLD != default(float))
            {
                _duration.Value = durationOLD;
                durationOLD = default(float);
            }       
        }

        #endregion
    }
}
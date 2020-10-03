// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Applies a camera shake effect to the main camera.
    /// </summary>
    [CommandInfo("LeanTween",
                 "Shake Camera",
                 "Applies a camera shake effect to the main camera.")]
    [AddComponentMenu("")]
    public abstract class ShakeCameraLean : Command
    {
        [Tooltip("The time in seconds the animation will take to complete")]
        [SerializeField]
        protected FloatData _duration = new FloatData(1f);

        [Tooltip("Amount to skake on each axis")]
        [SerializeField]
        protected Vector3Data _axisScale;

        [Tooltip("Amount to skake on each axis")]
        [SerializeField]
        protected Vector2Data _axisSpeedRange = new Vector2Data(new Vector2(5, 10));

        [Tooltip("Whether to animate in world space or relative to the parent. False by default.")]
        [SerializeField]
        protected bool isLocal;

        [Tooltip("The shape of the easing curve applied to the animation")]
        [SerializeField]
        protected LeanTweenType easeType = LeanTweenType.linear;

        [Tooltip("Stop any previously LeanTweens on this object before adding this one. Warning; expensive.")]
        [SerializeField]
        protected bool stopPreviousTweens = false;

        [Tooltip("Wait until the tween has finished before executing the next command")]
        [SerializeField]
        protected bool waitUntilFinished = true;

        [HideInInspector] protected LTDescr ourTween;

        protected virtual void OnTweenComplete()
        {
            Continue();
        }

        public override void OnEnter()
        {
            var targetTransform = Camera.main.transform;


            if (stopPreviousTweens)
            {
                LeanTween.cancel(targetTransform.gameObject);
            }

            if (isLocal)
                ourTween = LeanTweenHelpers.ShakePositionLocal(
                    targetTransform,
                    _axisScale.Value,
                    _axisSpeedRange.Value,
                    _duration.Value);
            else
                ourTween = LeanTweenHelpers.ShakePosition(
                    targetTransform,
                    _axisScale.Value,
                    _axisSpeedRange.Value,
                    _duration.Value);

            ourTween.setEase(easeType);

            if (waitUntilFinished)
            {
                if (ourTween != null)
                {
                    ourTween.setOnComplete(OnTweenComplete);
                }
            }
            else
            {
                Continue();
            }
        }

        public override string GetSummary()
        {
            return "over " + _duration.Value + " seconds";
        }

        public override Color GetButtonColor()
        {
            return new Color32(233, 163, 180, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return variable == _duration.floatRef ||
                _axisScale.vector3Ref == variable || _axisSpeedRange.vector2Ref == variable;
        }
    }
}

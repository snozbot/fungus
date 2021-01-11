// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Impulse style shake of an object's rotation, using LeanTween internally.
    /// </summary>
    [CommandInfo("LeanTween",
                 "Shake Rotation",
                 "")]
    [AddComponentMenu("Impulse style shake of an object's rotation, using LeanTween internally.")]
    public class ShakeRotationLean : ShakeLean
    {
        public override LTDescr ExecuteTween()
        {
            if (isLocal)
                return LeanTweenHelpers.ShakeEulerLocal(
                    _targetObject.Value.transform,
                    _axisScale.Value,
                    _axisSpeedRange.Value,
                    _duration.Value);
            else
                return LeanTweenHelpers.ShakeEuler(
                    _targetObject.Value.transform,
                    _axisScale.Value,
                    _axisSpeedRange.Value,
                    _duration.Value);
        }
    }
}

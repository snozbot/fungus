// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Impulse style shake of an object's scale, using LeanTween internally.
    /// </summary>
    [CommandInfo("LeanTween",
                 "Shake Scale",
                 "Impulse style shake of an object's scale, using LeanTween internally.")]
    [AddComponentMenu("")]
    public class ShakeScaleLean : ShakeLean
    {
        public override LTDescr ExecuteTween()
        {
            return LeanTweenHelpers.ShakeScale(
                _targetObject.Value.transform,
                _axisScale.Value,
                _axisSpeedRange.Value,
                _duration.Value);
        }
    }
}

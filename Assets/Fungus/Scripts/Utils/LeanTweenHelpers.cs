// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    public static class LeanTweenHelpers
    {
        public static readonly AnimationCurve ShakeScaleCurve = new AnimationCurve(
            new Keyframe(0, 0),
            new Keyframe(0.05f, 1),
            new Keyframe(1, 0));

        public static readonly Vector2 DefaultSpeedRange = new Vector2(5, 10);

        private const float PerlinStratingRange = 1000;

        internal static Vector3 PerlinStartingPoint
        {
            get
            {
                return new Vector3(Random.Range(-PerlinStratingRange, PerlinStratingRange),
                    Random.Range(-PerlinStratingRange, PerlinStratingRange),
                    Random.Range(-PerlinStratingRange, PerlinStratingRange));
            }
        }

        internal static Vector3 PerlinSpeed(Vector2 speedRange)
        {
            return new Vector3(Random.Range(speedRange.x, speedRange.y),
                Random.Range(speedRange.x, speedRange.y),
                Random.Range(speedRange.x, speedRange.y));
        }

        internal static LTDescr Shake(
            System.Func<Vector3> getStartingValue,
            System.Action<Vector3> setValue,
            Transform transform,
            Vector3 axisScale,
            Vector2 speedRange,
            float time)
        {
            var startingEuler = getStartingValue();
            var px = PerlinStartingPoint;
            var py = PerlinStartingPoint;
            var psx = PerlinSpeed(speedRange);
            var psy = PerlinSpeed(speedRange);
            var lt = LeanTween.value(0, 1, time)
                .setOnUpdate((float v) =>
                {
                    var n = new Vector3(
                        Mathf.PerlinNoise(px.x + v * psx.x, py.x + v * psy.x),
                        Mathf.PerlinNoise(px.y + v * psx.y, py.y + v * psy.y),
                        Mathf.PerlinNoise(px.z + v * psx.z, py.z + v * psy.z));
                    n *= 2;
                    n -= Vector3.one;
                    n.Scale(axisScale);
                    setValue(startingEuler + n * ShakeScaleCurve.Evaluate(v));
                })
                .setOnComplete(() => setValue(startingEuler));

            return lt;
        }

        public static LTDescr ShakeEuler(Transform transform, Vector3 axisScale, Vector2 speedRange, float time)
        {
            return Shake(
                () => transform.eulerAngles,
                (x) => transform.eulerAngles = x,
                transform, axisScale, speedRange, time);
        }

        public static LTDescr ShakeEulerLocal(Transform transform, Vector3 axisScale, Vector2 speedRange, float time)
        {
            return Shake(
                () => transform.localEulerAngles,
                (x) => transform.localEulerAngles = x,
                transform, axisScale, speedRange, time);
        }

        public static LTDescr ShakePosition(Transform transform, Vector3 axisScale, Vector2 speedRange, float time)
        {
            return Shake(
                () => transform.position,
                (x) => transform.position = x,
                transform, axisScale, speedRange, time);
        }

        public static LTDescr ShakePositionLocal(Transform transform, Vector3 axisScale, Vector2 speedRange, float time)
        {
            return Shake(
                () => transform.localPosition,
                (x) => transform.localPosition = x,
                transform, axisScale, speedRange, time);
        }

        public static LTDescr ShakeScale(Transform transform, Vector3 axisScale, Vector2 speedRange, float time)
        {
            return Shake(
                () => transform.localScale,
                (x) => transform.localScale = x,
                transform, axisScale, speedRange, time);
        }
    }
}

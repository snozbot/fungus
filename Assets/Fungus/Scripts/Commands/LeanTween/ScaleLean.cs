// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System;

namespace Fungus
{
    /// <summary>
    /// Changes a game object's scale to a specified value over time.
    /// </summary>
    [CommandInfo("LeanTween",
                 "Scale",
                 "Changes a game object's scale to a specified value over time.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class ScaleLean : BaseLeanTweenCommand
    {
        [Tooltip("Target transform that the GameObject will scale to")]
        [SerializeField]
        protected TransformData _toTransform;

        [Tooltip("Target scale that the GameObject will scale to, if no To Transform is set")]
        [SerializeField]
        protected Vector3Data _toScale = new Vector3Data(Vector3.one);

        public override LTDescr ExecuteTween()
        {
            var sc = _toTransform.Value == null ? _toScale.Value : _toTransform.Value.localScale;

            if (IsInAddativeMode)
            {
                sc += _targetObject.Value.transform.localScale;
            }

            if (IsInFromMode)
            {
                var cur = _targetObject.Value.transform.localScale;
                _targetObject.Value.transform.localScale = sc;
                sc = cur;
            }

            return LeanTween.scale(_targetObject.Value, sc, _duration);
        }
        
        public override bool HasReference(Variable variable)
        {
            return variable == _toTransform.transformRef || _toScale.vector3Ref == variable || base.HasReference(variable);
        }
    }
}

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System;

namespace Fungus
{
    /// <summary>
    /// Moves a game object to a specified position over time. The position can be defined by a transform in another object (using To Transform) or by setting an absolute position (using To Position, if To Transform is set to None).
    /// </summary>
    [CommandInfo("LeanTween",
                 "Move",
                 "Moves a game object to a specified position over time. The position can be defined by a transform in another object (using To Transform) or by setting an absolute position (using To Position, if To Transform is set to None).")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class MoveLean : BaseLeanTweenCommand
    {
        [Tooltip("Target transform that the GameObject will move to")]
        [SerializeField]
        protected TransformData _toTransform;

        [Tooltip("Target world position that the GameObject will move to, if no From Transform is set")]
        [SerializeField]
        protected Vector3Data _toPosition;

        [Tooltip("Whether to animate in world space or relative to the parent. False by default.")]
        [SerializeField]
        protected bool isLocal;
        

        public override LTDescr ExecuteTween()
        {
            var loc = _toTransform.Value == null ? _toPosition.Value : _toTransform.Value.position;

            if(IsInAddativeMode)
            {
                loc += _targetObject.Value.transform.position;
            }

            if(IsInFromMode)
            {
                var cur = _targetObject.Value.transform.position;
                _targetObject.Value.transform.position = loc;
                loc = cur;
            }

            if (isLocal)
                return LeanTween.moveLocal(_targetObject.Value, loc, _duration);
            else
                return LeanTween.move(_targetObject.Value, loc, _duration);
        }

        public override bool HasReference(Variable variable)
        {
            return _toTransform.transformRef == variable || _toPosition.vector3Ref == variable || base.HasReference(variable);
        }
    }
}
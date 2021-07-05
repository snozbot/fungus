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
                 "Size",
                 "Changes a gameObject's RectTransform size to a specified value over time.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SizeLean : BaseLeanTweenCommand
    {
        [Tooltip("Target transform that the GameObject will scale to")]
        [SerializeField]
        protected RectTransform _toRectTransform;

        [Tooltip("Target scale that the GameObject will scale to, if no To RectTransform is set")]
        [SerializeField]
        protected Vector2 _toScale = Vector2.one;

        public override LTDescr ExecuteTween()
        {
            var ui = _targetObject.Value.GetComponent<RectTransform>();
            var sc = _toRectTransform == null ? _toScale : _toRectTransform.sizeDelta;

            if (IsInAddativeMode)
            {
                sc += ui.sizeDelta;
            }

            if (IsInFromMode)
            {
                var cur = ui.sizeDelta;
                ui.sizeDelta = sc;
                sc = cur;
            }

            return LeanTween.size(ui, sc, _duration);
        }
        
        public override bool HasReference(Variable variable)
        {
            return variable == base.HasReference(variable);
        }
    }
}
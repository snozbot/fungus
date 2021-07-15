// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Select which type of fade will be applied.
    /// </summary>
    public enum FadeMode
    {
        /// <summary> Fade the alpha color component only. </summary>
        Alpha,
        /// <summary> Fade all color components (RGBA). </summary>
        Color
    }

    /// <summary>
    /// Fades a UI object.
    /// </summary>
    [CommandInfo("UI",
                 "Fade UI",
                 "Fades a UI object")]
    public class FadeUI : TweenUI 
    {
        [SerializeField] protected FadeMode fadeMode = FadeMode.Alpha;

        [SerializeField] protected ColorData targetColor = new ColorData(Color.white);

        [SerializeField] protected FloatData targetAlpha = new FloatData(1f);

        protected override void ApplyTween(GameObject go)
        {
            var images = go.GetComponentsInChildren<Image>();
            for (int i = 0; i < images.Length; i++)
            {
                var image = images[i];
                if (Mathf.Approximately(duration, 0f))
                {
                    switch (fadeMode)
                    {
                        case FadeMode.Alpha:
                            Color tempColor = image.color;
                            tempColor.a = targetAlpha;
                            image.color = tempColor;
                            break;
                        case FadeMode.Color:
                            image.color = targetColor;
                            break;
                    }
                }
                else
                {
                    switch (fadeMode)
                    {
                        case FadeMode.Alpha:
                            LeanTween.alpha(image.rectTransform, targetAlpha, duration).setEase(tweenType).setEase(tweenType);
                            break;
                        case FadeMode.Color:
                            LeanTween.color(image.rectTransform, targetColor, duration).setEase(tweenType).setEase(tweenType);
                            break;
                    }
                }
            }

            var texts = go.GetComponentsInChildren<Text>();
            for (int i = 0; i < texts.Length; i++)
            {
                var text = texts[i];
                if (Mathf.Approximately(duration, 0f))
                {
                    switch (fadeMode)
                    {
                        case FadeMode.Alpha:
                            Color tempColor = text.color;
                            tempColor.a = targetAlpha;
                            text.color = tempColor;
                            break;
                        case FadeMode.Color:
                            text.color = targetColor;
                            break;
                    }
                }
                else
                {
                    switch (fadeMode)
                    {
                        case FadeMode.Alpha:
                            LeanTween.textAlpha(text.rectTransform, targetAlpha, duration).setEase(tweenType);
                            break;
                        case FadeMode.Color:
                            LeanTween.textColor(text.rectTransform, targetColor, duration).setEase(tweenType);
                            break;
                    }
                }
            }

            var textMeshes = go.GetComponentsInChildren<TextMesh>();
            for (int i = 0; i < textMeshes.Length; i++)
            {
                var textMesh = textMeshes[i];
                if (Mathf.Approximately(duration, 0f))
                {
                    switch (fadeMode)
                    {
                        case FadeMode.Alpha:
                            Color tempColor = textMesh.color;
                            tempColor.a = targetAlpha;
                            textMesh.color = tempColor;
                            break;
                        case FadeMode.Color:
                            textMesh.color = targetColor;
                            break;
                    }
                }
                else
                {
                    switch (fadeMode)
                    {
                        case FadeMode.Alpha:
                            LeanTween.alpha(go, targetAlpha, duration).setEase(tweenType);
                            break;
                        case FadeMode.Color:
                            LeanTween.color(go, targetColor, duration).setEase(tweenType);
                            break;
                    }
                }
            }

#if UNITY_2018_1_OR_NEWER
            var tmpros = go.GetComponentsInChildren<TMPro.TMP_Text>();
            for (int i = 0; i < tmpros.Length; i++)
            {
            
                var tmpro = tmpros[i];
                if (Mathf.Approximately(duration, 0f))
                {
                    switch (fadeMode)
                    {
                    case FadeMode.Alpha:
                        Color tempColor = tmpro.color;
                        tempColor.a = targetAlpha;
                        tmpro.color = tempColor;
                        break;
                    case FadeMode.Color:
                        tmpro.color = targetColor;
                        break;
                    }
                }
                else
                {
                    switch (fadeMode)
                    {
                    case FadeMode.Alpha:
                        LeanTween.value(tmpro.gameObject, tmpro.color.a, targetAlpha.Value, duration)
                                 .setEase(tweenType)
                                 .setOnUpdate((float alphaValue) =>
                                 {
                                     Color tempColor = tmpro.color;
                                     tempColor.a = alphaValue;
                                     tmpro.color = tempColor;
                                 });
                        break;
                    case FadeMode.Color:
                        LeanTween.value(tmpro.gameObject, tmpro.color, targetColor.Value, duration)
                                 .setEase(tweenType)
                                 .setOnUpdate((Color colorValue) =>
                                 {
                                     tmpro.color = colorValue;
                                 });
                        break;
                    }
                }
            }
#endif
            //canvas groups don't support color but we can anim the alpha IN the color
            var canvasGroups = go.GetComponentsInChildren<CanvasGroup>();
            for (int i = 0; i < canvasGroups.Length; i++)
            {
                var canvasGroup = canvasGroups[i];
                if (Mathf.Approximately(duration, 0f))
                {
                    switch (fadeMode)
                    {
                        case FadeMode.Alpha:
                            canvasGroup.alpha = targetAlpha.Value;
                            break;
                        case FadeMode.Color:
                            canvasGroup.alpha = targetColor.Value.a;
                        break;
                    }
                }
                else
                {
                    switch (fadeMode)
                    {
                        case FadeMode.Alpha:
                            LeanTween.alphaCanvas(canvasGroup, targetAlpha, duration).setEase(tweenType);
                            break;
                        case FadeMode.Color:
                            LeanTween.alphaCanvas(canvasGroup, targetColor.Value.a, duration).setEase(tweenType);
                        break;
                    }
                }
            }
        }

        protected override string GetSummaryValue()
        {
            if (fadeMode == FadeMode.Alpha)
            {
                return targetAlpha.Value.ToString() + " alpha";
            }
            else if (fadeMode == FadeMode.Color)
            {
                return targetColor.Value.ToString()  + " color";
            }

            return "";
        }

        #region Public members

        public override bool IsPropertyVisible(string propertyName)
        {
            if (fadeMode == FadeMode.Alpha &&
                propertyName == "targetColor")
            {
                return false;
            }

            if (fadeMode == FadeMode.Color &&
                propertyName == "targetAlpha")
            {
                return false;
            }

            return true;
        }

        public override bool HasReference(Variable variable)
        {
            return targetColor.colorRef == variable || targetAlpha.floatRef == variable ||
                base.HasReference(variable);
        }

        #endregion
    }
}

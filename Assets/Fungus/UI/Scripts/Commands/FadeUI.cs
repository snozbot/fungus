using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Fungus;

namespace Fungus
{

	[CommandInfo("UI",
	             "Fade UI",
	             "Fades a UI object")]
	public class FadeUI : TweenUI 
	{
		public enum FadeMode
		{
			Alpha,
			Color
		}

		public FadeMode fadeMode = FadeMode.Alpha;

		public ColorData targetColor = new ColorData(Color.white);

		public FloatData targetAlpha = new FloatData(1f);

		protected override void ApplyTween(GameObject go)
		{
			foreach (Image image in go.GetComponentsInChildren<Image>())
			{
				if (duration == 0f)
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

			foreach (Text text in go.GetComponentsInChildren<Text>())
			{
				if (duration == 0f)
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

			foreach (TextMesh textMesh in go.GetComponentsInChildren<TextMesh>())
			{
				if (duration == 0f)
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
		}

		protected override string GetSummaryValue()
		{
			return targetAlpha.Value.ToString();
		}

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
	}

}

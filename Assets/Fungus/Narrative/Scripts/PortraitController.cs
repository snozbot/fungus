using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	public class PortraitController : MonoBehaviour
	{
		// Timer for waitUntilFinished functionality
		protected float waitTimer;

		protected Stage stage;

		void Awake()
		{
			waitTimer = 0f;
			stage = GetComponentInParent<Stage>();
		}

		public void CreatePortraitObject(Character character, float fadeDuration)
		{
			// Create a new portrait object
			GameObject portraitObj = new GameObject(character.name,
													typeof(RectTransform),
													typeof(CanvasRenderer),
													typeof(Image));

			// Set it to be a child of the stage
			portraitObj.transform.SetParent(stage.portraitCanvas.transform, true);

			// Configure the portrait image
			Image portraitImage = portraitObj.GetComponent<Image>();
			portraitImage.preserveAspect = true;
			portraitImage.sprite = character.profileSprite;
			portraitImage.color = new Color(1f, 1f, 1f, 0f);

			// LeanTween doesn't handle 0 duration properly
			float duration = (fadeDuration > 0f) ? fadeDuration : float.Epsilon;

			// Fade in character image (first time)
			LeanTween.alpha(portraitImage.transform as RectTransform, 1f, duration).setEase(stage.fadeEaseType);

			// Tell character about portrait image
			character.state.portraitImage = portraitImage;
		}

		public virtual IEnumerator WaitUntilFinished(float duration, Action Continue)
		{
			// Wait until the timer has expired
			// Any method can modify this timer variable to delay continuing.

			waitTimer = duration;
			while (waitTimer > 0f)
			{
				waitTimer -= Time.deltaTime;
				yield return null;
			}

			Continue();
		}

		public void SetupPortrait(Character character, RectTransform fromPosition, FacingDirection facing)
		{
			SetRectTransform(character.state.portraitImage.rectTransform, fromPosition);

			if (character.state.facing != character.portraitsFace)
			{
				character.state.portraitImage.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
			}
			else
			{
				character.state.portraitImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
			}

			if (facing != character.portraitsFace)
			{
				character.state.portraitImage.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
			}
			else
			{
				character.state.portraitImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
			}
		}

		public static void SetRectTransform(RectTransform oldRectTransform, RectTransform newRectTransform)
		{
			oldRectTransform.eulerAngles = newRectTransform.eulerAngles;
			oldRectTransform.position = newRectTransform.position;
			oldRectTransform.rotation = newRectTransform.rotation;
			oldRectTransform.anchoredPosition = newRectTransform.anchoredPosition;
			oldRectTransform.sizeDelta = newRectTransform.sizeDelta;
			oldRectTransform.anchorMax = newRectTransform.anchorMax;
			oldRectTransform.anchorMin = newRectTransform.anchorMin;
			oldRectTransform.pivot = newRectTransform.pivot;
			oldRectTransform.localScale = newRectTransform.localScale;
		}

		public void MoveToFront(Character character)
		{
			character.state.portraitImage.transform.SetSiblingIndex(character.state.portraitImage.transform.parent.childCount);
		}

		public void DoMoveTween(Character character, RectTransform fromPosition, RectTransform toPosition, float moveDuration, Boolean waitUntilFinished)
		{
			// LeanTween doesn't handle 0 duration properly
			float duration = (moveDuration > 0f) ? moveDuration : float.Epsilon;

			// LeanTween.move uses the anchoredPosition, so all position images must have the same anchor position
			LeanTween.move(character.state.portraitImage.gameObject, toPosition.position, duration).setEase(stage.fadeEaseType);
			if (waitUntilFinished)
			{
				waitTimer = duration;
			}
		}
	}
}

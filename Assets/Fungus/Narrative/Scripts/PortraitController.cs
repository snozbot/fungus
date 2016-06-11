using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
	public struct PortraitOptions
	{
		public Character character;
		public Character replacedCharacter;
		public Sprite portrait;
		public DisplayType display;
		public PositionOffset offset;
		public RectTransform fromPosition;
		public RectTransform toPosition;
		public FacingDirection facing;
		public bool useDefaultSettings;
		public float fadeDuration;
		public float moveDuration;
		public Vector2 shiftOffset;
		public bool move;
		public bool shiftIntoPlace;
		public bool waitUntilFinished;
		public Action onComplete;

		public PortraitOptions(bool useDefaultSettings = true)
		{
			// Defaults usually assigned on constructing a struct
			character = null;
			replacedCharacter = null;
			portrait = null;
			display = DisplayType.None;
			offset = PositionOffset.None;
			fromPosition = null;
			toPosition = null;
			facing = FacingDirection.None;
			shiftOffset = new Vector2(0,0);
			move = false;
			shiftIntoPlace = false;
			waitUntilFinished = false;
			onComplete = null;

			// Special values that can be overriden
			fadeDuration = 0.5f;
			moveDuration = 1f;
			this.useDefaultSettings = useDefaultSettings;
		}

		public override string ToString()
		{
			return base.ToString();
		}
	}

	public struct PortraitState
	{
		public bool onScreen;
		public bool dimmed;
		public DisplayType display;
		public Sprite portrait;
		public RectTransform position;
		public FacingDirection facing;
		public Image portraitImage;
	}

	public enum DisplayType
	{
		None,
		Show,
		Hide,
		Replace,
		MoveToFront
	}

	public enum FacingDirection
	{
		None,
		Left,
		Right
	}

	public enum PositionOffset
	{
		None,
		OffsetLeft,
		OffsetRight
	}
	
	public class PortraitController : MonoBehaviour
	{
		// Timer for waitUntilFinished functionality
		protected float waitTimer;

		protected Stage stage;
        
		void Awake()
		{
			stage = GetComponentInParent<Stage>();
		}

		// Using this function, run any portriat command available
		public void RunPortraitCommand(PortraitOptions options, Action onComplete)
		{
            waitTimer = 0f;
            
            // If no character specified, do nothing
            if (options.character == null)
			{
				onComplete();
				return;
			}

			// If Replace and no replaced character specified, do nothing
			if (options.display == DisplayType.Replace && options.replacedCharacter == null)
			{
				onComplete();
				return;
			}
			
			// Early out if hiding a character that's already hidden
			if (options.display == DisplayType.Hide &&
				!options.character.state.onScreen)
			{
				onComplete();
				return;
			}

            options = CleanPortraitOptions(options);
			options.onComplete = onComplete;

			switch (options.display)
			{
				case (DisplayType.Show):
					Show(options);
					break;

				case (DisplayType.Hide):
					Hide(options);
					break;

				case (DisplayType.Replace):
					Show(options);
					Hide(options.replacedCharacter, options.replacedCharacter.state.position, options.replacedCharacter.state.position);
				    break;

				case (DisplayType.MoveToFront):
					MoveToFront(options);
					break;
			}
			
		}

		private void FinishCommand(PortraitOptions options)
		{
			if (options.onComplete != null)
			{
				if (!options.waitUntilFinished)
				{
					options.onComplete();
				}
				else
				{
					StartCoroutine(WaitUntilFinished(options.fadeDuration, options.onComplete));
				}
			}
			else
			{
				StartCoroutine(WaitUntilFinished(options.fadeDuration));
			}
		}

        private PortraitOptions CleanPortraitOptions(PortraitOptions options)
        {
			// Use default stage settings
            if (options.useDefaultSettings)
            {
                options.fadeDuration = stage.fadeDuration;
                options.moveDuration = stage.moveDuration;
                options.shiftOffset = stage.shiftOffset;
            }

            // if no previous portrait, use default portrait
            if (options.character.state.portrait == null)
            {
                options.character.state.portrait = options.character.profileSprite;
            }

            // Selected "use previous portrait"
            if (options.portrait == null)
            {
                options.portrait = options.character.state.portrait;
            }

            // if no previous position, use default position
            if (options.character.state.position == null)
            {
                options.character.state.position = stage.defaultPosition.rectTransform;
            }

            // Selected "use previous position"
            if (options.toPosition == null)
            {
                options.toPosition = options.character.state.position;
            }

            if (options.replacedCharacter != null)
            {
                // if no previous position, use default position
                if (options.replacedCharacter.state.position == null)
                {
                    options.replacedCharacter.state.position = stage.defaultPosition.rectTransform;
                }
            }

            // If swapping, use replaced character's position
            if (options.display == DisplayType.Replace)
            {
                options.toPosition = options.replacedCharacter.state.position;
            }

            // Selected "use previous position"
            if (options.fromPosition == null)
            {
                options.fromPosition = options.character.state.position;
            }

            // if portrait not moving, use from position is same as to position
            if (!options.move)
            {
                options.fromPosition = options.toPosition;
            }

            if (options.display == DisplayType.Hide)
            {
                options.fromPosition = options.character.state.position;
            }

            // if no previous facing direction, use default facing direction
            if (options.character.state.facing == FacingDirection.None)
            {
                options.character.state.facing = options.character.portraitsFace;
            }

            // Selected "use previous facing direction"
            if (options.facing == FacingDirection.None)
            {
                options.facing = options.character.state.facing;
            }

            if (options.character.state.portraitImage == null)
            {
                CreatePortraitObject(options.character, options.fadeDuration);
            }

            return options;
        }

		private void CreatePortraitObject(Character character, float fadeDuration)
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

		public virtual IEnumerator WaitUntilFinished(float duration, Action onComplete = null)
		{
			// Wait until the timer has expired
			// Any method can modify this timer variable to delay continuing.

			waitTimer = duration;
			while (waitTimer > 0f)
			{
				waitTimer -= Time.deltaTime;
				yield return null;
			}

			if (onComplete != null)
			{
				onComplete();
			}
		}

		private void SetupPortrait(PortraitOptions options)
		{
			SetRectTransform(options.character.state.portraitImage.rectTransform, options.fromPosition);

			if (options.character.state.facing != options.character.portraitsFace)
			{
                options.character.state.portraitImage.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
			}
			else
			{
                options.character.state.portraitImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
			}

			if (options.facing != options.character.portraitsFace)
			{
                options.character.state.portraitImage.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
			}
			else
			{
                options.character.state.portraitImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
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
			PortraitOptions options = new PortraitOptions(true);
			options.character = character;
			
			MoveToFront(CleanPortraitOptions(options));
		}

		public void MoveToFront(PortraitOptions options)
		{
			options.character.state.portraitImage.transform.SetSiblingIndex(options.character.state.portraitImage.transform.parent.childCount);
            options.character.state.display = DisplayType.MoveToFront;
			FinishCommand(options);
		}

        public void DoMoveTween(Character character, RectTransform fromPosition, RectTransform toPosition, float moveDuration, Boolean waitUntilFinished)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.fromPosition = fromPosition;
            options.toPosition = toPosition;
            options.moveDuration = moveDuration;
            options.waitUntilFinished = waitUntilFinished;

            DoMoveTween(CleanPortraitOptions(options));
        }

        public void DoMoveTween(PortraitOptions options)
        {
            // LeanTween doesn't handle 0 duration properly
            float duration = (options.moveDuration > 0f) ? options.moveDuration : float.Epsilon;

			// LeanTween.move uses the anchoredPosition, so all position images must have the same anchor position
			LeanTween.move(options.character.state.portraitImage.gameObject, options.toPosition.position, duration).setEase(stage.fadeEaseType);

			if (options.waitUntilFinished)
			{
				waitTimer = duration;
			}
		}

		public void Show(Character character, RectTransform position)
		{
			Show(character, position, position);
		}

		public void Show(Character character, RectTransform fromPosition, RectTransform toPosition)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.fromPosition = fromPosition;
            options.toPosition = toPosition;

            Show(CleanPortraitOptions(options));
        }
        
        public void Show(PortraitOptions options)
        {
            if (options.shiftIntoPlace)
			{
				options.fromPosition = Instantiate(options.toPosition) as RectTransform;
				if (options.offset == PositionOffset.OffsetLeft)
				{
                    options.fromPosition.anchoredPosition = 
                        new Vector2(options.fromPosition.anchoredPosition.x - Mathf.Abs(options.shiftOffset.x), 
                        options.fromPosition.anchoredPosition.y - Mathf.Abs(options.shiftOffset.y));
				}
				else if (options.offset == PositionOffset.OffsetRight)
				{
                    options.fromPosition.anchoredPosition =
                        new Vector2(options.fromPosition.anchoredPosition.x + Mathf.Abs(options.shiftOffset.x),
                        options.fromPosition.anchoredPosition.y + Mathf.Abs(options.shiftOffset.y));
				}
				else
				{
					options.fromPosition.anchoredPosition = new Vector2(options.fromPosition.anchoredPosition.x, options.fromPosition.anchoredPosition.y);
				}
			}

			SetupPortrait(options);

			// LeanTween doesn't handle 0 duration properly
			float duration = (options.fadeDuration > 0f) ? options.fadeDuration : float.Epsilon;

			// Fade out a duplicate of the existing portrait image
			if (options.character.state.portraitImage != null)
			{
				GameObject tempGO = GameObject.Instantiate(options.character.state.portraitImage.gameObject);
				tempGO.transform.SetParent(options.character.state.portraitImage.transform, false);
				tempGO.transform.localPosition = Vector3.zero;
				tempGO.transform.localScale = options.character.state.position.localScale;

				Image tempImage = tempGO.GetComponent<Image>();
				tempImage.sprite = options.character.state.portraitImage.sprite;
				tempImage.preserveAspect = true;
				tempImage.color = options.character.state.portraitImage.color;

				LeanTween.alpha(tempImage.rectTransform, 0f, duration).setEase(stage.fadeEaseType).setOnComplete(() => {
					Destroy(tempGO);
				});
			}

            // Fade in the new sprite image
            options.character.state.portraitImage.sprite = options.portrait;
            options.character.state.portraitImage.color = new Color(1f, 1f, 1f, 0f);
			LeanTween.alpha(options.character.state.portraitImage.rectTransform, 1f, duration).setEase(stage.fadeEaseType);

			DoMoveTween(options);

			FinishCommand(options);

			if (!stage.charactersOnStage.Contains(options.character))
            {
                stage.charactersOnStage.Add(options.character);
            }

			// Update character state after showing
			options.character.state.onScreen = true;
			options.character.state.display = DisplayType.Show;
			options.character.state.portrait = options.portrait;
			options.character.state.facing = options.facing;
			options.character.state.position = options.toPosition;
		}
        
        public void Hide(Character character)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.fromPosition = character.state.position;
            options.toPosition = character.state.position;
            Hide(CleanPortraitOptions(options));
        }

        public void Hide(Character character, RectTransform fromPosition, RectTransform toPosition)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.fromPosition = fromPosition;
            options.toPosition = toPosition;

            Hide(CleanPortraitOptions(options));
        }

        public void Hide(PortraitOptions options)
		{
			if (options.character.state.display == DisplayType.None)
			{
				return;
			}

			SetupPortrait(options);

			// LeanTween doesn't handle 0 duration properly
			float duration = (options.fadeDuration > 0f) ? options.fadeDuration : float.Epsilon;

			LeanTween.alpha(options.character.state.portraitImage.rectTransform, 0f, duration).setEase(stage.fadeEaseType);

			DoMoveTween(options);
			
			stage.charactersOnStage.Remove(options.character);

			//update character state after hiding
			options.character.state.onScreen = false;
			options.character.state.portrait = options.portrait;
			options.character.state.facing = options.facing;
			options.character.state.position = options.toPosition;

			FinishCommand(options);
		}

		public void SetDimmed(Character character, bool dimmedState)
		{
			if (character.state.dimmed == dimmedState)
			{
				return;
			}

			character.state.dimmed = dimmedState;

			Color targetColor = dimmedState ? new Color(0.5f, 0.5f, 0.5f, 1f) : Color.white;

			// LeanTween doesn't handle 0 duration properly
			float duration = (stage.fadeDuration > 0f) ? stage.fadeDuration : float.Epsilon;

			LeanTween.color(character.state.portraitImage.rectTransform, targetColor, duration).setEase(stage.fadeEaseType);
		}
	}
}

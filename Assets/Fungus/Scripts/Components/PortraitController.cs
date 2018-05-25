// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using MoonSharp.Interpreter;

namespace Fungus
{
    /// <summary>
    /// Types of display operations supported by portraits.
    /// </summary>
    public enum DisplayType
    {
        /// <summary> Do nothing. </summary>
        None,
        /// <summary> Show the portrait. </summary>
        Show,
        /// <summary> Hide the portrait. </summary>
        Hide,
        /// <summary> Replace the existing portrait. </summary>
        Replace,
        /// <summary> Move portrait to the front. </summary>
        MoveToFront
    }

    /// <summary>
    /// Directions that character portraits can face.
    /// </summary>
    public enum FacingDirection
    {
        /// <summary> Unknown direction </summary>
        None,
        /// <summary> Facing left. </summary>
        Left,
        /// <summary> Facing right. </summary>
        Right
    }

    /// <summary>
    /// Offset direction for position.
    /// </summary>
    public enum PositionOffset
    {
        /// <summary> Unknown offset direction. </summary>
        None,
        /// <summary> Offset applies to the left. </summary>
        OffsetLeft,
        /// <summary> Offset applies to the right. </summary>
        OffsetRight
    }

    /// <summary>
    /// Controls the Portrait sprites on stage
    /// </summary>
    public class PortraitController : MonoBehaviour
    {
        // Timer for waitUntilFinished functionality
        protected float waitTimer;

        protected Stage stage;

        protected virtual void Awake()
        {
            stage = GetComponentInParent<Stage>();
        }

        protected virtual void FinishCommand(PortraitOptions options)
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

        /// <summary>
        /// Makes sure all options are set correctly so it won't break whatever command it's sent to
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        protected virtual PortraitOptions CleanPortraitOptions(PortraitOptions options)
        {
            // Use default stage settings
            if (options.useDefaultSettings)
            {
                options.fadeDuration = stage.FadeDuration;
                options.moveDuration = stage.MoveDuration;
                options.shiftOffset = stage.ShiftOffset;
            }

            // if no previous portrait, use default portrait
            if (options.character.State.portrait == null)
            {
                options.character.State.portrait = options.character.ProfileSprite;
            }

            // Selected "use previous portrait"
            if (options.portrait == null)
            {
                options.portrait = options.character.State.portrait;
            }

            // if no previous position, use default position
            if (options.character.State.position == null)
            {
                options.character.State.position = stage.DefaultPosition.rectTransform;
            }

            // Selected "use previous position"
            if (options.toPosition == null)
            {
                options.toPosition = options.character.State.position;
            }

            if (options.replacedCharacter != null)
            {
                // if no previous position, use default position
                if (options.replacedCharacter.State.position == null)
                {
                    options.replacedCharacter.State.position = stage.DefaultPosition.rectTransform;
                }
            }

            // If swapping, use replaced character's position
            if (options.display == DisplayType.Replace)
            {
                options.toPosition = options.replacedCharacter.State.position;
            }

            // Selected "use previous position"
            if (options.fromPosition == null)
            {
                options.fromPosition = options.character.State.position;
            }

            // if portrait not moving, use from position is same as to position
            if (!options.move)
            {
                options.fromPosition = options.toPosition;
            }

            if (options.display == DisplayType.Hide)
            {
                options.fromPosition = options.character.State.position;
            }

            // if no previous facing direction, use default facing direction
            if (options.character.State.facing == FacingDirection.None)
            {
                options.character.State.facing = options.character.PortraitsFace;
            }

            // Selected "use previous facing direction"
            if (options.facing == FacingDirection.None)
            {
                options.facing = options.character.State.facing;
            }

            if (options.character.State.portraitImage == null)
            {
                CreatePortraitObject(options.character, options.fadeDuration);
            }

            return options;
        }

        /// <summary>
        /// Creates and sets the portrait image for a character
        /// </summary>
        /// <param name="character"></param>
        /// <param name="fadeDuration"></param>
        protected virtual void CreatePortraitObject(Character character, float fadeDuration)
        {
            // Create a new portrait object
            GameObject portraitObj = new GameObject(character.name,
                                                    typeof(RectTransform),
                                                    typeof(CanvasRenderer),
                                                    typeof(Image));

            // Set it to be a child of the stage
            portraitObj.transform.SetParent(stage.PortraitCanvas.transform, true);

            // Configure the portrait image
            Image portraitImage = portraitObj.GetComponent<Image>();
            portraitImage.preserveAspect = true;
            portraitImage.sprite = character.ProfileSprite;
            portraitImage.color = new Color(1f, 1f, 1f, 0f);

            // LeanTween doesn't handle 0 duration properly
            float duration = (fadeDuration > 0f) ? fadeDuration : float.Epsilon;

            // Fade in character image (first time)
            LeanTween.alpha(portraitImage.transform as RectTransform, 1f, duration).setEase(stage.FadeEaseType).setRecursive(false);

            // Tell character about portrait image
            character.State.portraitImage = portraitImage;
        }

        protected virtual IEnumerator WaitUntilFinished(float duration, Action onComplete = null)
        {
            // Wait until the timer has expired
            // Any method can modify this timer variable to delay continuing.

            waitTimer = duration;
            while (waitTimer > 0f)
            {
                waitTimer -= Time.deltaTime;
                yield return null;
            }

            // Wait until next frame just to be safe
            yield return new WaitForEndOfFrame();

            if (onComplete != null)
            {
                onComplete();
            }
        }

        protected virtual void SetupPortrait(PortraitOptions options)
        {
            SetRectTransform(options.character.State.portraitImage.rectTransform, options.fromPosition);

            if (options.character.State.facing != options.character.PortraitsFace)
            {
                options.character.State.portraitImage.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else
            {
                options.character.State.portraitImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            }

            if (options.facing != options.character.PortraitsFace)
            {
                options.character.State.portraitImage.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
            }
            else
            {
                options.character.State.portraitImage.rectTransform.localScale = new Vector3(1f, 1f, 1f);
            }
        }

        protected virtual void DoMoveTween(Character character, RectTransform fromPosition, RectTransform toPosition, float moveDuration, Boolean waitUntilFinished)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.fromPosition = fromPosition;
            options.toPosition = toPosition;
            options.moveDuration = moveDuration;
            options.waitUntilFinished = waitUntilFinished;

            DoMoveTween(options);
        }

        protected virtual void DoMoveTween(PortraitOptions options)
        {
            CleanPortraitOptions(options);

            // LeanTween doesn't handle 0 duration properly
            float duration = (options.moveDuration > 0f) ? options.moveDuration : float.Epsilon;

            // LeanTween.move uses the anchoredPosition, so all position images must have the same anchor position
            LeanTween.move(options.character.State.portraitImage.gameObject, options.toPosition.position, duration).setEase(stage.FadeEaseType);

            if (options.waitUntilFinished)
            {
                waitTimer = duration;
            }
        }

        #region Public members

        /// <summary>
        /// Performs a deep copy of all values from one RectTransform to another.
        /// </summary>
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

        /// <summary>
        /// Using all portrait options available, run any portrait command.
        /// </summary>
        /// <param name="options">Portrait Options</param>
        /// <param name="onComplete">The function that will run once the portrait command finishes</param>
        public virtual void RunPortraitCommand(PortraitOptions options, Action onComplete)
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
                !options.character.State.onScreen)
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
                    Hide(options.replacedCharacter, options.replacedCharacter.State.position.name);
                    break;

                case (DisplayType.MoveToFront):
                    MoveToFront(options);
                    break;
            }
        }

        /// <summary>
        /// Moves Character in front of other characters on stage
        /// </summary>
        public virtual void MoveToFront(Character character)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;

            MoveToFront(CleanPortraitOptions(options));
        }

        /// <summary>
        /// Moves Character in front of other characters on stage
        /// </summary>
        public virtual void MoveToFront(PortraitOptions options)
        {
            options.character.State.portraitImage.transform.SetSiblingIndex(options.character.State.portraitImage.transform.parent.childCount);
            options.character.State.display = DisplayType.MoveToFront;
            FinishCommand(options);
        }

        /// <summary>
        /// Shows character at a named position in the stage
        /// </summary>
        /// <param name="character"></param>
        /// <param name="position">Named position on stage</param>
        public virtual void Show(Character character, string position)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.fromPosition = options.toPosition = stage.GetPosition(position);

            Show(options);
        }

        /// <summary>
        /// Shows character moving from a position to a position
        /// </summary>
        /// <param name="character"></param>
        /// <param name="portrait"></param>
        /// <param name="fromPosition">Where the character will appear</param>
        /// <param name="toPosition">Where the character will move to</param>
        public virtual void Show(Character character, string portrait, string fromPosition, string toPosition)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.portrait = character.GetPortrait(portrait);
            options.fromPosition = stage.GetPosition(fromPosition);
            options.toPosition = stage.GetPosition(toPosition);
            options.move = true;

            Show(options);
        }

        /// <summary>
        /// From lua, you can pass an options table with named arguments
        /// example:
        ///     stage.show{character=jill, portrait="happy", fromPosition="right", toPosition="left"}
        /// Any option available in the PortraitOptions is available from Lua
        /// </summary>
        /// <param name="optionsTable">Moonsharp Table</param>
        public virtual void Show(Table optionsTable)
        {
            Show(PortraitUtil.ConvertTableToPortraitOptions(optionsTable, stage));
        }

        /// <summary>
        /// Show portrait with the supplied portrait options
        /// </summary>
        /// <param name="options"></param>
        public virtual void Show(PortraitOptions options)
        {
            options = CleanPortraitOptions(options);

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
            if (options.character.State.portraitImage != null && options.character.State.portraitImage.sprite != null)
            {
                GameObject tempGO = GameObject.Instantiate(options.character.State.portraitImage.gameObject);
                tempGO.transform.SetParent(options.character.State.portraitImage.transform, false);
                tempGO.transform.localPosition = Vector3.zero;
                tempGO.transform.localScale = options.character.State.position.localScale;

                Image tempImage = tempGO.GetComponent<Image>();
                tempImage.sprite = options.character.State.portraitImage.sprite;
                tempImage.preserveAspect = true;
                tempImage.color = options.character.State.portraitImage.color;

                LeanTween.alpha(tempImage.rectTransform, 0f, duration).setEase(stage.FadeEaseType).setOnComplete(() => {
                    Destroy(tempGO);
                }).setRecursive(false);
            }

            // Fade in the new sprite image
            if (options.character.State.portraitImage.sprite != options.portrait ||
                options.character.State.portraitImage.color.a < 1f)
            {
                options.character.State.portraitImage.sprite = options.portrait;
                options.character.State.portraitImage.color = new Color(1f, 1f, 1f, 0f);
                LeanTween.alpha(options.character.State.portraitImage.rectTransform, 1f, duration).setEase(stage.FadeEaseType).setRecursive(false);
            }

            DoMoveTween(options);

            FinishCommand(options);

            if (!stage.CharactersOnStage.Contains(options.character))
            {
                stage.CharactersOnStage.Add(options.character);
            }

            // Update character state after showing
            options.character.State.onScreen = true;
            options.character.State.display = DisplayType.Show;
            options.character.State.portrait = options.portrait;
            options.character.State.facing = options.facing;
            options.character.State.position = options.toPosition;
        }

        /// <summary>
        /// Simple show command that shows the character with an available named portrait
        /// </summary>
        /// <param name="character">Character to show</param>
        /// <param name="portrait">Named portrait to show for the character, i.e. "angry", "happy", etc</param>
        public virtual void ShowPortrait(Character character, string portrait)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.portrait = character.GetPortrait(portrait);

            if (character.State.position == null)
            {
                options.toPosition = options.fromPosition = stage.GetPosition("middle");
            }
            else
            {
                options.fromPosition = options.toPosition = character.State.position;
            }

            Show(options);
        }

        /// <summary>
        /// Simple character hide command
        /// </summary>
        /// <param name="character">Character to hide</param>
        public virtual void Hide(Character character)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;

            Hide(options);
        }

        /// <summary>
        /// Move the character to a position then hide it
        /// </summary>
        /// <param name="character">Character to hide</param>
        /// <param name="toPosition">Where the character will disapear to</param>
        public virtual void Hide(Character character, string toPosition)
        {
            PortraitOptions options = new PortraitOptions(true);
            options.character = character;
            options.toPosition = stage.GetPosition(toPosition);
            options.move = true;

            Hide(options);
        }

        /// <summary>
        /// From lua, you can pass an options table with named arguments
        /// example:
        ///     stage.hide{character=jill, toPosition="left"}
        /// Any option available in the PortraitOptions is available from Lua
        /// </summary>
        /// <param name="optionsTable">Moonsharp Table</param>
        public virtual void Hide(Table optionsTable)
        {
            Hide(PortraitUtil.ConvertTableToPortraitOptions(optionsTable, stage));
        }

        /// <summary>
        /// Hide portrait with provided options
        /// </summary>
        public virtual void Hide(PortraitOptions options)
        {
            CleanPortraitOptions(options);

            if (options.character.State.display == DisplayType.None)
            {
                return;
            }

            SetupPortrait(options);

            // LeanTween doesn't handle 0 duration properly
            float duration = (options.fadeDuration > 0f) ? options.fadeDuration : float.Epsilon;

            LeanTween.alpha(options.character.State.portraitImage.rectTransform, 0f, duration).setEase(stage.FadeEaseType).setRecursive(false);

            DoMoveTween(options);

            stage.CharactersOnStage.Remove(options.character);

            //update character state after hiding
            options.character.State.onScreen = false;
            options.character.State.portrait = options.portrait;
            options.character.State.facing = options.facing;
            options.character.State.position = options.toPosition;
            options.character.State.display = DisplayType.Hide;

            FinishCommand(options);
        }

        /// <summary>
        /// Sets the dimmed state of a character on the stage.
        /// </summary>
        public virtual void SetDimmed(Character character, bool dimmedState)
        {
            if (character.State.dimmed == dimmedState)
            {
                return;
            }

            character.State.dimmed = dimmedState;

            Color targetColor = dimmedState ? stage.DimColor : Color.white;

            // LeanTween doesn't handle 0 duration properly
            float duration = (stage.FadeDuration > 0f) ? stage.FadeDuration : float.Epsilon;

            LeanTween.color(character.State.portraitImage.rectTransform, targetColor, duration).setEase(stage.FadeEaseType).setRecursive(false);
        }

        #endregion
    }
}

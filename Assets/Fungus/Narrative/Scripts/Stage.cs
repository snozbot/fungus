// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Define a set of screen positions where character sprites can be displayed.
    /// </summary>
    [ExecuteInEditMode]
    public class Stage : PortraitController, IStage
    {
        [Tooltip("Canvas object containing the stage positions.")]
        [SerializeField] protected Canvas portraitCanvas;

        [Tooltip("Dim portraits when a character is not speaking.")]
        [SerializeField] protected bool dimPortraits;

        [Tooltip("Duration for fading character portraits in / out.")]
        [SerializeField] protected float fadeDuration = 0.5f;

        [Tooltip("Duration for moving characters to a new position")]
        [SerializeField] protected float moveDuration = 1f;

        [Tooltip("Ease type for the fade tween.")]
        [SerializeField] protected LeanTweenType fadeEaseType;

        [Tooltip("Constant offset to apply to portrait position.")]
        [SerializeField] protected Vector2 shiftOffset;

        [Tooltip("The position object where characters appear by default.")]
        [SerializeField] protected Image defaultPosition;

        [Tooltip("List of stage position rect transforms in the stage.")]
        [SerializeField] protected List<RectTransform> positions;

        protected List<Character> charactersOnStage = new List<Character>();

        static public List<Stage> activeStages = new List<Stage>();

        protected virtual void OnEnable()
        {
            if (!activeStages.Contains(this))
            {
                activeStages.Add(this);
            }
        }

        protected virtual void OnDisable()
        {
            activeStages.Remove(this);
        }

        protected virtual void Start()
        {
            if (Application.isPlaying &&
                portraitCanvas != null)
            {
                // Ensure the stage canvas is active
                portraitCanvas.gameObject.SetActive(true);
            }
        }

        public static Stage GetActiveStage()
        {
            if (Stage.activeStages == null ||
                Stage.activeStages.Count == 0)
            {
                return null;
            }

            return Stage.activeStages[0];
        }

        #region IStage implementation

        public virtual Canvas PortraitCanvas { get { return portraitCanvas; } }

        public virtual bool DimPortraits { get { return dimPortraits; } set { dimPortraits = value; } }

        public virtual float FadeDuration { get { return fadeDuration; } set { fadeDuration = value; } }

        public virtual float MoveDuration { get { return moveDuration; } set { moveDuration = value; } }

        public virtual LeanTweenType FadeEaseType { get { return fadeEaseType; } }

        public virtual Vector2 ShiftOffset { get { return shiftOffset; } }

        public virtual Image DefaultPosition { get { return defaultPosition; } }

        public virtual List<RectTransform> Positions { get { return positions; } }

        public virtual List<Character> CharactersOnStage { get { return charactersOnStage; } }

        public RectTransform GetPosition(string positionString)
        {
            if (string.IsNullOrEmpty(positionString))
            {
                return null;
            }

            for (int i = 0; i < positions.Count; i++)
            {
                if ( String.Compare(positions[i].name, positionString, true) == 0 )
                {
                    return positions[i];
                }
            }
            return null;
        }

        #endregion
    }
}


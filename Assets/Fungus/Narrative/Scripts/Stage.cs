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
    public class Stage : PortraitController
    {
        [SerializeField] protected Canvas portraitCanvas;
        public Canvas PortraitCanvas { get { return portraitCanvas; } }

        [SerializeField] protected bool dimPortraits;
        public bool DimPortraits { get { return dimPortraits; } set { dimPortraits = value; } }

        [SerializeField] protected float fadeDuration = 0.5f;
        public float FadeDuration { get { return fadeDuration; } set { fadeDuration = value; } }

        [SerializeField] protected float moveDuration = 1f;
        public float MoveDuration { get { return moveDuration; } set { moveDuration = value; } }

        [SerializeField] protected LeanTweenType fadeEaseType;
        public LeanTweenType FadeEaseType { get { return fadeEaseType; } }

        [SerializeField] protected Vector2 shiftOffset;
        public Vector2 ShiftOffset { get { return shiftOffset; } }

        [SerializeField] protected Image defaultPosition;
        public Image DefaultPosition { get { return defaultPosition; } }

        [SerializeField] protected List<RectTransform> positions;
        public List<RectTransform> Positions { get { return positions; } }

        [SerializeField] protected RectTransform[] cachedPositions;

        protected List<Character> charactersOnStage = new List<Character>();
        [SerializeField] public List<Character> CharactersOnStage { get { return charactersOnStage; } }

        static public List<Stage> activeStages = new List<Stage>();

        protected virtual void OnEnable()
        {
            if (!activeStages.Contains(this))
            {
                activeStages.Add(this);
            }
        }

        public void CachePositions()
        {
            cachedPositions = new RectTransform[positions.Count];
            positions.CopyTo(cachedPositions);
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

        /// <summary>
        /// Searches the stage's named positions
        /// If none matches the string provided, give a warning and return a new RectTransform
        /// </summary>
        /// <param name="position_string">Position name to search for</param>
        /// <returns></returns>
        public RectTransform GetPosition(String position_string)
        {
            if (string.IsNullOrEmpty(position_string))
            {
                return null;
            }

            for (int i = 0; i < cachedPositions.Length; i++)
            {
                if ( String.Compare(cachedPositions[i].name, position_string, true) == 0 )
                {
                    return cachedPositions[i];
                }
            }
            return null;
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
    }
}


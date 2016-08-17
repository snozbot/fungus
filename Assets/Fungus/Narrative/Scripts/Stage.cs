/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{

    [ExecuteInEditMode]
    public class Stage : PortraitController
    {
        public Canvas portraitCanvas;
        public bool dimPortraits;
        public float fadeDuration = 0.5f;
        public float moveDuration = 1f;
        public LeanTweenType fadeEaseType;
        public LeanTweenType moveEaseType;
        public Vector2 shiftOffset;
        public Image defaultPosition;
        public List<RectTransform> positions;
        public RectTransform[] cachedPositions;
        public List<Character> charactersOnStage = new List<Character>();

        [HideInInspector]
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


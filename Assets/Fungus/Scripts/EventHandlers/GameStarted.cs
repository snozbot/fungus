// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the game starts playing.
    /// </summary>
    [EventHandlerInfo("",
                      "Game Started",
                      "The block will execute when the game starts playing.")]
    [AddComponentMenu("")]
    public class GameStarted : EventHandler
    {
        [Tooltip("Wait for a number of frames after startup before executing the Block. Can help fix startup order issues.")]
        [SerializeField] protected int waitForFrames = 1;

        [Tooltip("If false, this event will not fire if the game started due to a save being loaded.")]
        [SerializeField] protected bool shouldExecuteDuringSaveLoad = true;

        protected virtual void Start()
        {
            if (!shouldExecuteDuringSaveLoad && FungusManager.Instance.SaveManager.IsSaveLoading)
                return;

            StartCoroutine(GameStartCoroutine());
        }

        protected virtual IEnumerator GameStartCoroutine()
        {
            int frameCount = waitForFrames;
            while (frameCount > 0)
            {
                yield return new WaitForEndOfFrame();
                frameCount--;
            }

            ExecuteBlock();
        }
    }
}

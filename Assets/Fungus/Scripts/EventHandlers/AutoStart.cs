// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// When the object is created, if the game is not presently loading a save game, the block will execute.
    /// </summary>
    [EventHandlerInfo("Flow",
                      "Auto Start",
                      "When the object is created, if the game is not presently loading a save game, the block will execute.")]
    [AddComponentMenu("")]
    public class AutoStart : EventHandler
    {
        [Tooltip("Frames to delay auto start by")]
        [SerializeField] protected int frameDelay = 1;

        protected virtual void Awake()
        {
            if (!FungusManager.Instance.SaveManager.IsSaveLoading)
            {
                StartCoroutine(DelayStart());
            }
        }

        protected virtual IEnumerator DelayStart()
        {
            for (int i = 0; i < frameDelay; i++)
            {
                yield return null;
            }

            ExecuteBlock();
        }
    }
}
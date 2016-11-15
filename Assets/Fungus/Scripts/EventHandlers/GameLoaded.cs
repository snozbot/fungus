// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    [EventHandlerInfo("",
                      "Game Loaded",
                      "Execute this block when a saved game is loaded.")]
    public class GameLoaded : EventHandler 
    {
        [Tooltip("Block will execute if the Save Key of the loaded data matches an entry in the Save Keys list.")]
        [SerializeField] protected List<string> saveKeys = new List<string>();

        #region Public methods

        /// <summary>
        /// Called when a saved game is loaded.
        /// </summary>
        public void OnGameLoaded(string saveKey)
        {
            if (saveKeys.Contains(saveKey))
            {
                ExecuteBlock();
            }
        }

        #endregion
    }
}
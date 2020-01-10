// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    [EventHandlerInfo("Scene",
                      "Save Point Loaded",
                      "Execute this block when a saved point is loaded. Use the 'new_game' key to handle game start.")]
    public class SavePointLoaded : EventHandler 
    {
        [Tooltip("Block will execute if the Save Key of the loaded save point matches this save key.")]
        [SerializeField] protected List<string> savePointKeys = new List<string>();

        protected void OnSavePointLoaded(string _savePointKey)
        {
            for (int i = 0; i < savePointKeys.Count; i++)
            {
                var key = savePointKeys[i];
                if (string.Compare(key, _savePointKey, true) == 0)
                {
                    ExecuteBlock();
                    break;
                }
            }
        }

        #region Public methods

        public static void NotifyEventHandlers(string _savePointKey)
        {
            // Fire any matching SavePointLoaded event handler with matching save key.
            var eventHandlers = Object.FindObjectsOfType<SavePointLoaded>();
            for (int i = 0; i < eventHandlers.Length; i++)
            {
                var eventHandler = eventHandlers[i];
                eventHandler.OnSavePointLoaded(_savePointKey);
            }
        }

        #endregion
    }
}
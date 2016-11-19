// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    [EventHandlerInfo("Scene",
                      "Save Point Loaded",
                      "Execute this block when a saved point is loaded.")]
    public class SavePointLoaded : EventHandler 
    {
        [Tooltip("Block will execute if the Save Key of the loaded save point matches this save key.")]
        [SerializeField] protected string saveKey = "";

        protected void OnSavePointLoaded(string _saveKey)
        {
            if (string.Compare(saveKey, _saveKey, true) == 0)
            {
                ExecuteBlock();
            }
        }

        #region Public methods

        public static void NotifyEventHandlers(string _saveKey)
        {
            // Fire any matching SavePointLoaded event handler with matching save key.
            var eventHandlers = Object.FindObjectsOfType<SavePointLoaded>();
            for (int i = 0; i < eventHandlers.Length; i++)
            {
                var eventHandler = eventHandlers[i];
                eventHandler.OnSavePointLoaded(_saveKey);
            }
        }

        #endregion
    }
}
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Handler for executing blocks based exclusively on save loading, rather than pure progress marker being reached.
    /// </summary>
    [EventHandlerInfo("Scene",
                      "Save Loaded",
                      "Execute blocks when a saved of matching marker name is loaded. If you wish to run regardless of save loading see ProgressMarkerReached.")]
    public class SaveLoaded : EventHandler
    {
        [Tooltip("Block will execute if the Save Key of the loaded save point matches this save key. If empty, will execute on any key.")]
        [UnityEngine.Serialization.FormerlySerializedAs("savePointKeys")]
        [SerializeField] protected List<string> progressMarkerCustomKeys = new List<string>();

        protected void OnSavePointLoaded(string _savePointKey)
        {
            for (int i = 0; i < progressMarkerCustomKeys.Count; i++)
            {
                var key = progressMarkerCustomKeys[i];
                if (string.Compare(key, _savePointKey, true) == 0)
                {
                    ExecuteBlock();
                    break;
                }
            }

            //empty collection means go on any key
            if (progressMarkerCustomKeys.Count == 0)
            {
                ExecuteBlock();
            }
        }

        public static void NotifyEventHandlers(string progressMarkerName)
        {
            // Fire any matching SavePointLoaded event handler with matching save key.
            var eventHandlers = Object.FindObjectsOfType<SaveLoaded>();
            for (int i = 0; i < eventHandlers.Length; i++)
            {
                var eventHandler = eventHandlers[i];
                eventHandler.OnSavePointLoaded(progressMarkerName);
            }
        }
    }
}

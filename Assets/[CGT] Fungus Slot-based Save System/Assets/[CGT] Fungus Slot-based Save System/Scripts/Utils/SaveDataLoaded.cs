using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

namespace CGTUnity.Fungus.SaveSystem
{
    [EventHandlerInfo("Scene",
                        "Save Data Loaded",
                        "Execute this block when Game Save Data is loaded.")]
    public class SaveDataLoaded : EventHandler
    {
        [SerializeField] protected string[] ProgressMarkerKeys;

        protected virtual void OnSaveDataLoaded(string ProgressMarkerKey)
        {
            // If this has no keys, it acts as if it has the right one.
            if (ProgressMarkerKeys.Length == 0)
            {
                ExecuteBlock();
                return;
            }

            for (int i = 0; i < ProgressMarkerKeys.Length; i++)
            {
                if (string.Compare(ProgressMarkerKeys[i], ProgressMarkerKey) == 0)
                {
                    ExecuteBlock();
                    break;
                }
            }
        }

        public static void NotifyEventHandlers(string _savePointKey)
        {
            // Fire any matching SavePointLoaded event handler with matching save key.
            var eventHandlers =             FindObjectsOfType<SaveDataLoaded>();
            for (int i = 0; i < eventHandlers.Length; i++)
            {
                var eventHandler =          eventHandlers[i];
                eventHandler.OnSaveDataLoaded(_savePointKey);
            }

        }
    }
}
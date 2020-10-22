// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when a Progress Marker with a name from set the is reached.
    /// </summary>
    [EventHandlerInfo("Flow",
                      "Progress Marker Reached",
                      "The block will execute when a Progress Marker of the same name is reached or set. If you only want to run when loading a save see SaveLoaded.")]
    [AddComponentMenu("")]
    public class ProgressMarkerReached : EventHandler
    {
        [Tooltip("Block will execute of any match. If empty, will execute on any key.")]
        [SerializeField] protected List<string> progressMarkerCustomKeys = new List<string>();

        protected virtual void Awake()
        {
            SaveManagerSignals.OnProgressMarkerReached += SaveManagerSignals_OnProgressMarkerReached;
        }

        protected virtual void OnDestroy()
        {
            SaveManagerSignals.OnProgressMarkerReached -= SaveManagerSignals_OnProgressMarkerReached;
        }

        protected virtual void SaveManagerSignals_OnProgressMarkerReached(ProgressMarker was, ProgressMarker now)
        {
            if (enabled)
            {
                OnProgressMarkerReached(now.CustomKey);
            }
        }

        public virtual void OnProgressMarkerReached(string progressMarkerKey)
        {
            for (int i = 0; i < progressMarkerCustomKeys.Count; i++)
            {
                var key = progressMarkerCustomKeys[i];
                if (string.Compare(key, progressMarkerKey, true) == 0)
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
    }
}

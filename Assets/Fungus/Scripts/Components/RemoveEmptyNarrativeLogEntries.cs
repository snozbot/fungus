using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    public class RemoveEmptyNarrativeLogEntries : MonoBehaviour
    {
        protected NarrativeLogEntryUI logUI;
        protected NarrativeLog narrLog;

        protected virtual void Awake()
        {
            logUI = GetComponent<NarrativeLogEntryUI>();
        }

        protected virtual void Start()
        {
            var manager = FungusManager.Instance;
            narrLog = manager.NarrativeLog;
        }

        protected virtual void OnEnable()
        {
            ListenForNarrativeAdding();
        }

        protected virtual void ListenForNarrativeAdding()
        {
            logUI.OnEntryAdded += OnEntryAdded;
        }

        protected virtual void OnEntryAdded(NarrativeLogEntryDisplay entry)
        {
            bool emptyEntry = string.IsNullOrEmpty(entry.ToDisplay.text);

            if (emptyEntry)
            {
                // Get the Narrative Log to unregister the entry's metadata. That should get the UI to
                // remove the display that had it.
                narrLog.RemoveLine(entry.ToDisplay);
            }
        }

        protected virtual void OnDisable()
        {
            UnlistenForEntryAdding();
        }

        protected virtual void UnlistenForEntryAdding()
        {
            logUI.OnEntryAdded -= OnEntryAdded;
        }
    }
}
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;
using NarrativeHandler = Fungus.NarrativeLog.NarrativeHandler;

namespace Fungus
{
    /// <summary>
    /// Manages the UI aspects of the Narrative Log via EntryDisplay objects
    ///
    /// Originally contributed by https://github.com/CG-Tespy
    /// </summary>
    public class NarrativeLogEntryUI : MonoBehaviour
    {
        [Tooltip("Contains the overall aesthetic of each entry.")]
        [SerializeField] protected NarrativeLogEntryDisplay entryDisplayPrefab;

        [SerializeField] protected RectTransform entryHolder;
        protected CanvasGroup canvasGroup;
        protected List<NarrativeLogEntryDisplay> entryDisplays = new List<NarrativeLogEntryDisplay>();
        protected UnityEngine.UI.ScrollRect scrollRect;

        public delegate void EntryDisplayHandler(NarrativeLogEntryDisplay entryDisplay);

        /// <summary>
        /// For when an entry has been registered in the UI.
        /// </summary>
        public event EntryDisplayHandler OnEntryAdded = delegate { };

        /// <summary>
        /// For when an entry has been removed from the UI.
        /// </summary>
        public event EntryDisplayHandler OnEntryRemoved = delegate { };

        protected virtual void Awake()
        {
            ListenForNarrativeEvents();
            canvasGroup = GetComponent<CanvasGroup>();
            scrollRect = GetComponentInChildren<UnityEngine.UI.ScrollRect>();
        }

        protected virtual void ListenForNarrativeEvents()
        {
            var manager = FungusManager.Instance;

            if (manager != null)
            {
                var narrLog = manager.NarrativeLog;
                // Make sure to update the UI when new entries are added to or
                // cleared from the log.
                //using the fungusmanager to ensure that the narrativeLog is inited
                narrLog.OnNarrativeAdded += OnNarrativeAdded;
                narrLog.OnNarrativeRemoved += OnNarrativeRemoved;
                narrLog.OnNarrativeLogClear += Clear;
            }
        }


        public virtual void Clear()
        {
            for (int i = 0; i < entryDisplays.Count; i++)
            {
                var entryDisplay = entryDisplays[i];
                Destroy(entryDisplay.gameObject);
            }

            entryDisplays.Clear();
        }

        public virtual void Open()
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        public virtual void Close()
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        protected virtual void OnNarrativeAdded(NarrativeLogEntry entryAdded)
        {
            var newEntryDisplay = AddEntryToUI(entryAdded);
            AlertListenersForEntryAdding(newEntryDisplay);
            StartCoroutine(ForceToBottom());
        }

        protected virtual NarrativeLogEntryDisplay AddEntryToUI(NarrativeLogEntry entry)
        {
            var newEntryDisplay = Instantiate(entryDisplayPrefab);
            newEntryDisplay.transform.SetParent(entryHolder, false);
            newEntryDisplay.ToDisplay = entry;
            entryDisplays.Add(newEntryDisplay);
            return newEntryDisplay;
        }

        protected virtual void AlertListenersForEntryAdding(NarrativeLogEntryDisplay display)
        {
            OnEntryAdded(display);
        }

        private System.Collections.IEnumerator ForceToBottom()
        {
            yield return null;
            scrollRect.verticalNormalizedPosition = 0;
        }

        protected virtual void OnNarrativeRemoved(NarrativeLogEntry entryRemoved)
        {
            // Based on the entry's index, make sure we remove exactly the right entry display from the log.
            int index = entryRemoved.index;

            var toRemove = entryHolder.GetChild(index);
            toRemove.gameObject.SetActive(false); // Should make the entry disappear from the log
            
            var entryDisplayScript = toRemove.GetComponent<NarrativeLogEntryDisplay>();
            entryDisplays.Remove(entryDisplayScript);
            AlertListenersForEntryRemoval(entryDisplayScript);


            Destroy(toRemove.gameObject);
        }

        protected virtual void AlertListenersForEntryRemoval(NarrativeLogEntryDisplay entry)
        {
            OnEntryRemoved(entry);
        }

        protected virtual void OnLogCleared()
        {
            Clear();
        }

        protected virtual void OnDestroy()
        {
            var fManInst = FungusManager.Instance;
            // Avoid this responding to signals when being destroyed.
            if (fManInst != null)
            {
                FungusManager.Instance.NarrativeLog.OnNarrativeAdded -= OnNarrativeAdded;
                FungusManager.Instance.NarrativeLog.OnNarrativeLogClear -= Clear;
            }
        }

        protected virtual void UnlistenForNarrativeEvents()
        {
            var manager = FungusManager.Instance;

            if (manager != null)
            {
                var narrLog = manager.NarrativeLog;
                narrLog.OnNarrativeAdded -= OnNarrativeAdded;
                narrLog.OnNarrativeRemoved -= OnNarrativeRemoved;
                narrLog.OnNarrativeLogClear -= Clear;
            }
        }
    }
}
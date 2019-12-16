using System.Collections.Generic;
using UnityEngine;

using EntryDisplay =                                        CGTUnity.Fungus.NarrativeLogSystem.NarrativeLogEntryDisplay;

namespace CGTUnity.Fungus.NarrativeLogSystem
{
    /// <summary>
    /// Manages the UI aspects of the Narrative Log.
    /// </summary>
    public class NarrativeLogUI : MonoBehaviour
    {
        #region Fields
        [Tooltip("Contains the entries this UI is there to display.")]
        [SerializeField] protected NarrativeLog narrativeLog;
        [Tooltip("Contains the overall aesthetic of each entry.")]
        [SerializeField] protected EntryDisplay entryDisplayPrefab;
        [SerializeField] protected RectTransform entryHolder;
        protected CanvasGroup canvasGroup;
        protected List<EntryDisplay> entryDisplays =        new List<EntryDisplay>();
        #endregion

        #region Methods

        protected virtual void Awake()
        {
            canvasGroup =                                   GetComponent<CanvasGroup>();
            // Make sure to update the UI when new entries are added to or 
            // cleared from the log.
            Signals.NarrativeAdded +=    OnNarrativeAdded;
            Signals.LogCleared +=        OnLogCleared;
        }

        public virtual void SetLogEntries(IList<Entry> entries)
        {
            // Out with the old, and in with the new!
            narrativeLog.Clear(); // Note the display entries go boom when the log is cleared.
            narrativeLog.AddEntries(entries);
        }

        public virtual void Clear()
        {
            for (int i = 0; i < entryDisplays.Count; i++)
            {
                var entryDisplay =                      entryDisplays[i];
                Destroy(entryDisplay.gameObject);
            }

            entryDisplays.Clear();
        }

        public virtual void Open()
        {
            canvasGroup.alpha =                         1;
            canvasGroup.interactable =                  true;
            canvasGroup.blocksRaycasts =                true;
        }

        public virtual void Close()
        {
            canvasGroup.alpha =                         0;
            canvasGroup.interactable =                  false;
            canvasGroup.blocksRaycasts =                false;
        }

        #region For event-listening

        protected virtual void OnNarrativeAdded(Entry entryAdded)
        {
            // Create a display for the new entry, and have it show in the UI. 
                
            var newEntryDisplay =                           Instantiate<EntryDisplay>(entryDisplayPrefab);
            newEntryDisplay.transform.SetParent(entryHolder, false);
            newEntryDisplay.ToDisplay =                     entryAdded;
            entryDisplays.Add(newEntryDisplay);
        }

        protected virtual void OnLogCleared()
        {
            this.Clear();
        }

        protected virtual void OnDestroy()
        {
            // Avoid this responding to signals when being destroyed.
            Signals.NarrativeAdded -=    OnNarrativeAdded;
            Signals.LogCleared -=        OnLogCleared;
        }

        #endregion
        #endregion
    }
}
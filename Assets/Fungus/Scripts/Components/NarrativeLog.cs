// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// A single line of dialog
    /// </summary>
    [System.Serializable]
    public class NarrativeLogEntry
    {
        [SerializeField] public string name;
        [SerializeField] public string text;
        [SerializeField] public int index = -1; 
        // The index is to keep track of where in the log this entry is to be displayed. A negative index
        // means it's not yet even been added to the log.
    }

    /// <summary>
    /// Serializable object to store Narrative Lines
    /// </summary>
    [System.Serializable]
    public class NarrativeData
    {
        public List<NarrativeLogEntry> entries = new List<NarrativeLogEntry>();
    }

    /// <summary>
    /// Controls dialog history
    /// </summary>
    public class NarrativeLog : MonoBehaviour
    {
        /// <summary>
        /// NarrativeAdded signal. Sent when a line is added.
        /// </summary>
        public event NarrativeAddedHandler OnNarrativeAdded = delegate { }; 
        public event NarrativeHandler OnNarrativeRemoved = delegate { };
        // ^ Makes sure these events are never null. Less necessary null checks = better performance ^_^

        public delegate void NarrativeHandler(NarrativeLogEntry data);
        public delegate void NarrativeAddedHandler(NarrativeLogEntry data);

        public void DoNarrativeAdded(NarrativeLogEntry data)
        {
            if (OnNarrativeAdded != null)
            {
                OnNarrativeAdded(data);
            }
        }

        public void DoNarrativeRemoved(NarrativeLogEntry data)
        {
            OnNarrativeRemoved(data);
        }

        /// <summary>
        /// Signal sent when log history is cleared or loaded
        /// </summary>
        public System.Action OnNarrativeLogClear;
        public void DoNarrativeCleared()
        {
            if (OnNarrativeLogClear != null)
            {
                OnNarrativeLogClear();
            }
        }

        NarrativeData history;

        protected virtual void Awake()
        {
            history = new NarrativeData();
            DoNarrativeCleared();
        }

        protected virtual void OnEnable()
        {
            ListenForNarrativeEvents();
            WriterSignals.OnWriterState += OnWriterState;
        }

        protected virtual void ListenForNarrativeEvents()
        {
            OnNarrativeRemoved += RespondToNarrativeRemoval;
            OnNarrativeAdded += RespondToNarrativeAdding;
        }

        protected virtual void RespondToNarrativeRemoval(NarrativeLogEntry entryRemoved)
        {
            // We may want to have this class do other responses to narrative-removal in the
            // future, so it's best to have each response in its own func.
            UpdateEntryIndexesAfterRemoval(entryRemoved);
        }

        protected virtual void UpdateEntryIndexesAfterRemoval(NarrativeLogEntry entryRemoved)
        {
            // The entry removed might not be the last one added, after all.
            // We'll need to update the indexes of all the entries that followed
            // the removed one.
            for (int i = entryRemoved.index; i < history.entries.Count; i++)
            {
                var entry = history.entries[i];
                entry.index = i;
            }
        }

        protected virtual void RespondToNarrativeAdding(NarrativeLogEntry entryAdded)
        {
            // Future-proofing.
        }

        protected virtual void OnDisable()
        {
            UnlistenForNarrativeEvents();
            WriterSignals.OnWriterState -= OnWriterState;
        }

        protected virtual void UnlistenForNarrativeEvents()
        {
            OnNarrativeRemoved -= RespondToNarrativeRemoval;
            OnNarrativeAdded -= RespondToNarrativeAdding;
        }

        protected virtual void OnWriterState(Writer writer, WriterState writerState)
        {
            if (writerState == WriterState.End)
            {
                var sd = SayDialog.GetSayDialog();

                if (sd != null)
                {
                    NarrativeLogEntry entry = new NarrativeLogEntry()
                    {
                        name = sd.NameText,
                        text = sd.StoryText
                    };
                    AddLine(entry);
                }
            }
        }


        #region Public Methods

        /// <summary>
        /// Add a line of dialog to the Narrative Log
        /// </summary>
        public void AddLine(NarrativeLogEntry entry)
        {
            entry.index = history.entries.Count;
            history.entries.Add(entry);
            DoNarrativeAdded(entry);
        }

        /// <summary>
        /// Removes a line of dialog from the Narrative Log. Returns whether or not
        /// the line was in the log at the time this func was called.
        /// </summary>
        public bool RemoveLine(NarrativeLogEntry entry)
        {
            bool hadLineToBeginWith = history.entries.Remove(entry);

            if (hadLineToBeginWith)
            {
                DoNarrativeRemoved(entry);
            }

            return hadLineToBeginWith;
        }

        /// <summary>
        /// Clear all lines of the  narrative log
        /// Usually used on restart
        /// </summary>
        public void Clear()
        {
            history.entries.Clear();

            DoNarrativeCleared();
        }

        /// <summary>
        /// Convert history into Json for saving in SaveData
        /// </summary>
        /// <returns></returns>
        public string GetJsonHistory()
        {
            string jsonText = JsonUtility.ToJson(history, true);
            return jsonText;
        }

        /// <summary>
        /// Show previous lines for display purposes
        /// </summary>
        /// <returns></returns>
        public string GetPrettyHistory(bool previousOnly = false)
        {
            string output = "\n ";
            int count;

            count = previousOnly ? history.entries.Count - 1 : history.entries.Count;

            for (int i = 0; i < count; i++)
            {
                output += "<b>" + history.entries[i].name + "</b>\n";
                output += history.entries[i].text + "\n\n";
            }
            return output;
        }

        /// <summary>
        /// Load History from Json
        /// </summary>
        /// <param name="narrativeData"></param>
        public void LoadHistory(string narrativeData)
        {
            if (narrativeData == null)
            {
                Debug.LogError("Failed to decode History save data item");
                return;
            }
            history = JsonUtility.FromJson<NarrativeData>(narrativeData);

            DoNarrativeCleared();
        }
        #endregion
    }
}
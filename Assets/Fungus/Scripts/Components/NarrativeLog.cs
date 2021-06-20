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
        public static event NarrativeAddedHandler OnNarrativeAdded;
        public delegate void NarrativeAddedHandler(NarrativeLogEntry data);
        public static void DoNarrativeAdded(NarrativeLogEntry data)
        {
            if (OnNarrativeAdded != null)
            {
                OnNarrativeAdded(data);
            }
        }

        /// <summary>
        /// Signal sent when log history is cleared or loaded
        /// </summary>
        public static System.Action OnNarrativeLogClear;
        public static void DoNarrativeCleared()
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
            WriterSignals.OnWriterState += OnWriterState;
        }

        protected virtual void OnDisable()
        {
            WriterSignals.OnWriterState -= OnWriterState;
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
            history.entries.Add(entry);
            DoNarrativeAdded(entry);
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
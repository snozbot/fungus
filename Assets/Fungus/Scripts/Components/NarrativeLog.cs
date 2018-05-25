// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// A single line of dialog
    /// </summary>
    [Serializable]
    public class Line
    {
        [SerializeField] public string name;
        [SerializeField] public string text;

    }

    /// <summary>
    /// Serializable object to store Narrative Lines
    /// </summary>
    [Serializable]
    public class NarrativeData
    {
        [SerializeField] public List<Line> lines;

        public NarrativeData() {
            lines = new List<Line>();
        }
        
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
        public delegate void NarrativeAddedHandler();
        public static void DoNarrativeAdded() { if (OnNarrativeAdded != null) OnNarrativeAdded(); }

        NarrativeData history;

        protected virtual void Awake()
        {
            history = new NarrativeData();
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
                var from = sd.NameText != null ? sd.NameText.text : string.Empty;
                var line = sd.StoryText != null ? sd.StoryText.text : string.Empty;

                AddLine(from, line);
            }
        }

        #region Public Methods
        
        /// <summary>
        /// Add a line of dialog to the Narrative Log
        /// </summary>
        /// <param name="name">Character Name</param>
        /// <param name="text">Narrative Text</param>
        public void AddLine(string name, string text)
        {
            Line line = new Line();
            line.name = name;
            line.text = text;
            history.lines.Add(line);
            DoNarrativeAdded();
        }

        /// <summary>
        /// Clear all lines of the  narrative log
        /// Usually used on restart
        /// </summary>
        public void Clear()
        {
            history.lines.Clear();
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

            count = previousOnly ? history.lines.Count - 1: history.lines.Count;

            for (int i = 0; i < count; i++)
            {
                output += "<b>" + history.lines[i].name + "</b>\n";
                output += history.lines[i].text + "\n\n";
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
        }
        #endregion
    }
}
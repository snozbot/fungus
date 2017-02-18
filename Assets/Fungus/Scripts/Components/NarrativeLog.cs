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

        NarrativeData history;

        protected virtual void Awake()
        {
            history = new NarrativeData();
        }

        protected virtual void OnEnable()
        {
            SaveManagerSignals.OnSaveReset += OnSaveReset;
        }

        protected virtual void OnDisable()
        {
            SaveManagerSignals.OnSaveReset -= OnSaveReset;
        }

        protected virtual void OnSaveReset()
        {
            Clear();
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
        public string GetPrettyHistory()
        {
            string output = "";

            for (int i = 0; i < history.lines.Count; i++)
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
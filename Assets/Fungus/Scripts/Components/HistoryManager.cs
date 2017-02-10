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
        public string name;
        public string text;

    }


    [Serializable]
    public class HistoryData
    {
        public List<Line> lines;

        public HistoryData() {
            lines = new List<Line>();
        }
    }

    /// <summary>
    /// Controls dialog history
    /// </summary>
    public class HistoryManager : MonoBehaviour
    {

        HistoryData history;

        protected virtual void Awake()
        {
            history = new HistoryData();
        }

        public void AddLine(string name, string text)
        {
            Line line = new Line();
            line.name = name;
            line.text = text;
            history.lines.Add(line);
        }

        public string GetHistory()
        {
            string jsonText = JsonUtility.ToJson(history, true);
            return jsonText;
        }

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
    }
}
using System.Collections.Generic;
using UnityEngine;
using CGTUnity.Fungus.NarrativeLogSystem;

namespace CGTUnity.Fungus.SaveSystem
{

    /// <summary>
    /// Save data to contain the state of a single NarrativeLog.
    /// </summary>
    [System.Serializable]
    public class NarrativeLogData : SaveData
    {
        [SerializeField] List<Entry> entries =      new List<Entry>();

        public virtual List<Entry> Entries          
        { 
            get                                     { return entries; } 
            protected set                           { entries = value; } 
        }

        public NarrativeLogData() { }

        public NarrativeLogData(ICollection<Entry> entries) : base()
        {
            this.entries.AddRange(entries);
        }

    }
}
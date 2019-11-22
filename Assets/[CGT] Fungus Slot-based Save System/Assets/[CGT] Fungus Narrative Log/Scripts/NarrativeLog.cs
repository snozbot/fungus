using System.Collections.Generic;
using UnityEngine;
using Fungus;

namespace CGTUnity.Fungus.NarrativeLogSystem
{
    /// <summary>
    /// Keeps track of the text in Say Dialogs up to any given point in a game.
    /// </summary>
    public class NarrativeLog : MonoBehaviour
    {
        public virtual List<Entry> Entries          { get; protected set; }

        #region Methods
        protected virtual void Awake()
        {
            Entries =                               new List<Entry>();
        }

        public virtual void AddNewEntry()
        {
            // Try finding the say Dialog in the scene 
            var sayDialog =                         SayDialog.ActiveSayDialog;

            if (sayDialog == null) // This is no time to be logging any narrative
                return;

            // Get the story and name texts, apply them to a new entry to register
            var newEntry =                          new Entry(sayDialog.StoryText, sayDialog.NameText);
            AddNewEntry(newEntry);

        }

        public virtual void AddNewEntry(Entry newEntry)
        {
            Entries.Add(newEntry);
            Signals.NarrativeAdded.Invoke(newEntry);
        }

        public virtual void AddEntries(IList<Entry> toAdd)
        {
            // Make sure listeners are alerted for each element added
            for (int i = 0; i < toAdd.Count; i++) 
                AddNewEntry(toAdd[i]);
        }

        protected virtual void OnWriterStateChange(Writer writer, WriterState writerState)
        {
            // Add a new boxful when it's done being typed out.
            if (writerState == WriterState.End)
            {
                var sayDialog =                     SayDialog.GetSayDialog();
                var newEntry =                      new Entry(sayDialog.StoryText, sayDialog.NameText);
                AddNewEntry(newEntry);
            }
        }

        public virtual void Clear()
        {
            Entries.Clear();
            Signals.LogCleared.Invoke();
        }

        protected virtual void OnEnable()
        {
            WriterSignals.OnWriterState +=                  OnWriterStateChange;
        }

        protected virtual void OnDisable()
        {
            WriterSignals.OnWriterState -=                  OnWriterStateChange;
        }

        #endregion

    }

    
}
using System.Collections.Generic;
using UnityEngine;
using CGTUnity.Fungus.NarrativeLogSystem;
using NarrativeSignals =                                CGTUnity.Fungus.NarrativeLogSystem.Signals;

namespace CGTUnity.Fungus.SaveSystem
{
    public class NarrativeLogSaver : DataSaver<NarrativeLogData>, ISaveCreator<NarrativeLogData>
    {
        // The algorithm used here keeps an instance of NarrativeLogData updated as the NarrativeLog
        // is itself updated. This way, when this is called to "create" the save data, it can return
        // it much faster, using less resources per frame than it would if it were to just create 
        // the data on-demand.
        [SerializeField] protected NarrativeLog toSave;
        protected NarrativeLogData saveData =           new NarrativeLogData();

        #region Methods

        #region MonoBehaviour Standard
        protected virtual void Awake()
        {
            NarrativeSignals.LogCleared +=              OnNarrativeLogCleared;
            NarrativeSignals.NarrativeAdded +=          OnNarrativeAdded;
        }

        protected virtual void OnDestroy()
        {
            NarrativeSignals.LogCleared -=              OnNarrativeLogCleared;
            NarrativeSignals.NarrativeAdded -=          OnNarrativeAdded;
        }
        #endregion

        #region Data-creation
        public virtual NarrativeLogData CreateSave()
        {
            // The data's ready immediately, having been updated as appropriate by the event listeners.
            return saveData;
        }

        public override IList<SaveDataItem> CreateItems()
        {
            // At the time of this writing, the NarrativeLog is a singleton, so we're setting up a
            // one-element array.
            var jsonData =                              JsonUtility.ToJson(saveData);
            var singleItem =                            new SaveDataItem(saveType.Name, jsonData);
            var result =                                new SaveDataItem[1] {singleItem};

            return result;
        }
        #endregion

        #region Event Listeners
        protected virtual void OnNarrativeLogCleared()
        {
            saveData.Entries.Clear();
        }

        protected virtual void OnNarrativeAdded(Entry newEntry)
        {
            saveData.Entries.Add(newEntry);
        }

        #endregion

        #endregion
    }
}
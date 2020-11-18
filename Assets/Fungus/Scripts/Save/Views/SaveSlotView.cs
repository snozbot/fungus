using UnityEngine;

namespace Fungus
{
    public interface ISaveSlotView
    {
        SaveGameMetaData SaveData { get; set; }
        void Refresh();
    }

    /// <summary>
    /// For components meant to display something based on save data.
    /// </summary>
    public abstract class SaveSlotView : MonoBehaviour, ISaveSlotView
    {
        public SaveGameMetaData SaveData
        {
            get { return saveData; }
            set
            {
                saveData = value;
                Refresh();
            }
        }

        private SaveGameMetaData saveData;

        public abstract void Refresh();

        protected virtual bool HasSaveData
        {
            get { return saveData != null; }
        }
    }
}
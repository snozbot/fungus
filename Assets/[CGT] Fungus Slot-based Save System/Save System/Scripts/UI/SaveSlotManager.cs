using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Fungus.SaveSystem
{
    /// <summary>
    /// Manages and is used to interact with the slots in the Save System's UI.
    /// </summary>
    public class SaveSlotManager : MonoBehaviour
    {
        // This class updates its slots mainly by reacting to saves being written, read, or erased. It
        // updates some other parts of its state based on those actions, as well.
        // See this class's event listeners.
        [SerializeField] protected RectTransform slotHolder;
        protected List<SaveSlot> slots =                new List<SaveSlot>();

        public virtual SaveSlot selectedSlot            { get; protected set; }

        #region Methods

        #region Monobehaviour Standard
        protected virtual void Awake()
        {
            if (slotHolder == null)
                throw new System.MissingFieldException(this.name + " needs a slot holder!");

            slots.AddRange(slotHolder.GetComponentsInChildren<SaveSlot>());
            ListenForEvents();
        }

        protected virtual void OnDestroy()
        {
            UnlistenForEvents();
        }

        #endregion

        #region Retrieving Slots
        /// <summary>
        /// Returns a slot this keeps track of with the passed slot number if possible.
        /// Null otherwise.
        /// </summary>
        public virtual SaveSlot FindSlot(int slotNumber)
        {
            for (int i = 0; i < slots.Count; i++)
                if (slots[i].Number == slotNumber)
                    return slots[i];
            
            return null;
        }

        /// <summary>
        /// Returns the slot that was assigned the passed save data if possible.
        /// Null otherwise.
        /// </summary>
        public virtual SaveSlot FindSlot(GameSaveData saveData)
        {
            ValidateSaveData(saveData);

            for (int i = 0; i < slots.Count; i++)
                if (slots[i].SaveData == saveData)
                    return slots[i];

            return null;
        }
        #endregion

        #region Altering Slots
        /// <summary>
        /// Assigns the passed saves to the appropriate slots.
        /// </summary>
        public virtual void SetSlotsWith(IList<GameSaveData> saves, bool clearSlotDataFirst = true)
        {
            if (saves == null)
                throw new System.NullReferenceException("Cannot set slots with a null list of saves.");

            if (clearSlotDataFirst)
                for (int i = 0; i < slots.Count; i++)
                    slots[i].Clear();
                
            for (int i = 0; i < saves.Count; i++)
            {
                var saveData =                      saves[i];
                SetSlotWith(saveData);
            }
        }

        /// <summary>
        /// Assigns the passed save data to the slot sharing a number with it.
        /// </summary>
        public virtual void SetSlotWith(GameSaveData saveData)
        {
            ValidateSaveData(saveData);

            var slot =                              FindSlot(saveData.SlotNumber);
            if (slot != null)
                slot.SaveData =                     saveData;
        }

        /// <summary>
        /// Assigns the passed save data to the passed slot, changing the former's slot 
        /// number if necessary.
        /// </summary>
        public virtual void ForceSetSlotWith(GameSaveData saveData, SaveSlot slot)
        {
            ValidateSaveData(saveData);

            if (slot == null)
                throw new System.ArgumentException("Cannot set save data to a null slot.");

            saveData.SlotNumber =                   slot.Number;
            slot.SaveData =                         saveData;
        }

        public virtual void ClearSlot(int slotNumber)
        {
            var slot =                              FindSlot(slotNumber);

            if (slot != null)
                slot.Clear();
        }

        #endregion

        #region Event listeners
        
        /// <summary>
        /// Assigns the newly-written save data to the appropriate slot, since the slots should
        /// only display already-written save data.
        /// </summary>
        protected virtual void OnGameSaveWritten(GameSaveData written, string filePath, string fileName)
        {
            AssignSave(written);
        }   

        protected virtual void OnGameSaveRead(GameSaveData readFromDisk, string filePath, string fileName)
        {
            AssignSave(readFromDisk);
        }

        protected virtual void OnGameSaveErased(GameSaveData deletedFromDisk, string filePath, string fileName)
        {
            // Clear the slot that was holding the data.
            var slot =                                  FindSlot(deletedFromDisk);
            slot.Clear();
        }

        protected virtual void OnSaveSlotClicked(SaveSlot slotClicked)
        {
            selectedSlot =                      slotClicked;
        }

        #endregion

        public virtual void SelectSlot(int indexNumber)
        {
            if (indexNumber < 0 || indexNumber >= slots.Count)
                return;

            selectedSlot =                              slots[indexNumber];
            EventSystem.current.SetSelectedGameObject(selectedSlot.gameObject);
        }
        #region Helpers

        /// <summary>
        /// Checks if the save data passed is valid. Returns true if it's valid, false otherwise.
        /// Can throw an exception instead of returning false.
        /// </summary>
        protected virtual bool ValidateSaveData(GameSaveData saveData, bool throwExceptionOnFalse = true)
        {
            var valid =                         saveData != null;

            if (!valid && throwExceptionOnFalse)
                throw new System.NullReferenceException(this.name + " cannot work with null save data.");

            return valid;
        }

        protected virtual void ListenForEvents()
        {
            Signals.GameSaveWritten +=        OnGameSaveWritten;
            Signals.GameSaveRead +=           OnGameSaveRead;
            Signals.GameSaveErased +=         OnGameSaveErased;
            Signals.SaveSlotClicked +=        OnSaveSlotClicked;
        }

        protected virtual void UnlistenForEvents()
        {
            Signals.GameSaveWritten -=        OnGameSaveWritten;
            Signals.GameSaveRead -=           OnGameSaveRead;
            Signals.GameSaveErased -=         OnGameSaveErased;
            Signals.SaveSlotClicked -=        OnSaveSlotClicked;
        }

        protected virtual void AssignSave(GameSaveData saveData)
        {
            for (int i = 0; i < slots.Count; i++)
                if (slots[i].Number == saveData.SlotNumber)
                    slots[i].SaveData =                 saveData;
        }
        #endregion
        #endregion

    }
}
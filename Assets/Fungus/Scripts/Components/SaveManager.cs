using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fungus
{
    public class SaveManager : MonoBehaviour 
    {
        const string ActiveSlotKey = "active_slot";

        const string SlotKeyFormat = "slot{0}";

        protected string saveBuffer = "";

        protected virtual string FormatSaveKey(int slot)
        {
            return string.Format(SlotKeyFormat, slot);
        }

        protected virtual void StoreJSONData(string key, string jsonData)
        {
            if (key.Length > 0)
            {
                PlayerPrefs.SetString(key, jsonData);
            }
        }

        protected virtual string LoadJSONData(string key)
        {
            if (key.Length > 0)
            {
                return PlayerPrefs.GetString(key);
            }

            return "";
        }

        #region Public members

        public virtual int ActiveSlot
        {
            get
            {
                return PlayerPrefs.GetInt(ActiveSlotKey);
            }
            set
            {
                PlayerPrefs.SetInt(ActiveSlotKey, value);
            }
        }

        public virtual void Save()
        {
            if (saveBuffer == "")
            {
                // Nothing to save
                return;
            }

            var key = FormatSaveKey(ActiveSlot);

            PlayerPrefs.SetString(key, saveBuffer);

            saveBuffer = "";
        }

        public virtual bool SlotExists(int slot)
        {
            var key = FormatSaveKey(slot);

            if (PlayerPrefs.HasKey(key) &&
                PlayerPrefs.GetString(key) != "")
            {
                return false;
            }

            return true;
        }

        public virtual void Load(int slot)
        {
            ActiveSlot = slot;

            var key = FormatSaveKey(slot);

            var saveDataJSON = LoadJSONData(key);
            if (saveDataJSON != "")
            {
                SavePointData.Decode(saveDataJSON);
            }
        }

        public virtual void LoadNewGame(int slot, string saveDescription)
        {
            var key = FormatSaveKey(slot);

            var saveDataJSON = SavePointData.EncodeNewGame(saveDescription, SceneManager.GetActiveScene().name);

            // Create a new save entry
            PlayerPrefs.SetString(key, saveDataJSON);

            SavePointLoaded.NotifyEventHandlers("new_game");
        }

        public virtual void Delete(int slot)
        {
            var key = FormatSaveKey(slot);
            PlayerPrefs.DeleteKey(key);
        }

        public virtual void PopulateSaveBuffer(string saveKey, string description)
        {
            saveBuffer = SavePointData.Encode(saveKey, description, SceneManager.GetActiveScene().name);
            Debug.Log(saveBuffer);
        }

        #endregion
    }
}
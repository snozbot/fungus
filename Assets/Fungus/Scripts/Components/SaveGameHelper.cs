using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Fungus
{
    public class SaveGameHelper : MonoBehaviour 
    {
        const string NewGameSaveKey = "new_game";

        [SerializeField] protected string startScene = "";

        [SerializeField] protected bool autoStartGame = true;

        [SerializeField] protected bool restartDeletesSave = false;

        [SerializeField] protected SaveGameObjects saveGameObjects = new SaveGameObjects();

        protected virtual void OnEnable()
        {
            SaveSignals.OnGameSave += OnGameSave;
        }

        protected virtual void OnDisable()
        {
            SaveSignals.OnGameSave -= OnGameSave;
        }

        protected virtual void Start()
        {
            var saveManager = FungusManager.Instance.SaveManager;

            if (autoStartGame &&
                saveManager.NumSavePoints == 0)
            {
                SavePointLoaded.NotifyEventHandlers(NewGameSaveKey);
            }
        }

        protected virtual void OnGameSave(string saveKey, string saveDescription)
        {
        }

        #region Public methods

        public SaveGameObjects SaveGameObjects { get { return saveGameObjects; } }

        public virtual void Save()
        {
            var saveManager = FungusManager.Instance.SaveManager;
            saveManager.Save();
        }

        public virtual void Load()
        {
            var saveManager = FungusManager.Instance.SaveManager;
            saveManager.Load();
        }

        public virtual void Rewind()
        {
            var saveManager = FungusManager.Instance.SaveManager;
            saveManager.Rewind();
        }

        public virtual void Restart()
        {
            var saveManager = FungusManager.Instance.SaveManager;
            saveManager.ClearHistory();

            if (restartDeletesSave)
            {
                saveManager.Delete();
            }

            SceneManager.LoadScene(startScene);
        }

        #endregion
    }
}
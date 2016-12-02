using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Fungus
{
    public class SaveGameHelper : MonoBehaviour 
    {
        const string NewGameSavePointKey = "new_game";

        [SerializeField] protected string startScene = "";

        [SerializeField] protected bool autoStartGame = true;

        [SerializeField] protected bool restartDeletesSave = false;

        [SerializeField] protected SaveGameObjects saveGameObjects = new SaveGameObjects();

        protected virtual void Start()
        {
            var saveManager = FungusManager.Instance.SaveManager;

            if (autoStartGame &&
                saveManager.NumSavePoints == 0)
            {
                SavePointLoaded.NotifyEventHandlers(NewGameSavePointKey);
            }

            CheckSavePointKeys();
        }

        protected void CheckSavePointKeys()
        {
            List<string> keys = new List<string>();

            var savePoints = GameObject.FindObjectsOfType<SavePoint>();

            foreach (var savePoint in savePoints)
            {
                if (string.IsNullOrEmpty(savePoint.SavePointKey))
                {
                    continue;
                }

                if (keys.Contains(savePoint.SavePointKey))
                {
                    Debug.LogError("Save Point Key " + savePoint.SavePointKey + " is defined multiple times.");
                }
                else
                {
                    keys.Add(savePoint.SavePointKey);
                }
            }

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

        public virtual void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }

        #endregion
    }
}
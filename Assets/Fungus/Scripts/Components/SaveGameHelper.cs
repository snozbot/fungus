using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Fungus
{
    public class SaveGameHelper : MonoBehaviour 
    {
        [SerializeField] protected string startScene = "";

        [SerializeField] protected SaveGameObjects saveGameObjects = new SaveGameObjects();

        protected virtual void OnEnable()
        {
            SaveSignals.OnGameSave += OnGameSave;
        }

        protected virtual void OnDisable()
        {
            SaveSignals.OnGameSave -= OnGameSave;
        }

        protected virtual void OnGameSave(string saveKey, string saveDescription)
        {
            // TODO: Play sound effect
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
            saveManager.Clear();
            SceneManager.LoadScene(startScene);
        }

        #endregion
    }
}
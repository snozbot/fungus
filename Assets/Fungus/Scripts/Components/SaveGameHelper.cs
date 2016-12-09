using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Fungus
{
    public class SaveGameHelper : MonoBehaviour 
    {
        const string SaveDataKey = "save_data";

        const string NewGameSavePointKey = "new_game";

        [SerializeField] protected string startScene = "";

        [SerializeField] protected bool autoStartGame = true;

        [SerializeField] protected bool saveMenuActive = false;

        [SerializeField] protected bool restartDeletesSave = false;

        [SerializeField] protected CanvasGroup saveMenuGroup;

        [SerializeField] protected Button saveMenuButton;

        [SerializeField] protected Button saveButton;

        [SerializeField] protected Button loadButton;

        [SerializeField] protected Button rewindButton;

        [SerializeField] protected Button forwardButton;

        [SerializeField] protected Button restartButton;

        [SerializeField] protected SaveGameObjects saveGameObjects = new SaveGameObjects();

        protected AudioSource clickAudioSource;

        protected LTDescr fadeTween;

        protected virtual void Awake()
        {
            clickAudioSource = GetComponent<AudioSource>();
        }

        protected virtual void Start()
        {
            var saveManager = FungusManager.Instance.SaveManager;

            if (!saveMenuActive)
            {
                saveMenuGroup.alpha = 0f;
            }

            if (autoStartGame &&
                saveManager.NumSavePoints == 0)
            {
                SavePointLoaded.NotifyEventHandlers(NewGameSavePointKey);
            }

            CheckSavePointKeys();
        }

        protected virtual void Update()
        {
            var saveManager = FungusManager.Instance.SaveManager;

            if (saveButton != null)
            {
                // Don't allow saving unless there's at least one save point in the history,
                // This avoids the case where you could try to load a save data with 0 save points.
                saveButton.interactable = saveManager.NumSavePoints > 0;
            }
            if (loadButton != null)
            {
                loadButton.interactable = saveManager.SaveDataExists(SaveDataKey);
            }
            if (rewindButton != null)
            {
                rewindButton.interactable = saveManager.NumSavePoints > 1;
            }
            if (forwardButton != null)
            {
                forwardButton.interactable = saveManager.NumRewoundSavePoints > 0;
            }
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

        protected void PlayClickSound()
        {
            if (clickAudioSource != null)
            {
                clickAudioSource.Play();
            }
        }

        #region Public methods

        public SaveGameObjects SaveGameObjects { get { return saveGameObjects; } }

        public virtual void ToggleSaveMenu()
        {
            if (fadeTween != null)
            {
                LeanTween.cancel(fadeTween.id);
                fadeTween = null;
            }

            if (saveMenuActive)
            {
                // Switch menu off
                LeanTween.value(saveMenuGroup.gameObject, saveMenuGroup.alpha, 0f, 0.5f).setOnUpdate( (t) => { 
                    saveMenuGroup.alpha = t;
                });
            }
            else
            {
                // Switch menu on
                LeanTween.value(saveMenuGroup.gameObject, saveMenuGroup.alpha, 1f, 0.5f).setOnUpdate( (t) => { 
                    saveMenuGroup.alpha = t;
                });
            }

            saveMenuActive = !saveMenuActive;
        }

        public virtual void Save()
        {
            var saveManager = FungusManager.Instance.SaveManager;

            if (saveManager.NumSavePoints > 0)
            {
                PlayClickSound();
                saveManager.Save(SaveDataKey);
            }
        }

        public virtual void Load()
        {
            var saveManager = FungusManager.Instance.SaveManager;

            if (saveManager.SaveDataExists(SaveDataKey))
            {
                PlayClickSound();
                saveManager.Load(SaveDataKey);
            }
        }

        public virtual void Rewind()
        {
            PlayClickSound();

            var saveManager = FungusManager.Instance.SaveManager;
            if (saveManager.NumSavePoints > 1)
            {
                saveManager.Rewind();
            }
        }

        public virtual void Forward()
        {
            PlayClickSound();

            var saveManager = FungusManager.Instance.SaveManager;
            if (saveManager.NumRewoundSavePoints > 0)
            {
                saveManager.FastForward();
            }
        }

        public virtual void Restart()
        {
            if (string.IsNullOrEmpty(startScene))
            {
                Debug.LogError("No start scene specified");
                return;
            }

            var saveManager = FungusManager.Instance.SaveManager;

            PlayClickSound();

            saveManager.ClearHistory();
            if (restartDeletesSave)
            {
                saveManager.Delete(SaveDataKey);
            }

            SceneManager.LoadScene(startScene);
        }

        public virtual void LoadScene(string sceneName)
        {
            PlayClickSound();

            SceneManager.LoadScene(sceneName);
        }

        #endregion
    }
}
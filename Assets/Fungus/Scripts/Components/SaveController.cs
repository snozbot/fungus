// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

//TODO 
//  use meta from save manager
//  use a view component per save
//  allow change of save profile



namespace Fungus
{
    /// <summary>
    /// A singleton game object which displays a simple UI for the save system.
    /// </summary>
    public class SaveController : MonoBehaviour 
    {        //[Tooltip("Automatically load the most recently saved game on startup")]
             //[SerializeField] protected bool loadOnStart = true;

        [SerializeField] protected SaveSettings saveSettings;

        [Tooltip("The CanvasGroup containing the save menu buttons")]
        [SerializeField] protected CanvasGroup saveMenuGroup;

        [Tooltip("The button which hides / displays the save menu")]
        [SerializeField] protected Button saveMenuButton;

        [Tooltip("The button which saves the save history to disk")]
        [SerializeField] protected Button saveButton;

        [Tooltip("The button which loads the save history from disk")]
        [SerializeField] protected Button loadButton;


        //[Tooltip("The button which rewinds the save history to the previous save point.")]
        //[SerializeField] protected Button rewindButton;

        //[Tooltip("The button which fast forwards the save history to the next save point.")]
        //[SerializeField] protected Button forwardButton;
        
        [Tooltip("The button which restarts the game.")]
        [SerializeField] protected Button restartButton;

        [Tooltip("A scrollable text field used for debugging the save data. The text field should be disabled in normal use.")]
        [SerializeField] protected ScrollRect debugView;

        protected static bool saveMenuActive = false;

        protected AudioSource clickAudioSource;

        protected LTDescr fadeTween;

        protected static SaveController instance;

        protected static bool hasLoadedOnStart = false;

        protected virtual void Awake()
        {
            // Only one instance of SaveMenu may exist
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            if (transform.parent == null)
            {
                GameObject.DontDestroyOnLoad(this);
            }
            else
            {
                Debug.LogError("Save Menu cannot be preserved across scene loads if it is a child of another GameObject.");
            }

            clickAudioSource = GetComponent<AudioSource>();
        }

        protected virtual void OnEnable()
        {
            SaveManagerSignals.OnSaveSaved += OnSaveAdded;
        }

        protected virtual void OnDisable()
        {
            SaveManagerSignals.OnSaveSaved -= OnSaveAdded;
        }

        protected virtual void Start()
        {
            if (!saveMenuActive)
            {
                saveMenuGroup.alpha = 0f;
            }

            var saveManager = FungusManager.Instance.SaveManager;

            // Make a note of the current scene. This will be used when restarting the game.
            if (string.IsNullOrEmpty(saveManager.StartScene))
            {
                saveManager.StartScene = SceneManager.GetActiveScene().name;
            }

            //if (loadOnStart && !hasLoadedOnStart)
            //{
            //    hasLoadedOnStart = true;

            //    if (saveManager.SaveDataExists(saveDataKey))
            //    {
            //        saveManager.Load(saveDataKey);
            //    }
            //}
        }

        //todo this looks like it should just be done when the menu is toggled/interacted with
        protected virtual void Update()
        {
            var saveManager = FungusManager.Instance.SaveManager;

            // Hide the Save and Load buttons if autosave is on

            //bool showSaveAndLoad = !autoSave;
            //if (saveButton.IsActive() != showSaveAndLoad)
            //{
            //    saveButton.gameObject.SetActive(showSaveAndLoad);
            //    loadButton.gameObject.SetActive(showSaveAndLoad);
            //}
 
            
            //if (saveButton != null)
            //{
            //    // Don't allow saving unless there's at least one save point in the history,
            //    // This avoids the case where you could try to load a save data with 0 save points.
            //    saveButton.interactable = saveManager.NumSavePoints > 0 && saveMenuActive;
            //}
            if (loadButton != null)
            {
                loadButton.interactable = saveManager.NumSaves > 0 && saveMenuActive;
            }
            

            if (restartButton != null)
            {
                restartButton.interactable = saveMenuActive;
            }

            //if (rewindButton != null)
            //{
            //    rewindButton.interactable = saveManager.NumSavePoints > 0 && saveMenuActive;
            //}
            //if (forwardButton != null)
            //{
            //    forwardButton.interactable = saveManager.NumRewoundSavePoints > 0 && saveMenuActive;
            //}

            if (debugView.enabled)
            {
                var debugText = debugView.GetComponentInChildren<Text>();
                if (debugText != null)
                {
                    debugText.text = saveManager.GetDebugInfo();
                }
            }

        }

        protected virtual void OnSaveAdded(string savePointKey, string savePointDescription)
        {
            //if we limit autos and it is an auto, are there now to many, delete oldest until not over limit
            var autoSaves = FungusManager.Instance.SaveManager.SaveMetas.Where(x => x.saveName.StartsWith(FungusConstants.AutoSavePrefix))
                .OrderBy(x => x.savePointLastWritten).ToList();

            for (int i = 0; i < autoSaves.Count - saveSettings.NumberOfAutoSaves; i++)
            {
                FungusManager.Instance.SaveManager.DeleteSave(autoSaves[i]);
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


        /// <summary>
        /// Toggles the expanded / collapsed state of the save menu.
        /// Uses a tween to fade the menu UI in and out.
        /// </summary>
        public virtual void ToggleSaveMenu()
        {
            if (fadeTween != null)
            {
                LeanTween.cancel(fadeTween.id, true);
                fadeTween = null;
            }

            if (saveMenuActive)
            {
                // Switch menu off
                fadeTween = LeanTween.alphaCanvas(saveMenuGroup, 0f, 0.2f)
                    .setEase(LeanTweenType.easeOutQuint)
                    .setOnComplete(() => saveMenuGroup.alpha = 0);
            }
            else
            {
                // Switch menu on
                fadeTween = LeanTween.alphaCanvas(saveMenuGroup, 1f, 0.2f)
                    .setEase(LeanTweenType.easeOutQuint)
                    .setOnComplete(() => saveMenuGroup.alpha = 1);
            }

            saveMenuActive = !saveMenuActive;
        }

        /// <summary>
        /// Handler function called when the Save button is pressed.
        /// </summary>
        //public virtual void SaveNew()
        //{
        //    //TODO need a way to save new or override

        //    var saveManager = FungusManager.Instance.SaveManager;

        //    if (saveManager.NumSavePoints > 0)
        //    {
        //        PlayClickSound();
        //        saveManager.Save(saveDataKey);
        //    }
        //}

        /// <summary>
        /// Handler function called when the Load button is pressed.
        /// </summary>
        public virtual void LoadMostRecent()
        {
            var saveManager = FungusManager.Instance.SaveManager;

            var newestSaveTime = saveManager.SaveMetas.Max(x => x.savePointLastWritten);

            var mostRecentMeta = saveManager.SaveMetas.FirstOrDefault(x => x.savePointLastWritten == newestSaveTime);

            if (mostRecentMeta != null && saveManager.Load(mostRecentMeta))
            {
                PlayClickSound();
            }
        }

        /// <summary>
        /// Handler function called when the Rewind button is pressed.
        /// </summary>
        //public virtual void Rewind()
        //{
        //    PlayClickSound();

        //    var saveManager = FungusManager.Instance.SaveManager;
        //    if (saveManager.NumSavePoints > 0)
        //    {
        //        saveManager.Rewind();
        //    }

        //}

        /// <summary>
        /// Handler function called when the Fast Forward button is pressed.
        /// </summary>
        //public virtual void FastForward()
        //{
        //    PlayClickSound();

        //    var saveManager = FungusManager.Instance.SaveManager;
        //    if (saveManager.NumRewoundSavePoints > 0)
        //    {
        //        saveManager.FastForward();
        //    }
        //}

        /// <summary>
        /// Handler function called when the Restart button is pressed.
        /// </summary>
        public virtual void Restart()
        {
            var saveManager = FungusManager.Instance.SaveManager;
            if (string.IsNullOrEmpty(saveManager.StartScene))
            {
                Debug.LogError("No start scene specified");
                return;
            }

            PlayClickSound();

            // Reset the Save History for a new game
            if (saveSettings.RestartDeletesSave)
            {
                saveManager.DeleteAllSaves();
                SaveManagerSignals.DoSaveReset();
            }

            SceneManager.LoadScene(saveManager.StartScene);
        }

        #endregion
    }
}

#endif
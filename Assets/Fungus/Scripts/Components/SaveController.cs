// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using System.Collections.Generic;

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

        [Tooltip("The button which deletes the selected slot")]
        [SerializeField] protected Button deleteButton;


        //[Tooltip("The button which rewinds the save history to the previous save point.")]
        //[SerializeField] protected Button rewindButton;

        //[Tooltip("The button which fast forwards the save history to the next save point.")]
        //[SerializeField] protected Button forwardButton;
        
        [SerializeField] protected SaveSlotController slotPrefab;

        [SerializeField] protected Text timeSinceLastSaveText;
        protected DateTime lastSaveTime;

        protected SaveSlotController selectedSaveSlot;
        public void SetSelectedSlot(SaveSlotController saveSlotController)
        {
            selectedSaveSlot = saveSlotController;
            selectedSaveSlot.OurButton.Select();
        }

        protected List<SaveSlotController> autoSaveSlots = new List<SaveSlotController>();
        protected List<SaveSlotController> userSaveSlots = new List<SaveSlotController>();

        [SerializeField] protected RectTransform autoSaveScrollViewContainer, userSaveScrollViewContainer;

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
            SaveManagerSignals.OnSaveDeleted += OnSaveDeleted;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        protected virtual void OnDisable()
        {
            SaveManagerSignals.OnSaveSaved -= OnSaveAdded;
            SaveManagerSignals.OnSaveDeleted -= OnSaveDeleted;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }
        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            //force menu off after load
            saveMenuActive = true;
            ToggleSaveMenu();
        }

        protected virtual void OnSaveAdded(string savePointKey, string savePointDescription)
        {
            UpdateSlots();
        }
        protected virtual void OnSaveDeleted(string savePointKey)
        {
            UpdateSlots();
        }

        protected virtual void Start()
        {
            if (!saveMenuActive)
            {
                saveMenuGroup.alpha = 0f;
            }

            var saveManager = FungusManager.Instance.SaveManager;
            saveManager.SaveSettings = saveSettings;

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
            UpdateSlots();
        }

        public void UpdateSlots()
        {
            var saveMan = FungusManager.Instance.SaveManager;

            var mostRecentMeta = saveMan.GetMostRecentSave();
            if (mostRecentMeta != null)
            {
                lastSaveTime = mostRecentMeta.savePointLastWritten;
            }
            //if unlimited

            //ensure we have the correct number of slots
            //while(slots.Count < saveMan.NumSaves)
            //{
            //    //add more
            //    var newSlot = Instantiate(slotPrefab, scrollViewContainer);
            //    slots.Add(newSlot);
            //}
            //while(slots.Count < saveMan.NumSaves)
            //{
            //    //remove excess
            //    Destroy(slots.Last().gameObject);
            //    slots.RemoveAt(slots.Count - 1);
            //}

            //if specificed number of slots
            var autoSaves = saveMan.CollectAutoSaves();
            AdjustAndUpdateSaveSlots(autoSaves, autoSaveSlots, autoSaveScrollViewContainer);

            var userSaves = saveMan.CollectUserSaves();
            AdjustAndUpdateSaveSlots(userSaves, userSaveSlots, userSaveScrollViewContainer);
        }

        protected virtual void AdjustAndUpdateSaveSlots(List<SaveManager.SavePointMeta> saves, List<SaveSlotController> slots, RectTransform scrollViewContainer)
        {
            while (slots.Count < saves.Count)
            {
                var newSlot = Instantiate(slotPrefab, scrollViewContainer);
                slots.Add(newSlot);
            }
            //shouldnt happen but be thorough
            while (slots.Count > saves.Count)
            {
                //    //remove excess
                Destroy(slots.Last().gameObject);
                slots.RemoveAt(slots.Count - 1);
            }

            for (int i = 0; i < saves.Count; i++)
            {
                slots[i].LinkedMeta = saves[i];
            }

            scrollViewContainer.parent.gameObject.SetActive(saves.Count != 0);
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
                loadButton.interactable = saveMenuActive;
            }

            if (saveButton != null)
            {
                saveButton.interactable = saveMenuActive && selectedSaveSlot != null && selectedSaveSlot.LinkedMeta != null &&
                    selectedSaveSlot.LinkedMeta.saveName.StartsWith(FungusConstants.UserSavePrefix);
            }

            if (deleteButton != null)
            {
                deleteButton.interactable = saveMenuActive && selectedSaveSlot != null && selectedSaveSlot.LinkedMeta != null
                    && !string.IsNullOrEmpty(selectedSaveSlot.LinkedMeta.fileLocation);
            }

            //if (rewindButton != null)
            //{
            //    rewindButton.interactable = saveManager.NumSavePoints > 0 && saveMenuActive;
            //}
            //if (forwardButton != null)
            //{
            //    forwardButton.interactable = saveManager.NumRewoundSavePoints > 0 && saveMenuActive;
            //}

            if (timeSinceLastSaveText != null && timeSinceLastSaveText.gameObject.activeInHierarchy && timeSinceLastSaveText.isActiveAndEnabled)
            {
                timeSinceLastSaveText.text = "Since last save: " + (DateTime.Now - lastSaveTime).ToString(@"dd\.hh\:mm\:ss");
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
            selectedSaveSlot = null;
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
                    .setOnComplete(() => { saveMenuGroup.alpha = 0; saveMenuGroup.interactable = false; saveMenuGroup.blocksRaycasts = false; }) ;
            }
            else
            {
                // Switch menu on
                fadeTween = LeanTween.alphaCanvas(saveMenuGroup, 1f, 0.2f)
                    .setEase(LeanTweenType.easeOutQuint)
                    .setOnComplete(() => { saveMenuGroup.alpha = 1; saveMenuGroup.interactable = true; saveMenuGroup.blocksRaycasts = true; });
            }

            saveMenuActive = !saveMenuActive;
        }

        /// <summary>
        /// Handler function called when the Save button is pressed.
        /// </summary>
        public virtual void SaveOver()
        {
            //TODO
            var saveManager = FungusManager.Instance.SaveManager;

            if (selectedSaveSlot != null)
            {
                //todo better desc
                if (selectedSaveSlot.LinkedMeta != null && selectedSaveSlot.LinkedMeta.saveName.StartsWith(FungusConstants.UserSavePrefix))
                {
                    saveManager.Save(selectedSaveSlot.LinkedMeta.saveName, AutoSave.TimeStampDesc);
                    PlayClickSound();
                }
            }
        }

        public virtual void DeleteSelectedSave()
        {
            if(selectedSaveSlot != null && selectedSaveSlot.LinkedMeta != null)
            {
                var saveManager = FungusManager.Instance.SaveManager;
                saveManager.DeleteSave(selectedSaveSlot.LinkedMeta);
                PlayClickSound();
            }
        }

        //todo this should instead find an empty slot
        public virtual void SaveNew()
        {
            //var saveManager = FungusManager.Instance.SaveManager;
            //var userSaves = saveManager.CollectUserSaves();

            //if (userSaves.Count < saveSettings.NumberOfUserSaves)
            //{
            //    saveManager.Save(FungusConstants.UserSavePrefix + (userSaves.Count + 1).ToString(), AutoSave.TimeStampDesc);
            //    PlayClickSound();
            //}

            //writting to empty slot
            //saveManager.Save(selectedSaveSlot, AutoSave.TimeStampDesc);
        }

        /// <summary>
        /// Handler function called when the Load button is pressed.
        /// </summary>
        public virtual void LoadMostRecent()
        {
            var saveManager = FungusManager.Instance.SaveManager;

            var mostRecentMeta = saveManager.GetMostRecentSave();

            if (mostRecentMeta != null && saveManager.Load(mostRecentMeta))
            {
                PlayClickSound();
            }
        }

        public virtual void LoadSelected()
        {
            var saveManager = FungusManager.Instance.SaveManager;

            if (selectedSaveSlot != null && selectedSaveSlot.LinkedMeta != null && saveManager.Load(selectedSaveSlot.LinkedMeta))
            {
                PlayClickSound();
            }
            else
            {
                //TODO not final, used for testing
                if (string.IsNullOrEmpty(saveManager.StartScene))
                {
                    Debug.LogError("No start scene specified");
                    return;
                }

                PlayClickSound();

                //// Reset the Save History for a new game
                //if (saveSettings.RestartDeletesSave)
                //{
                //    saveManager.DeleteAllSaves();
                //    SaveManagerSignals.DoSaveReset();
                //}

                SceneManager.LoadScene(saveManager.StartScene);
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


        #endregion
    }
}

#endif
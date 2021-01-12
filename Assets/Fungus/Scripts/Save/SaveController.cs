// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// A default singleton controller of save menus, connecting metas, to buttons.
    /// </summary>
    public class SaveController : MonoBehaviour
    {
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

        [SerializeField] protected SaveSlotController slotPrefab;

        protected SaveSlotController selectedSaveSlot;

        protected List<SaveSlotController> autoSaveSlots = new List<SaveSlotController>();
        protected List<SaveSlotController> userSaveSlots = new List<SaveSlotController>();

        [SerializeField] protected RectTransform autoSaveScrollViewContainer, userSaveScrollViewContainer;

        protected bool saveMenuActive = false;

        protected AudioSource clickAudioSource;

        protected LTDescr fadeTween;

        protected static SaveController instance;

        protected static bool hasLoadedOnStart = false;

        protected SaveManager saveManager;

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

            saveManager = FungusManager.Instance.SaveManager;
        }

        protected virtual void OnEnable()
        {
            SaveManagerSignals.OnSaveSaved += OnSaveAddedHandler;
            SaveManagerSignals.OnSaveDeleted += OnSaveDeletedHandler;
            SaveManagerSignals.OnSaveMetasRefreshed += SaveManagerSignals_OnSaveMetasRefreshed;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }

        protected virtual void OnDisable()
        {
            SaveManagerSignals.OnSaveSaved -= OnSaveAddedHandler;
            SaveManagerSignals.OnSaveDeleted -= OnSaveDeletedHandler;
            SaveManagerSignals.OnSaveMetasRefreshed -= SaveManagerSignals_OnSaveMetasRefreshed;
            SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            //force menu off after load
            saveMenuActive = true;
            ToggleSaveMenu();
        }

        protected virtual void OnSaveAddedHandler(string savePointKey, string savePointDescription)
        {
            UpdateSlots();
        }

        protected virtual void OnSaveDeletedHandler(string savePointKey)
        {
            UpdateSlots();
        }

        private void SaveManagerSignals_OnSaveMetasRefreshed()
        {
            UpdateSlots();
        }

        protected virtual void Start()
        {
            if (!saveMenuActive)
            {
                saveMenuGroup.alpha = 0f;
            }

            UpdateSlots();
        }

        /// <summary>
        /// Creates, destroys, and update content of SaveSlotControllers and prefab UI elements as needed
        /// by the collection save metas from the SaveManager.
        /// </summary>
        public void UpdateSlots()
        {
            var saveMan = saveManager;

            var autoSaves = saveMan.CollectAutoSaves();
            AdjustAndUpdateSaveSlots(autoSaves, autoSaveSlots, autoSaveScrollViewContainer);

            var userSaves = saveMan.CollectUserSaves();
            AdjustAndUpdateSaveSlots(userSaves, userSaveSlots, userSaveScrollViewContainer);

            SetSelectedSlot(null);
        }

        private void Update()
        {
            if (selectedSaveSlot != null)
            {
                if (!IsSelectingSlot && !IsSelectingSaveMenuElements)
                {
                    SetSelectedSlot(null);
                }

                // Better to have this unselect the slot when the menu isn't active
                if (!saveMenuActive)
                    SetSelectedSlot(null);
            }
        }

        private bool IsSelectingSlot =>
            EventSystem.current.currentSelectedGameObject != selectedSaveSlot.OurButton.gameObject;

        private bool IsSelectingSaveMenuElements
        {
            get
            {
                var selected = EventSystem.current.currentSelectedGameObject;
                return selected != saveButton.gameObject &&
                       selected != loadButton.gameObject &&
                       selected != deleteButton.gameObject;
            }
        }

        public void SetSelectedSlot(SaveSlotController saveSlotController)
        {
            selectedSaveSlot = saveSlotController;

            if (selectedSaveSlot != null)
                selectedSaveSlot.OurButton.Select();

            if (loadButton != null)
            {
                loadButton.interactable = saveMenuActive && selectedSaveSlot != null && selectedSaveSlot.IsLoadable && saveManager.IsLoadingAllowed;
            }

            if (deleteButton != null)
            {
                deleteButton.interactable = saveMenuActive && selectedSaveSlot != null && selectedSaveSlot.IsLoadable;
            }

            if (saveButton != null)
            {
                saveButton.interactable = saveMenuActive && selectedSaveSlot != null && selectedSaveSlot.LinkedMeta != null &&
                    selectedSaveSlot.LinkedMeta.saveName.StartsWith(FungusConstants.SlotSavePrefix) && saveManager.IsSavingAllowed;
            }
        }

        /// <summary>
        /// Helper for ensureing the correct number of slot ui elements and linking them to their metas.
        /// If there are none, it disables the ScrollView.
        /// </summary>
        /// <param name="saves"></param>
        /// <param name="slots"></param>
        /// <param name="scrollViewContainer"></param>
        protected virtual void AdjustAndUpdateSaveSlots(
            List<SaveGameMetaData> saves,
            List<SaveSlotController> slots,
            RectTransform scrollViewContainer)
        {
            while (slots.Count < saves.Count)
            {
                var newSlot = Instantiate(slotPrefab, scrollViewContainer);
                slots.Add(newSlot);
            }
            //shouldnt happen but be thorough
            while (slots.Count > saves.Count)
            {
                //remove excess
                Destroy(slots.Last().gameObject);
                slots.RemoveAt(slots.Count - 1);
            }

            for (int i = 0; i < saves.Count; i++)
            {
                slots[i].LinkedMeta = saves[i];
            }

            scrollViewContainer.parent.gameObject.SetActive(saves.Count != 0);
        }

        protected void PlayClickSound()
        {
            if (clickAudioSource != null)
            {
                clickAudioSource.Play();
            }
        }

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
                    .setOnComplete(() => { saveMenuGroup.alpha = 0; saveMenuGroup.interactable = false; saveMenuGroup.blocksRaycasts = false; });
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
        /// Handler function called when the Save button is pressed. Saves over the currently selected slot,
        /// doing nothing if there's no slot selected.
        /// </summary>
        public virtual void SaveOver()
        {
            if (selectedSaveSlot != null)
            {
                if (selectedSaveSlot.LinkedMeta != null &&
                    selectedSaveSlot.LinkedMeta.saveName.StartsWith(FungusConstants.SlotSavePrefix))
                {
                    saveManager.ReplaceSave(selectedSaveSlot.LinkedMeta, AutoSave.TimeStampDesc);
                    PlayClickSound();
                    //selectedSaveSlot.RefreshDisplay();
                }
            }
        }

        /// <summary>
        /// If selected slot is bound to a meta, requests the SaveManager delete it.
        /// </summary>
        public virtual void DeleteSelectedSave()
        {
            if (selectedSaveSlot != null && selectedSaveSlot.LinkedMeta != null)
            {
                saveManager.DeleteSave(selectedSaveSlot.LinkedMeta);
                PlayClickSound();
            }
        }

        /// <summary>
        /// Helper for loading the most recently created save, regardless of type.
        /// </summary>
        public virtual void LoadMostRecent()
        {
            var mostRecentMeta = saveManager.GetMostRecentSave();

            if (mostRecentMeta != null && saveManager.Load(mostRecentMeta))
            {
                PlayClickSound();
            }
        }

        /// <summary>
        /// If there is a currently selected slot with a valid meta, asks the SaveManager to load it.
        /// </summary>
        public virtual void LoadSelected()
        {
            if (selectedSaveSlot != null && selectedSaveSlot.LinkedMeta != null && saveManager.Load(selectedSaveSlot.LinkedMeta))
            {
                PlayClickSound();
            }
        }
    }
}

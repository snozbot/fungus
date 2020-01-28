using UnityEngine;
using UnityEngine.UI;

//todo needs updates, should be created via prefab by menu

namespace Fungus.SaveSystem
{
    [RequireComponent(typeof(RectTransform))]
    public class SaveSlot : MonoBehaviour
    {
        #region Fields

        [Tooltip("Displays the number for this slot.")]
        [SerializeField] private Text numDisplay = null;

        [Tooltip("Displays the description for this slot.")]
        [SerializeField] private Text descDisplay = null;

        protected Button clickReceiver = null;
        protected GameSaveData saveData = null;
        private int number; // Cached to avoid too much casting

        #endregion Fields

        #region Properties

        #region UI Elements

        public RectTransform rectTransform { get; private set; }

        #endregion UI Elements

        /// <summary>
        /// Defines which slot this is in the Save Menu. First, second, etc.
        /// </summary>
        public virtual int Number
        {
            get { return number; }
            protected set
            {
                number = value;
                numDisplay.text = "Save #" + number.ToString();
            }
        }

        public virtual string Description
        {
            get { return descDisplay.text; }
            protected set { descDisplay.text = value; }
        }

        public virtual GameSaveData SaveData
        {
            get { return saveData; }
            set
            {
                if (value == null)
                {
                    string message =
                    @"Cannot assign null GameSaveData to a Save Slot. If you want to
                    clear the slot, call its Clear() method instead.";
                    Debug.LogWarning(message);
                    return;
                }

                saveData = value;
                UpdateDisplays();
                Signals.SaveSlotUpdated.Invoke(this, value);
            }
        }

        #endregion Properties

        #region Methods

        protected virtual void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            Number = rectTransform.GetSiblingIndex();
            clickReceiver = GetComponent<Button>();
            clickReceiver.onClick.AddListener(OnClick);
            UpdateDisplays();
        }

        protected virtual void OnDestroy()
        {
            clickReceiver.onClick.RemoveListener(OnClick);
        }

        public virtual void Clear()
        {
            saveData = null;
            UpdateDisplays();
            Signals.SaveSlotUpdated.Invoke(this, SaveData);
        }

        /// <summary>
        /// Updates the UI elements in this SaveSlot based on the save data (or lack thereof) it holds.
        /// </summary>
        protected virtual void UpdateDisplays()
        {
            string newDesc = null;

            if (saveData != null)
                newDesc = saveData.Description;
            else
                newDesc = "<No Save Data>";

            Description = newDesc;
        }

        protected virtual void OnClick()
        {
            Signals.SaveSlotClicked.Invoke(this);
        }

        #endregion Methods
    }
}
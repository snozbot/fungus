// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Links ui element to a save slot, communicates back forth between a slot, ui, interactions and the SaveController
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SaveSlotController : MonoBehaviour, ISelectHandler
    {

        [SerializeField] protected Button ourButton;
        public virtual Button OurButton { get { return ourButton; } }

        protected SaveGameMetaData ourMeta;

        protected SaveController saveCont;

        public bool IsLoadable { get { return ourMeta != null && !string.IsNullOrEmpty(ourMeta.fileLocation); } }

        private ISaveSlotView[] slotViews;

        protected virtual void Awake()
        {
            slotViews = GetComponentsInChildren<ISaveSlotView>();
        }

        private void Start()
        {
            saveCont = GetComponentInParent<SaveController>();

            if (saveCont == null)
            {
                // The line below is a bit misleading; it doesn't execute based on clicking
                //Debug.LogError("SaveSlot clicked without a SaveController, not allowed");
            }
        }

        public SaveGameMetaData LinkedMeta
        {
            get
            {
                return ourMeta;
            }
            set
            {
                ourMeta = value;

                RefreshDisplay();
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            saveCont.SetSelectedSlot(this);
        }

        public virtual void RefreshDisplay()
        {
            UpdateViews();
        }

        protected virtual void UpdateViews()
        {
            // Pass the metadata to the views, so they can do their thing with it.
            slotViews = GetComponentsInChildren<ISaveSlotView>();
            for (int i = 0; i < slotViews.Length; i++)
            {
                ISaveSlotView currentView = slotViews[i];
                currentView.SaveData = LinkedMeta;
            }
        }

        /// <summary>
        /// Returns a view object of the specified type registered under this controller. Returns null
        /// if no such object exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public virtual T GetView<T>() where T: class, ISaveSlotView
        {
            foreach (var view in slotViews)
            {
                if (view is T)
                    return view as T;
            }

            return null;
        }
    }
}

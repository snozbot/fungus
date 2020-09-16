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
        [Tooltip("Displays the number for this slot.")]
        [SerializeField] protected Text nameText = null;

        [Tooltip("Displays the description for this slot.")]
        [SerializeField] protected Text descText = null;

        [Tooltip("To display time save was created at.")]
        [SerializeField] protected Text timeStampText = null;

        [SerializeField] protected Button ourButton;
        public virtual Button OurButton { get { return ourButton; } }

        protected SaveGameMetaData ourMeta;

        protected SaveController saveCont;

        public bool IsLoadable { get { return ourMeta != null && !string.IsNullOrEmpty(ourMeta.fileLocation); } }

        private void Start()
        {
            saveCont = GetComponentInParent<SaveController>();

            if (saveCont == null)
            {
                Debug.LogError("SaveSlot clicked without a SaveController, not allowed");
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
                //update views
                if (ourMeta != null)
                {
                    nameText.text = ourMeta.saveName;
                    descText.text = ourMeta.description;
                    timeStampText.text = ourMeta.GetReadableTime();
                }
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            saveCont.SetSelectedSlot(this);
        }
    }
}

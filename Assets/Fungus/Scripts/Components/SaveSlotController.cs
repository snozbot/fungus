using UnityEngine;
using UnityEngine.UI;

//todo needs updates, should be created via prefab by menu

namespace Fungus
{
    [RequireComponent(typeof(RectTransform))]
    public class SaveSlotController : MonoBehaviour
    {
        [Tooltip("Displays the number for this slot.")]
        [SerializeField] protected Text nameText = null;

        [Tooltip("Displays the description for this slot.")]
        [SerializeField] protected Text descText = null;

        [Tooltip("To display time save was created at.")]
        [SerializeField] protected Text timeStampText = null;

        [SerializeField] protected Button ourButton;
        public virtual Button OurButton { get { return ourButton; } }

        protected SaveManager.SavePointMeta ourMeta;

        public SaveManager.SavePointMeta LinkedMeta 
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
                    descText.text = ourMeta.savePointDescription;
                    timeStampText.text = ourMeta.savePointLastWritten.ToLongDateString();
                }
            }
        }
        
        public virtual void OnClick()
        {
            var cont = GetComponentInParent<SaveController>();
            if (cont != null)
            {
                cont.SetSelectedSlot(this);
            }
            else
            {
                Debug.LogError("SaveSlot clicked without a SaveController, not allowed");
            }
        }
    }
}
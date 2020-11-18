using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Displays the number of the save slot, allowing the user to customize how it's displayed.
    /// </summary>
    public class SaveSlotNumberView : SlotTextView
    {
        [Tooltip("This is shown right before the slot number.")]
        [SerializeField] protected string prefix = "Save #";
        [Tooltip("This is shown right after the slot number.")]
        [SerializeField] protected string postfix = "";

        // Having this be settable in the Inspector, since GetComponentInParent doesn't always work...
        [SerializeField] protected SaveSlotController controller = null;

        protected override void UpdateText()
        {
            base.UpdateText();
            SetStringToDisplay();
            textField.text = toDisplay;
        }

        protected virtual void SetStringToDisplay()
        {
            // This doesn't really need to know anything about the save data tied to the slot;
            // it gets the number based on where it is in the holder.
            int slotNumber = controller.transform.GetSiblingIndex();
            toDisplay = string.Concat("", prefix, slotNumber, postfix);
        }

        protected string toDisplay = "";
    }
}
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Displays the number of the save slot, allowing the user to customize how it's displayed.
    /// </summary>
    public class SaveSlotNumberView : SlotTextView
    {
        public virtual string Prefix { get { return prefix; } }

        [Tooltip("This is shown right before the slot number.")]
        [SerializeField] protected string prefix = "Save #";

        public virtual string Postfix { get { return postfix; } }

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
            toDisplay = string.Concat("", prefix, SlotNumber, postfix);
        }

        public virtual int SlotNumber { get { return controller.transform.GetSiblingIndex(); } }

        protected string toDisplay = "";
    }
}

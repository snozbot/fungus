namespace Fungus
{
    /// <summary>
    /// Displays the description on a text field on a Save Slot.
    /// </summary>
    public class SaveDescriptionView : SlotTextView
    {
        protected override void UpdateText()
        {
            base.UpdateText();
            if (this.HasSaveData)
                textField.text = SaveData.description;
        }
    }
}
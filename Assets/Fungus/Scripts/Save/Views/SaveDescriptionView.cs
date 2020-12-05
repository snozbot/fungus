// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

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

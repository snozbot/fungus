// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// For components that display some aspect of SaveData on a single Text component.
    /// </summary>
    public abstract class SlotTextView : SaveSlotView
    {
        [SerializeField] protected Text textField = null;

        public override void Refresh()
        {
            UpdateText();
        }

        protected virtual void UpdateText()
        {
            EmptyTextFieldWhenNoSaveData();
        }

        protected virtual void EmptyTextFieldWhenNoSaveData()
        {
            if (!this.HasSaveData)
                textField.text = "";
        }
    }
}

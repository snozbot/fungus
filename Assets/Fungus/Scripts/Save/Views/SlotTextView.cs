using System.Collections;
using System.Collections.Generic;
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
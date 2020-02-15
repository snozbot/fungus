// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Requests SaveManager load a recent save, be it Auto, User, or Regardless of type.
    /// </summary>
    [CommandInfo("Save",
                 "Save To Slot",
                 "Requests SaveManager save to a specificed slot by index.")]
    public class SaveToSlot : Command
    {
        [SerializeField] protected IntegerData slotIndex = new IntegerData(0);

        public override void OnEnter()
        {
            var saveMan = FungusManager.Instance.SaveManager;

            var saveIndex = saveMan.SaveNameToIndex(FungusConstants.UserSavePrefix + slotIndex.Value.ToString());
            if (saveIndex >= 0)
            {
                saveMan.Save(saveMan.SaveMetas[saveIndex].saveName, AutoSave.TimeStampDesc);
            }
            else
            {
                Debug.LogError("No save slot available at index " + slotIndex.Value.ToString());
            }

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override string GetSummary()
        {
            return slotIndex.Value.ToString();
        }

        public override bool HasReference(Variable variable)
        {
            return variable == slotIndex.integerRef || base.HasReference(variable);
        }
    }
}
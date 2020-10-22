// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Requests SaveManager load a specificed slot by index.
    /// </summary>
    [CommandInfo("Save",
                 "Load From Slot",
                 "Requests SaveManager load a specificed slot by index.")]
    public class LoadFromSlot : Command
    {
        [SerializeField] protected IntegerData slotIndex = new IntegerData(0);

        public override void OnEnter()
        {
            var saveManComp = FungusManager.Instance.SaveManager;

            if (!saveManComp.LoadSlot(slotIndex))
            {
                Debug.LogError("No save found at index " + slotIndex.Value.ToString());
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

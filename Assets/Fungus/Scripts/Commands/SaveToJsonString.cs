// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Directly calls Encode on the SavePointData, resulting in a jsonified savedata in string var.
    ///
    /// This does not use the SaveManager and will not fire signals, you probably want SaveToSlot.
    /// </summary>
    [CommandInfo("Save",
                 "Save To Json String",
                 "Directly calls Encode on the SavePointData, resulting in a jsonified savedata in string var." +
                 "This does not use the SaveManager and will not fire signals, you probably want SaveToSlot.")]
    public class SaveToJsonString : Command
    {
        [SerializeField] protected StringData saveName;
        [SerializeField] protected StringData saveDescription;

        [VariableProperty(typeof(StringVariable))]
        [SerializeField] protected StringVariable jsonStringVar;

        public override void OnEnter()
        {
            //prevent us being saving from being part of the save, will lead to looping
            var savingAllowed = ParentBlock.IsSavingAllowed;
            ParentBlock.IsSavingAllowed = false;

            var saveHandler = FungusManager.Instance.SaveManager.SaveFileManager.CurrentSaveHandler;
            var saveData = saveHandler.CreateSaveData(saveName.Value, saveDescription.Value);

            jsonStringVar.Value = saveHandler.EncodeToJSON(saveData);

            ParentBlock.IsSavingAllowed = savingAllowed;

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override string GetSummary()
        {
            return jsonStringVar != null ? jsonStringVar.Key : "Error: No String Var set";
        }

        public override bool HasReference(Variable variable)
        {
            return variable == jsonStringVar || base.HasReference(variable);
        }
    }
}
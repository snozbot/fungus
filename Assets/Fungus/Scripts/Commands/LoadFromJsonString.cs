// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Directly calls Decode on a SavePointData created from supplied json in a string.
    ///
    /// This does not use the SaveManager and will not fire signals.
    /// </summary>
    [CommandInfo("Save",
                 "Load From Json String",
                 "Directly calls Decode on a SavePointData created from supplied json in a string" +
                 "This does not use the SaveManager and will not fire signals.")]
    public class LoadFromJsonString : Command
    {
        [SerializeField] protected StringData jsonString;

        public override void OnEnter()
        {
            var saveHandler = FungusManager.Instance.SaveManager.SaveFileManager.CurrentSaveHandler;
            var saveData = saveHandler.DecodeFromJSON(jsonString.Value);

            saveHandler.LoadSaveData(saveData);

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override string GetSummary()
        {
            return jsonString.stringRef != null ? jsonString.stringRef.Key : jsonString.Value;
        }

        public override bool HasReference(Variable variable)
        {
            return variable == jsonString.stringRef || base.HasReference(variable);
        }
    }
}
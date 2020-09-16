// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Linq;
using UnityEngine;
using static Fungus.SaveManager;

namespace Fungus
{
    /// <summary>
    /// Fetch the available meta data about a target save by type and index in the SaveManager
    /// </summary>
    [CommandInfo("Save",
                 "Get Meta Data",
                 "Fetch the available meta data about a target save by type and index in the SaveManager")]
    public class GetSaveMetaData : Command
    {
        [SerializeField] protected SaveType saveType = SaveType.Any;

        [Tooltip("")]
        [SerializeField] protected IntegerData saveIndexRequested;

        [Tooltip("")]
        [VariableProperty(typeof(StringVariable))]
        [SerializeField] protected StringVariable nameString;

        [Tooltip("")]
        [VariableProperty(typeof(StringVariable))]
        [SerializeField] protected StringVariable descString;


        [Tooltip("")]
        [VariableProperty(typeof(StringVariable))]
        [SerializeField] protected StringVariable dateString;

        public override void OnEnter()
        {
            SaveGameMetaData save = null;
            var saveMan = FungusManager.Instance.SaveManager;

            switch (saveType)
            {
                case SaveType.Auto:
                    save = saveMan.CollectAutoSaves().ElementAtOrDefault(saveIndexRequested.Value);
                    break;

                case SaveType.Slot:
                    save = saveMan.CollectUserSaves().ElementAtOrDefault(saveIndexRequested.Value);
                    break;

                case SaveType.Any:
                    save = saveMan.SaveFileManager.SaveMetas.ElementAtOrDefault(saveIndexRequested.Value);
                    break;

                default:
                break;
            }

            if (save != null)
            {
                if (nameString != null)
                    nameString.Value = save.saveName;
                if (descString != null)
                    descString.Value = save.description;
                if (dateString != null)
                    dateString.Value = save.GetReadableTime();
            }
            else
            {
                Debug.LogWarning("No save meta at index " + saveIndexRequested.Value);
            }

            Continue();
        }

        public override string GetSummary()
        {
            return saveType.ToString() + " @ " + saveIndexRequested.Value;
        }

        public override bool HasReference(Variable variable)
        {
            return variable == saveIndexRequested.integerRef ||
                variable == nameString ||
                variable == descString ||
                variable == dateString ||
                base.HasReference(variable);
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }
}
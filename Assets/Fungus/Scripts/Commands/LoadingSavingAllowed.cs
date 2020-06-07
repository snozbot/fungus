// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Get or set the whether Loading and/or Saving is currently allowed within the SaveManager
    /// </summary>
    [CommandInfo("Save",
                 "Loading / Saving allowed",
                 "Get or set the whether Loading and/or Saving is currently allowed within the SaveManager")]
    public class LoadingSavingAllowed : Command
    {
        public enum GetSet
        {
            Get,
            Set,
        }

        [SerializeField] protected GetSet savingAllowedGetSet, loadingAllowedGetSet;

        [SerializeField] protected BooleanData savingAllowedBool, loadingAllowedBool;

        public override void OnEnter()
        {
            var saveMan = FungusManager.Instance.SaveManager;

            switch (savingAllowedGetSet)
            {
                case GetSet.Get:
                    savingAllowedBool.Value = saveMan.IsSavingAllowed;
                    break;

                case GetSet.Set:
                    saveMan.IsSavingAllowed = savingAllowedBool.Value;
                    break;

                default:
                break;
            }

            switch (loadingAllowedGetSet)
            {
                case GetSet.Get:
                    loadingAllowedBool.Value = saveMan.IsLoadingAllowed;
                    break;

                case GetSet.Set:
                    saveMan.IsLoadingAllowed = loadingAllowedBool.Value;
                    break;

                default:
                break;
            }

            Continue();
        }

        public override string GetSummary()
        {
            return savingAllowedGetSet.ToString() + " saving. " + loadingAllowedGetSet.ToString() + " loading.";
        }

        public override bool HasReference(Variable variable)
        {
            return variable == savingAllowedBool.booleanRef ||
                variable == loadingAllowedBool.booleanRef ||
                base.HasReference(variable);
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }
}
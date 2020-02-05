// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Set or Get the number of auto and user save slots being set in the save manager.
    /// </summary>
    [CommandInfo("Save",
                 "Configure Save Manager",
                 "Set or Get the number of auto and user save slots being set in the save manager.")]
    public class ConfigureSaveManager : Command
    {
        public enum GetSet
        {
            Get,
            Set,
        }
        [SerializeField] protected GetSet getOrSet = GetSet.Set;

        [Tooltip("Number of AutoSaves for the Save Manager.")]
        [SerializeField] protected IntegerData numAutoSaves = new IntegerData(1);

        [Tooltip("Number of User Saves for the Save Manager.")]
        [SerializeField] protected IntegerData numUserSaves = new IntegerData(0);

        public override void OnEnter()
        {
            var saveMan = FungusManager.Instance.SaveManager;

            if (getOrSet == GetSet.Get)
            {
                numAutoSaves.Value = saveMan.NumberOfAutoSaves;
                numUserSaves.Value = saveMan.NumberOfUserSaves;
            }
            else
            {
                saveMan.ConfigureSaveNumber(numAutoSaves.Value, numUserSaves.Value);
            }

            Continue();
        }

        public override string GetSummary()
        {
            return getOrSet.ToString() + " Auto:" + numAutoSaves.Value.ToString() + " User:" + numUserSaves.Value.ToString();
        }

        public override bool HasReference(Variable variable)
        {
            return base.HasReference(variable) || variable == numUserSaves.integerRef || variable == numAutoSaves.integerRef;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }
}
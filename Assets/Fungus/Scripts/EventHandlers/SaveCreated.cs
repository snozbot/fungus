// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Execute blocks after the SaveManager has completed a save.
    /// </summary>
    [EventHandlerInfo("Save",
                      "Save Complete",
                      "Execute blocks after the SaveManager has completed a save.")]
    public class SaveCreated : EventHandler
    {
        protected virtual void OnEnable()
        {
            SaveManagerSignals.OnSaveSaved += SaveManagerSignals_OnSaveSaved;
        }

        protected virtual void OnDisable()
        {
            SaveManagerSignals.OnSaveSaved -= SaveManagerSignals_OnSaveSaved;
        }

        protected virtual void SaveManagerSignals_OnSaveSaved(string saveName, string saveDescription)
        {
            ExecuteBlock();
        }
    }
}

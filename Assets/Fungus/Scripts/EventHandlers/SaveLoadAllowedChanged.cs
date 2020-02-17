// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    /// <summary>
    /// Execute blocks after the SaveManager has signaled a change to either saving or loading being allowed or disallowed.
    /// </summary>
    [EventHandlerInfo("Save",
                      "Saving Loading Allowed chagned",
                      "Execute blocks after the SaveManager has signaled a change to either saving or loading being allowed or disallowed.")]
    public class SaveLoadAllowedChanged : EventHandler
    {
        protected virtual void OnEnable()
        {
            SaveManagerSignals.OnSavingLoadingAllowedChanged += SaveManagerSignals_OnSavingLoadingAllowedChanged;
        }

        protected virtual void OnDisable()
        {
            SaveManagerSignals.OnSavingLoadingAllowedChanged -= SaveManagerSignals_OnSavingLoadingAllowedChanged;
        }

        protected virtual void SaveManagerSignals_OnSavingLoadingAllowedChanged()
        {
            ExecuteBlock();
        }
    }
}
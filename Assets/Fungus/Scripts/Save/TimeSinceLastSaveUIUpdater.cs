// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    public class TimeSinceLastSaveUIUpdater : MonoBehaviour
    {
        [SerializeField] protected UnityEngine.UI.Text timeSinceLastSaveText;
        protected System.DateTime lastSaveTime;

        protected virtual void OnEnable()
        {
            SaveManagerSignals.OnSaveSaved += OnSaveAdded;
            SaveManagerSignals.OnSaveDeleted += OnSaveDeleted;
        }

        protected virtual void OnDisable()
        {
            SaveManagerSignals.OnSaveSaved -= OnSaveAdded;
            SaveManagerSignals.OnSaveDeleted -= OnSaveDeleted;
        }

        private void Update()
        {
            UpdateTimeSinceLastSave();
        }

        protected virtual void OnSaveAdded(string savePointKey, string savePointDescription)
        {
            UpdateLastSaveTime();
        }

        protected virtual void OnSaveDeleted(string savePointKey)
        {
            UpdateLastSaveTime();
        }

        protected void UpdateLastSaveTime()
        {
            var mostRecentMeta = FungusManager.Instance.SaveManager.GetMostRecentSave();
            if (mostRecentMeta != null)
            {
                lastSaveTime = mostRecentMeta.lastWritten;
            }
        }

        protected virtual void UpdateTimeSinceLastSave()
        {
            if (timeSinceLastSaveText != null && timeSinceLastSaveText.gameObject.activeInHierarchy && timeSinceLastSaveText.isActiveAndEnabled)
            {
                timeSinceLastSaveText.text = "Since last save: " + (System.DateTime.Now - lastSaveTime).ToString(@"dd\.hh\:mm\:ss");
            }
        }
    }
}

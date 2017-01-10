// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    [CommandInfo("Flow",
                 "Save Point", 
                 "Creates a save point which can be saved to persistant storage and loaded again later.")]
    public class SavePoint : Command
    {
        [Tooltip("A string key which identifies this save point. Must be unique in this scene.")]
        [SerializeField] protected string savePointKey;

        [Tooltip("A short description of this save point.")]
        [SerializeField] protected StringData savePointDescription;

        [Tooltip("Fire an event that will be handled by any Save Point Loaded event handler with matching key.")]
        [SerializeField] protected bool fireEvent = true;

        [Tooltip("Resume execution from this command after loading a save point.")]
        [SerializeField] protected bool resumeFromHere = true;

        #region Public members

        /// <summary>
        /// A string key which identifies this save point. Must be unique in this scene.
        /// </summary>
        public string SavePointKey { get { return savePointKey; } }

        /// <summary>
        /// Resume execution from this command after loading a save point.
        /// </summary>
        public bool ResumeFromHere { get { return resumeFromHere; } }

        public override void OnEnter()
        {
            if (string.IsNullOrEmpty(savePointKey))
            {
                Continue();
                return;
            }

            var saveManager = FungusManager.Instance.SaveManager;

            saveManager.AddSavePoint(savePointKey, savePointDescription.Value);

            if (fireEvent)
            {
                SavePointLoaded.NotifyEventHandlers(savePointKey);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (string.IsNullOrEmpty(savePointKey))
            {
                return "Error: Save Point Key not specified";
            }

            return savePointKey + " : " + savePointDescription.Value;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}
// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    [CommandInfo("Flow",
                 "Save Point", 
                 "Creates a Save Point and adds it to the Save History. The player can save the Save History to peristent storage and load it again later.")]
    public class SavePoint : Command
    {
        [Tooltip("A string key which uniquely identifies this save point. If you leave this empty then the parent Block's name will be used.")]
        [SerializeField] protected string savePointKey = "";

        [Tooltip("A short description of this save point. If you leave this empty then the current time and date will be used.")]
        [SerializeField] protected string savePointDescription;

        [Tooltip("Fire a Save Point Loaded event when this command executes.")]
        [SerializeField] protected bool fireEvent = true;

        [Tooltip("Resume execution from here after loading this Save Point.")]
        [SerializeField] protected bool resumeFromHere = true;

        protected virtual void Start()
        {
            // Each scene should have one Save Point with the 'Start' key.
            // We automatically start execution from this command whenever the scene starts 'normally' (i.e. first play, restart or scene load via the Load Scene command or SceneManager.LoadScene).
            // If on the other hand, the scene was loaded by loading a Save Point then the Save Manager handles resuming execution at the appropriate point, so we don't need to do anything here.

            if (string.Compare(SavePointKey, "Start", true) == 0 &&
                !FungusManager.Instance.SaveManager.HasLoadedSavePoint)
            {
                GetFlowchart().ExecuteBlock(ParentBlock, CommandIndex);
            }
        }

        #region Public members

        /// <summary>
        /// A string key which uniquely identifies this Save Point.
        /// If the save point key is empty then the parent Block's name is returned.
        /// </summary>
        public string SavePointKey 
        { 
            get 
            { 
                if (string.IsNullOrEmpty(savePointKey))
                {
                    return ParentBlock.BlockName;
                }
                return savePointKey; 
            } 
        }

        /// <summary>
        /// Gets the save point description.
        /// If the description is empty then the current time and date will be returned.
        /// </summary>
        public string SavePointDescription
        {
            get
            {
                if (string.IsNullOrEmpty(savePointDescription))
                {
                    return System.DateTime.UtcNow.ToString("HH:mm dd MMMM, yyyy");
                }
                return savePointDescription; 
            }
        }

        /// <summary>
        /// Resume execution from this command after loading a save point.
        /// </summary>
        public bool ResumeFromHere { get { return resumeFromHere; } }

        public override void OnEnter()
        {
            var saveManager = FungusManager.Instance.SaveManager;

            saveManager.AddSavePoint(SavePointKey, SavePointDescription);

            if (fireEvent)
            {
                SavePointLoaded.NotifyEventHandlers(SavePointKey);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (string.IsNullOrEmpty(savePointKey))
            {
                return "<Block Name>";
            }

            return savePointKey;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}
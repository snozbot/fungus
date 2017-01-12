// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections;

namespace Fungus
{
    [CommandInfo("Flow",
                 "Save Point", 
                 "Creates a Save Point and adds it to the Save History. The player can save the Save History to peristent storage and load it again later using the Save Menu.")]
    public class SavePoint : Command
    {
        /// <summary>
        /// Supported modes for specifying a Save Point Key.
        /// </summary>
        public enum KeyMode
        {
            /// <summary> Use the parent Block's name as the Save Point Key. N.B. If you change the Block name later it will break the save file!</summary>
            UseBlockName,
            /// <summary> Use a custom string for the key. This allows you to use multiple Save Points in the same block and save files will still work if the Block is renamed later. </summary>
            Custom
        }

        /// <summary>
        /// Supported modes for specifying a Save Point Description.
        /// </summary>
        public enum DescriptionMode
        {
            /// <summary> Uses the current date and time as the save point description.</summary>
            Timestamp,
            /// <summary> Use a custom string for the save point description.</summary>
            Custom
        }

        [Tooltip("How the Save Point Key for this Save Point is defined.")]
        [SerializeField] protected KeyMode keyMode = KeyMode.UseBlockName;

        [Tooltip("A string key which uniquely identifies this save point.")]
        [SerializeField] protected string customKey = "";

        [Tooltip("How the description for this Save Point is defined.")]
        [SerializeField] protected DescriptionMode descriptionMode = DescriptionMode.Timestamp;

        [Tooltip("A short description of this save point.")]
        [SerializeField] protected string customDescription;

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
        /// Gets a string key which uniquely identifies this Save Point.
        /// </summary>
        public string SavePointKey 
        { 
            get 
            { 
                if (keyMode == KeyMode.UseBlockName)
                {
                    return ParentBlock.BlockName;
                }
                else
                {
                    return customKey; 
                }
            } 
        }

        /// <summary>
        /// Gets the save point description.
        /// </summary>
        public string SavePointDescription
        {
            get
            {
                if (descriptionMode == DescriptionMode.Timestamp)
                {
                    return System.DateTime.UtcNow.ToString("HH:mm dd MMMM, yyyy");
                }
                else
                {
                    return customDescription; 
                }
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
            if (keyMode == KeyMode.UseBlockName)
            {
                return "key: <Block Name>";
            }

            return "key: " + customKey;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool IsPropertyVisible(string propertyName)
        {
            if (propertyName == "customKey" &&
                keyMode != KeyMode.Custom)
            {
                return false;
            }

            if (propertyName == "customDescription" &&
                descriptionMode != DescriptionMode.Custom)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}
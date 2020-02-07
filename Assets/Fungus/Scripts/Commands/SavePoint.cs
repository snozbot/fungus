// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

#if UNITY_5_3_OR_NEWER

using UnityEngine;

namespace Fungus
{
    [CommandInfo("Flow",
                 "Save Point", 
                 "Creates a Save Point and adds it to the Save History. The player can save the Save History to persistent storage and load it again later using the Save Menu.")]
    public class SavePoint : Command
    {
        /// <summary>
        /// Supported modes for specifying a Save Point Key.
        /// </summary>
        public enum KeyMode
        {
            /// <summary> Use the parent Block's name as the Save Point Key. N.B. If you change the Block name later it will break the save file!</summary>
            BlockName,
            /// <summary> Use a custom string for the key. This allows you to use multiple Save Points in the same block and save files will still work if the Block is renamed later. </summary>
            Custom,
            /// <summary> Use both the parent Block's name as well as a custom string for the Save Point key. This allows you to use your custom key every block, provided your Block names are unique./// </summary>
            BlockNameAndCustom
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

        [Tooltip("Marks this Save Point as the starting point for Flowchart execution in the scene. Each scene in your game should have exactly one Save Point with this enabled.")]
        [SerializeField] protected bool isStartPoint = false;

        [Tooltip("How the Save Point Key for this Save Point is defined.")]
        [SerializeField] protected KeyMode keyMode = KeyMode.BlockName;

        [Tooltip("A string key which uniquely identifies this save point.")]
        [SerializeField] protected string customKey = "";

        [Tooltip("A string to seperate the block name and custom key when using KeyMode.Both.")]
        [SerializeField]
        protected string keySeparator = "_";

        [Tooltip("How the description for this Save Point is defined.")]
        [SerializeField] protected DescriptionMode descriptionMode = DescriptionMode.Timestamp;

        [Tooltip("A short description of this save point.")]
        [SerializeField] protected string customDescription;

        [Tooltip("Fire a Save Point Loaded event when this command executes.")]
        [SerializeField] protected bool fireEvent = true;

        [Tooltip("Resume execution from this location after loading this Save Point.")]
        [SerializeField] protected bool resumeOnLoad = true;

        #region Public members

        /// <summary>
        /// Marks this Save Point as the starting point for Flowchart execution in the scene. Each scene in your game should have exactly one Save Point with this enabled.
        /// </summary>
        public bool IsStartPoint { get { return isStartPoint; } set { isStartPoint = value; } }

        /// <summary>
        /// Gets a string key which uniquely identifies this Save Point.
        /// </summary>
        public string SavePointKey 
        { 
            get 
            { 
                if (keyMode == KeyMode.BlockName)
                {
                    return ParentBlock.BlockName;
                }
                else if (keyMode == KeyMode.BlockNameAndCustom)
                {
                    return ParentBlock.BlockName + keySeparator + customKey;
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
        /// Resume execution from this location after loading this Save Point.
        /// </summary>
        public bool ResumeOnLoad { get { return resumeOnLoad; } }

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
            if (keyMode == KeyMode.BlockName)
            {
                return "key: " + ParentBlock.BlockName;
            }
            else if (keyMode == KeyMode.BlockNameAndCustom)
            {
                return "key: " + ParentBlock.BlockName + keySeparator + customKey;
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
                (keyMode != KeyMode.Custom && keyMode != KeyMode.BlockNameAndCustom))
            {
                return false;
            }

            if (propertyName == "keySeparator" &&
                keyMode != KeyMode.BlockNameAndCustom)
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

#endif
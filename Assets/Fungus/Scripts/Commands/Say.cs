// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Writes text in a dialog box.
    /// </summary>
    [CommandInfo("Narrative", 
                 "Say", 
                 "Writes text in a dialog box.")]
    [AddComponentMenu("")]
    public class Say : Command, ILocalizable
    {
        // Removed this tooltip as users's reported it obscures the text box
        [TextArea(5,10)]
        [SerializeField] protected string storyText = "";

        [Tooltip("Notes about this story text for other authors, localization, etc.")]
        [SerializeField] protected string description = "";

        [Tooltip("Character that is speaking")]
        [SerializeField] protected Character character;

        [Tooltip("Portrait that represents speaking character")]
        [SerializeField] protected Sprite portrait;

        [Tooltip("Voiceover audio to play when writing the text")]
        [SerializeField] protected AudioClip voiceOverClip;

        [Tooltip("Always show this Say text when the command is executed multiple times")]
        [SerializeField] protected bool showAlways = true;

        [Tooltip("Number of times to show this Say text when the command is executed multiple times")]
        [SerializeField] protected int showCount = 1;

        [Tooltip("Type this text in the previous dialog box.")]
        [SerializeField] protected bool extendPrevious = false;

        [Tooltip("Fade out the dialog box when writing has finished and not waiting for input.")]
        [SerializeField] protected bool fadeWhenDone = true;

        [Tooltip("Wait for player to click before continuing.")]
        [SerializeField] protected bool waitForClick = true;

        [Tooltip("Stop playing voiceover when text finishes writing.")]
        [SerializeField] protected bool stopVoiceover = true;

        [Tooltip("Wait for the Voice Over to complete before continuing")]
        [SerializeField] protected bool waitForVO = false;

        //add wait for vo that overrides stopvo

        [Tooltip("Sets the active Say dialog with a reference to a Say Dialog object in the scene. All story text will now display using this Say Dialog.")]
        [SerializeField] protected SayDialog setSayDialog;

        protected int executionCount;

        #region Public members

        /// <summary>
        /// Character that is speaking.
        /// </summary>
        public virtual Character _Character { get { return character; } }

        /// <summary>
        /// Portrait that represents speaking character.
        /// </summary>
        public virtual Sprite Portrait { get { return portrait; } set { portrait = value; } }

        /// <summary>
        /// Type this text in the previous dialog box.
        /// </summary>
        public virtual bool ExtendPrevious { get { return extendPrevious; } }

        /// <summary>
        /// A wait delay that applies after this command is done executing, and before the next command
        /// starts executing.
        /// </summary>
        public static float endDelay = 0f;

        protected virtual SayDialog ForDisplayingThis { get; set; } = null;

        protected virtual string DisplayText { get; set; } = null;

        public override void OnEnter()
        {
            if (this.ShouldBeSkipped())
            {
                Continue();
                return;
            }

            executionCount++;

            PrepareSayDialogToShowThis();
            PrepareDisplayText();

            ForDisplayingThis.Say(DisplayText, !extendPrevious, 
                waitForClick, fadeWhenDone, 
                stopVoiceover, waitForVO, 
                voiceOverClip, delegate { Continue(); });
        }

        bool ShouldBeSkipped()
        {
            return this.HasBeenShownEnoughAlready() && !this.HasSayDialogueToWorkWith();
        }

        bool HasSayDialogueToWorkWith()
        {
            return SayDialog.GetSayDialog() != null;
        }

        bool HasBeenShownEnoughAlready()
        {
            return !showAlways && executionCount >= showCount;
        }

        void PrepareSayDialogToShowThis()
        {
            OverrideActiveSayDialogAsNeeded();

            var sayDialog = SayDialog.GetSayDialog();

            sayDialog.SetActive(true);
            sayDialog.SetCharacter(character);
            sayDialog.SetCharacterImage(portrait);
        }

        void OverrideActiveSayDialogAsNeeded()
        {
            if (this.HasCharacterToDisplayFor())
            {
                SayDialog.ActiveSayDialog = character.SetSayDialog;
            }

            if (this.IsSetForSpecificSayDialog())
            {
                SayDialog.ActiveSayDialog = setSayDialog;
            }
        }

        bool HasCharacterToDisplayFor()
        {
            return character != null && character.SetSayDialog != null;
        }

        bool IsSetForSpecificSayDialog()
        {
            return setSayDialog != null;
        }

        void PrepareDisplayText()
        {
            DisplayText = storyText;
            AddEndDelayTagAsNeeded();
            HandleCustomTags();
            ApplyVariableSubstitution();
        }

        void AddEndDelayTagAsNeeded()
        {
            if (endDelay <= 0)
                return;

            string waitTag = string.Concat("{w=", endDelay,  "}");
            DisplayText = string.Concat(DisplayText, waitTag);
        }

        void HandleCustomTags()
        {
            var activeCustomTags = CustomTag.activeCustomTags;

            for (int i = 0; i < activeCustomTags.Count; i++)
            {
                var tag = activeCustomTags[i];
                DisplayText = DisplayText.Replace(tag.TagStartSymbol, tag.ReplaceTagStartWith);

                if (tag.TagEndSymbol != "" && tag.ReplaceTagEndWith != "")
                {
                    DisplayText = DisplayText.Replace(tag.TagEndSymbol, tag.ReplaceTagEndWith);
                }
            }
        }

        void ApplyVariableSubstitution()
        {
            var flowchart = GetFlowchart();
            DisplayText = flowchart.SubstituteVariables(DisplayText);
        }

        public override string GetSummary()
        {
            string namePrefix = "";
            if (character != null) 
            {
                namePrefix = character.NameText + ": ";
            }
            if (extendPrevious)
            {
                namePrefix = "EXTEND" + ": ";
            }
            return namePrefix + "\"" + storyText + "\"";
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override void OnReset()
        {
            executionCount = 0;
        }

        public override void OnStopExecuting()
        {
            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog == null)
            {
                return;
            }

            sayDialog.Stop();
        }

        #endregion

        #region ILocalizable implementation

        public virtual string GetStandardText()
        {
            return storyText;
        }

        public virtual void SetStandardText(string standardText)
        {
            storyText = standardText;
        }

        public virtual string GetDescription()
        {
            return description;
        }
        
        public virtual string GetStringId()
        {
            // String id for Say commands is SAY.<Localization Id>.<Command id>.[Character Name]
            string stringId = "SAY." + GetFlowchartLocalizationId() + "." + itemId + ".";
            if (character != null)
            {
                stringId += character.NameText;
            }

            return stringId;
        }

        #endregion
    }
}
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

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

        // character limits properties.
        [SerializeField] protected int storyMaxLength = 0;
        [SerializeField] protected string sayDialogName = string.Empty;
        
        protected int characterNameMaxLength = 0;

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

        public override void OnEnter()
        {
            if (!showAlways && executionCount >= showCount)
            {
                Continue();
                return;
            }

            executionCount++;

            // Override the active say dialog if needed
            if (character != null && character.SetSayDialog != null)
            {
                SayDialog.ActiveSayDialog = character.SetSayDialog;
            }

            if (setSayDialog != null)
            {
                SayDialog.ActiveSayDialog = setSayDialog;
            }

            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog == null)
            {
                Continue();
                return;
            }
    
            var flowchart = GetFlowchart();

            sayDialog.SetActive(true);

            //maintain max length for truncating if needed
            storyMaxLength = portrait != null ? sayDialog.portraitStoryTextLimit : sayDialog.storyTextLimit;

            sayDialog.SetCharacter(character); //sayDialog will truncate character name if it deems it necessary.
            sayDialog.SetCharacterImage(portrait);

            string displayText = storyText;

            var activeCustomTags = CustomTag.activeCustomTags;
            for (int i = 0; i < activeCustomTags.Count; i++)
            {
                var ct = activeCustomTags[i];
                displayText = displayText.Replace(ct.TagStartSymbol, ct.ReplaceTagStartWith);
                if (ct.TagEndSymbol != "" && ct.ReplaceTagEndWith != "")
                {
                    displayText = displayText.Replace(ct.TagEndSymbol, ct.ReplaceTagEndWith);
                }
            }

            string subbedText = flowchart.SubstituteVariables(displayText);

            // truncate story text if it's too long. Truncate using subbedText so markers don't count.
            sayDialog.Say(subbedText.Truncate(storyMaxLength, "<color=red><color=red><!></color></color>"), !extendPrevious, waitForClick, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, delegate {
                Continue();
            });
        }

        public override string GetSummary()
        {
            string namePrefix = "";
            string error = string.Empty;
            var flowchart = GetFlowchart();

            //get SayDialog data
            // for some reason, initialising sayDialog as null results in it getting the wrong dialog or being null, hence why GetActiveSayDialog() is called twice.
            // not sure if this is something weird, but not calling GetActiveSayDialog() here can result in weird behaviour.
            var sayDialog = SayDialog.GetActiveSayDialog();

            if (setSayDialog != null)
            {
                SayDialog.ActiveSayDialog = setSayDialog;
            }

            if (character != null) 
            {
                namePrefix = character.NameText + ": ";
                if (character.SetSayDialog != null) SayDialog.ActiveSayDialog = character.SetSayDialog;
            }
            if (extendPrevious)
            {
                namePrefix = "EXTEND" + ": ";
            }

            //reassign based on above changes. 
            sayDialog = SayDialog.GetActiveSayDialog();
            sayDialogName = sayDialog.name; //assign to be displayed in editor using SayEditor

            storyMaxLength = portrait != null ? sayDialog.portraitStoryTextLimit : sayDialog.storyTextLimit;
            characterNameMaxLength = sayDialog.characterNameLimit;

            string _storyText = flowchart.SubstituteVariables(storyText).SterilizeString();

            if (storyMaxLength != 0 && _storyText.Length > storyMaxLength)
            {
                error = "Error: Story text too large to fit in SayDialog: " + sayDialog.name + ", " + _storyText.Length + " / " + storyMaxLength;
            }

            //character name errors are a little more problematic, so any story errors will be overridden so the character name can be taken care of.
            if (character != null)
            {
                if (characterNameMaxLength != 0 && character.NameText.Length > characterNameMaxLength)
                {
                    error = "Error: Character name \"" + character.NameText.Truncate(characterNameMaxLength, "<color=red><!></color>") + "\" too large to fit in SayDialog: " + ", " + 
                            character.NameText.Length + " / " + characterNameMaxLength;
                }
            }

            if (error != string.Empty) return error;
            else return error + namePrefix + "\"" + _storyText.Truncate(92) + "\""; //truncate story text so it's easier to see.
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
// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
#if UNITY_LOCALIZATION
using UnityEngine.Localization;
#endif

namespace Fungus
{
    /// <summary>
    /// Displays a button in a multiple choice menu.
    /// </summary>
    [CommandInfo("Narrative", 
                 "Menu", 
                 "Displays a button in a multiple choice menu")]
    [AddComponentMenu("")]
    public class Menu : Command, ILocalizable, IBlockCaller
    {
#if UNITY_LOCALIZATION
        [Tooltip("Text to display on the menu button. Ignored if textString is not empty.")]
#else
        [Tooltip("Text to display on the menu button")]
#endif
        [TextArea()]
        [SerializeField] protected string text = "Option Text";
        
#if UNITY_LOCALIZATION
        [Tooltip("Localization entry for the menu button.")]
        [SerializeField] protected LocalizedString localizedText;
#endif
        
        [Tooltip("Notes about the option text for other authors, localization, etc.")]
        [SerializeField] protected string description = "";

        [FormerlySerializedAs("targetSequence")]
        [Tooltip("Block to execute when this option is selected")]
        [SerializeField] protected Block targetBlock;

        [Tooltip("Hide this option if the target block has been executed previously")]
        [SerializeField] protected bool hideIfVisited;

        [Tooltip("If false, the menu option will be displayed but will not be selectable")]
        [SerializeField] protected BooleanData interactable = new BooleanData(true);

        [Tooltip("A custom Menu Dialog to use to display this menu. All subsequent Menu commands will use this dialog.")]
        [SerializeField] protected MenuDialog setMenuDialog;

        [Tooltip("If true, this option will be passed to the Menu Dialogue but marked as hidden, this can be used to hide options while maintaining a Menu Shuffle.")]
        [SerializeField] protected BooleanData hideThisOption = new BooleanData(false);

        #region Public members

        public MenuDialog SetMenuDialog  { get { return setMenuDialog; } set { setMenuDialog = value; } }

        public override void OnEnter()
        {
            if (setMenuDialog != null)
            {
                // Override the active menu dialog
                MenuDialog.ActiveMenuDialog = setMenuDialog;
            }

            bool hideOption = (hideIfVisited && targetBlock != null && targetBlock.GetExecutionCount() > 0) || hideThisOption.Value;

            var menuDialog = MenuDialog.GetMenuDialog();
                if (menuDialog != null)
                {
                    menuDialog.SetActive(true);

                    var flowchart = GetFlowchart();
#if UNITY_LOCALIZATION
                    string displayText = flowchart.SubstituteVariables(localizedText.IsEmpty ? text : localizedText.GetLocalizedString());
#else
                    string displayText = flowchart.SubstituteVariables(text);
#endif
                    
                    menuDialog.AddOption(displayText, interactable, hideOption, targetBlock);
                }
            
            Continue();
        }

        public override void GetConnectedBlocks(ref List<Block> connectedBlocks)
        {
            if (targetBlock != null)
            {
                connectedBlocks.Add(targetBlock);
            }       
        }

        public override string GetSummary()
        {
            if (targetBlock == null)
            {
                return "Error: No target block selected";
            }

            if (text == "")
            {
                return "Error: No button text selected";
            }

            return text + " : " + targetBlock.BlockName;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return interactable.booleanRef == variable || hideThisOption.booleanRef == variable ||
                base.HasReference(variable);
        }

        public bool MayCallBlock(Block block)
        {
            return block == targetBlock;
        }

        #endregion

        #region ILocalizable implementation

        public virtual string GetStandardText()
        {
            return text;
        }

        public virtual void SetStandardText(string standardText)
        {
            text = standardText;
        }
        
        public virtual string GetDescription()
        {
            return description;
        }
        
        public virtual string GetStringId()
        {
            // String id for Menu commands is MENU.<Localization Id>.<Command id>
            return "MENU." + GetFlowchartLocalizationId() + "." + itemId;
        }
        
#if UNITY_LOCALIZATION

        public LocalizedString GetLocalizedStringComponent()
        {
            return localizedText;
        }
        
#endif

        #endregion

        #region Editor caches
#if UNITY_EDITOR
        protected override void RefreshVariableCache()
        {
            base.RefreshVariableCache();

            var f = GetFlowchart();

            f.DetermineSubstituteVariables(text, referencedVariables);
        }
#endif
        #endregion Editor caches
    }
}
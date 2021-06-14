// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

public enum ClickableCharacterState
{
    Enable,
    Disable
}
namespace Fungus
{
    /// <summary>
    /// Controls character's click property.
    /// </summary>
    [CommandInfo("Flow", 
                 "Cickable Character", 
                 "Sets character to be clickable")]
    public class ClickableCharacters : Command
    {
        [Tooltip("Enable/Disable clickable character")]
        [SerializeField] protected ClickableCharacterState activeState;

        [Tooltip("Character to display")]
        [SerializeField] protected Character character;

        [Tooltip("Set flowchart")]
        [SerializeField] protected Flowchart flowchart;

        [Tooltip("Execute block")]
        [SerializeField] protected Block executeBlock;

        #region Public members

        /// <summary>
        /// Character to display.
        /// </summary>
        public virtual Character _Character { get { return character; } set { character = value; } }

        public override void OnEnter()
        {
            if(character != null)
            {
                if(activeState == ClickableCharacterState.Enable && executeBlock != null)
                {
                    character.ClickableCharacter = true;
                    character.SetFlowchartForClickable = flowchart;
                    character.SetBlockForClickable = executeBlock.BlockName;
                }
                if(activeState == ClickableCharacterState.Disable)
                {
                    character.ClickableCharacter = false;
                    character.SetFlowchartForClickable = null;
                    character.SetBlockForClickable = string.Empty;
                }
            }

            Continue();
        }
        
        
        public override string GetSummary()
        {
            string chars = "";

            if(character != null)
            {
                chars = character.name;
            }
            else
            {
                chars = "Error : Character's can't be empty";
            }

            return chars;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(230, 200, 250, 255);
        }
        
        public override void OnCommandAdded(Block parentBlock)
        {
            //Default to display type: show
            activeState = ClickableCharacterState.Disable;
        }

        #endregion
    }
}
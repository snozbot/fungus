using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Groups together the parts of the PortraitStates that the save system needs to use
    /// to be able to restore portraits to what they should be upon load.
    /// </summary>
    [System.Serializable]
    public class PortraitSaveState : System.IEquatable<PortraitSaveState>
    {
        public string CharacterName
        {
            get { return characterName; }
            set { characterName = value; }
        }

        [SerializeField]
        string characterName = "[NullCharacterName]";

        public string StageName
        {
            get { return stageName; }
            set { stageName = value; }
        }

        [SerializeField]
        string stageName = "[NullStageName]";

        /// <summary>
        /// Whether or not the portrait should be hidden
        /// </summary>
        public bool OnScreen
        {
            get { return onScreen; }
            set { onScreen = value; }
        }

        [SerializeField]
        bool onScreen;

        public bool Dimmed
        {
            get { return dimmed; }
            set { dimmed = value; }
        }

        [SerializeField]
        bool dimmed;

        public string PositionName
        {
            get { return positionName; }
            set { positionName = value; }
        }

        [SerializeField]
        string positionName = "[NullPosition]";
        
        public FacingDirection FacingDirection
        {
            get { return facingDirection; }
            set { facingDirection = value; }
        }

        [SerializeField]
        FacingDirection facingDirection;

        public int PortraitIndex
        {
            get { return portraitIndex; }
            set { portraitIndex = value; }
        }

        [SerializeField]
        int portraitIndex = -1;

        public static PortraitSaveState From(Character character)
        {
            PortraitState charState = character.State;
            PortraitSaveState newState = new PortraitSaveState();

            newState.characterName = character.name;
            newState.Dimmed = charState.dimmed;
            newState.FacingDirection = charState.facing;
            newState.OnScreen = charState.onScreen;

            Image currentPortrait = charState.portraitImage;

            newState.PortraitIndex = charState.allPortraits.IndexOf(currentPortrait);
            if (charState.onScreen)
                newState.PositionName = charState.position.name;

            newState.stageName = FindStageNameFor(charState);

            return newState;
        }
            

        protected static string FindStageNameFor(PortraitState state)
        {
            // The Stage is the portrait holder's grandparent, so we need to hop up two spots
            // in the Hierarchy
            Transform portraitHolder = state.holder;
            bool thereIsNoHolder = portraitHolder == null;
            if (thereIsNoHolder)
                return "";

            Transform canvasHoldingTheHolder = portraitHolder.parent.transform;
            Stage stage = canvasHoldingTheHolder.parent.GetComponent<Stage>();
            bool thereIsNoStage = stage == null;
            if (thereIsNoStage)
                return "";
            else
                return stage.name;

        }

        public virtual bool Equals(PortraitSaveState other)
        {
            return this.CharacterName == other.CharacterName &&
                this.Dimmed == other.Dimmed &&
                this.FacingDirection == other.FacingDirection &&
                this.OnScreen == other.OnScreen &&
                this.PortraitIndex == other.PortraitIndex &&
                this.PositionName == other.PositionName &&
                this.StageName == other.StageName;
        }

    }
}
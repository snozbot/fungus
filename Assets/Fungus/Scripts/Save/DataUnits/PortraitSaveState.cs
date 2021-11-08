using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Groups together the parts of the PortraitStates that the save system needs to use
    /// to be able to restore portraits to what they should be upon load.
    /// </summary>
    [System.Serializable]
    public struct PortraitSaveState : System.IEquatable<PortraitSaveState>, ISaveUnit<PortraitSaveState>
    {
        public PortraitSaveState Contents => this;
        object ISaveUnit.Contents => this;

        public string CharacterName
        {
            get { return characterName; }
            set { characterName = value; }
        }

        [SerializeField]
        private string characterName;

        public string StageName
        {
            get { return stageName; }
            set { stageName = value; }
        }

        [SerializeField]
        private string stageName;

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
        private string positionName;
        
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
        private int portraitIndex;

        public string PortraitName
        {
            get { return portraitName; }
            set { portraitName = value; }
        }

        [SerializeField]
        private string portraitName;

        public static PortraitSaveState From(Character character)
        {
            PortraitSaveState newState = new PortraitSaveState();
            newState.SetFrom(character);

            return newState;
        }
        
        public PortraitSaveState(Character character)
        {
            PortraitState charState = character.State;

            this.characterName = character.name;
            this.dimmed = charState.dimmed;
            this.facingDirection = charState.facing;
            this.onScreen = charState.onScreen;

            Image currentPortrait = charState.portraitImage;

            this.portraitIndex = charState.allPortraits.IndexOf(currentPortrait);
            if (charState.onScreen)
            {
                this.positionName = charState.position.name;
                this.portraitName = charState.portrait.name;
            }
            else
            {
                this.positionName = "Null";
                this.portraitName = "Null";
            }

            this.stageName = FindStageNameFor(charState);
        }

        public void SetFrom(Character character)
        {
            PortraitState charState = character.State;

            this.CharacterName = character.name;
            this.Dimmed = charState.dimmed;
            this.FacingDirection = charState.facing;
            this.OnScreen = charState.onScreen;

            Image currentPortrait = charState.portraitImage;

            this.PortraitIndex = charState.allPortraits.IndexOf(currentPortrait);
            if (charState.onScreen)
            {
                this.positionName = charState.position.name;
                this.portraitName = charState.portrait.name;
            }
            else
            {
                this.PositionName = "Null";
                this.PortraitName = "Null";
            }

            this.StageName = FindStageNameFor(charState);
        }
            
        private static string FindStageNameFor(PortraitState state)
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

        public bool Equals(PortraitSaveState other)
        {
            return this.CharacterName == other.CharacterName &&
                this.Dimmed == other.Dimmed &&
                this.FacingDirection == other.FacingDirection &&
                this.OnScreen == other.OnScreen &&
                this.PortraitIndex == other.PortraitIndex &&
                this.PositionName == other.PositionName &&
                this.portraitName == other.PortraitName &&
                this.StageName == other.StageName;
        }

        public static PortraitSaveState Null = new PortraitSaveState();

    }
}
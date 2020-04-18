// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Controls a character portrait.
    /// </summary>
    [CommandInfo("Narrative", 
                 "Portrait", 
                 "Controls a character portrait.")]
    public class Portrait : ControlWithDisplay<DisplayType>
    {
        [Tooltip("Stage to display portrait on")]
        [SerializeField] protected Stage stage;

        [Tooltip("Character to display")]
        [SerializeField] protected Character character;

        [Tooltip("Character to swap with")]
        [SerializeField] protected Character replacedCharacter;

        [Tooltip("Portrait to display")]
        [SerializeField] protected Sprite portrait;

        [Tooltip("Move the portrait from/to this offset position")]
        [SerializeField] protected PositionOffset offset;

        [Tooltip("Move the portrait from this position")]
        [SerializeField] protected RectTransform fromPosition;

        [Tooltip("Move the portrait to this position")]
        [SerializeField] protected RectTransform toPosition;

        [Tooltip("Direction character is facing")]
        [SerializeField] protected FacingDirection facing;

        [Tooltip("Use Default Settings")]
        [SerializeField] protected bool useDefaultSettings = true;

        [Tooltip("Fade Duration")]
        [SerializeField] protected float fadeDuration = 0.5f;

        [Tooltip("Movement Duration")]
        [SerializeField] protected float moveDuration = 1f;

        [Tooltip("Shift Offset")]
        [SerializeField] protected Vector2 shiftOffset;

        [Tooltip("Move portrait into new position")]
        [SerializeField] protected bool move;

        [Tooltip("Start from offset position")]
        [SerializeField] protected bool shiftIntoPlace;

        [Tooltip("Wait until the tween has finished before executing the next command")]
        [SerializeField] protected bool waitUntilFinished = false;

        #region Public members

        /// <summary>
        /// Stage to display portrait on.
        /// </summary>
        public virtual Stage _Stage { get { return stage; } set { stage = value; } }

        /// <summary>
        /// Character to display.
        /// </summary>
        public virtual Character _Character { get { return character; } set { character = value; } }

        /// <summary>
        /// Portrait to display.
        /// </summary>
        public virtual Sprite _Portrait { get { return portrait; } set { portrait = value; } }

        /// <summary>
        /// Move the portrait from/to this offset position.
        /// </summary>
        public virtual PositionOffset Offset { get { return offset; } set { offset = value; } }

        /// <summary>
        /// Move the portrait from this position.
        /// </summary>
        public virtual RectTransform FromPosition { get { return fromPosition; } set { fromPosition = value;} }

        /// <summary>
        /// Move the portrait to this position.
        /// </summary>
        public virtual RectTransform ToPosition { get { return toPosition; } set { toPosition = value;} }

        /// <summary>
        /// Direction character is facing.
        /// </summary>
        public virtual FacingDirection Facing { get { return facing; } set { facing = value; } }

        /// <summary>
        /// Use Default Settings.
        /// </summary>
        public virtual bool UseDefaultSettings { get { return useDefaultSettings; } set { useDefaultSettings = value; } }

        /// <summary>
        /// Move portrait into new position.
        /// </summary>
        public virtual bool Move { get { return move; } set { move = value; } }

        /// <summary>
        /// Start from offset position.
        /// </summary>
        public virtual bool ShiftIntoPlace { get { return shiftIntoPlace; } set { shiftIntoPlace = value; } }

        public override void OnEnter()
        {
            // Selected "use default Portrait Stage"
            if (stage == null)
            {
                // If no default specified, try to get any portrait stage in the scene
                stage = Stage.GetActiveStage();
                // If portrait stage does not exist, do nothing
                if (stage == null)
                {
                    Continue();
                    return;
                }

            }

            // If no display specified, do nothing
            if (IsDisplayNone(display))
            {
                Continue();
                return;
            }

            PortraitOptions options = new PortraitOptions();
            
            options.character = character;
            options.replacedCharacter = replacedCharacter;
            options.portrait = portrait;
            options.display = display;
            options.offset = offset;
            options.fromPosition = fromPosition;
            options.toPosition = toPosition;
            options.facing = facing;
            options.useDefaultSettings = useDefaultSettings;
            options.fadeDuration = fadeDuration;
            options.moveDuration = moveDuration;
            options.shiftOffset = shiftOffset;
            options.move = move;
            options.shiftIntoPlace = shiftIntoPlace;
            options.waitUntilFinished = waitUntilFinished;

            stage.RunPortraitCommand(options, Continue);
            
        }
        
        public override string GetSummary()
        {
            if (display == DisplayType.None && character == null)
            {
                return "Error: No character or display selected";
            }
            else if (display == DisplayType.None)
            {
                return "Error: No display selected";
            }
            else if (character == null)
            {
                return "Error: No character selected";
            }

            string displaySummary = "";
            string characterSummary = "";
            string fromPositionSummary = "";
            string toPositionSummary = "";
            string stageSummary = "";
            string portraitSummary = "";
            string facingSummary = "";
            
            displaySummary = StringFormatter.SplitCamelCase(display.ToString());

            if (display == DisplayType.Replace)
            {
                if (replacedCharacter != null)
                {
                    displaySummary += " \"" + replacedCharacter.name + "\" with";
                }
            }

            characterSummary = character.name;
            if (stage != null)
            {
                stageSummary = " on \"" + stage.name + "\"";
            }
            
            if (portrait != null)
            {
                portraitSummary = " " + portrait.name;
            }

            if (shiftIntoPlace)
            {
                if (offset != 0)
                {
                    fromPositionSummary = offset.ToString();
                    fromPositionSummary = " from " + "\"" + fromPositionSummary + "\"";
                }
            }
            else if (fromPosition != null)
            {
                fromPositionSummary = " from " + "\"" + fromPosition.name + "\"";
            }

            if (toPosition != null)
            {
                string toPositionPrefixSummary = "";
                if (move)
                {
                    toPositionPrefixSummary = " to ";
                }
                else
                {
                    toPositionPrefixSummary = " at ";
                }

                toPositionSummary = toPositionPrefixSummary + "\"" + toPosition.name + "\"";
            }

            if (facing != FacingDirection.None)
            {
                if (facing == FacingDirection.Left)
                {
                    facingSummary = "<--";
                }
                if (facing == FacingDirection.Right)
                {
                    facingSummary = "-->";
                }

                facingSummary = " facing \"" + facingSummary + "\"";
            }

            return displaySummary + " \"" + characterSummary + portraitSummary + "\"" + stageSummary + facingSummary + fromPositionSummary + toPositionSummary;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(230, 200, 250, 255);
        }
        
        public override void OnCommandAdded(Block parentBlock)
        {
            //Default to display type: show
            display = DisplayType.Show;
        }

        #endregion
    }
}
// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using Fungus.Utils;

namespace Fungus.Commands
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
        public virtual Stage _Stage { get { return stage; } set { stage = value; } }

        [Tooltip("Character to display")]
        [SerializeField] protected Character character;
        public virtual Character _Character { get { return character; } set { character = value; } }

        [Tooltip("Character to swap with")]
        [SerializeField] protected Character replacedCharacter;
        
        [Tooltip("Portrait to display")]
        [SerializeField] protected Sprite portrait;
        public virtual Sprite _Portrait { get { return portrait; } set { portrait = value; } }

        [Tooltip("Move the portrait from/to this offset position")]
        [SerializeField] protected PositionOffset offset;
        public virtual PositionOffset Offset { get { return offset; } set { offset = value; } }

        [Tooltip("Move the portrait from this position")]
        [SerializeField] protected RectTransform fromPosition;
        public virtual RectTransform FromPosition { get { return fromPosition; } set { fromPosition = value;} }

        [Tooltip("Move the portrait to this positoin")]
        [SerializeField] protected RectTransform toPosition;
        public virtual RectTransform ToPosition { get { return toPosition; } set { toPosition = value;} }

        [Tooltip("Direction character is facing")]
        [SerializeField] protected FacingDirection facing;
        public virtual FacingDirection Facing { get { return facing; } set { facing = value; } }

        [Tooltip("Use Default Settings")]
        [SerializeField] protected bool useDefaultSettings = true;
        public virtual bool UseDefaultSettings { get { return useDefaultSettings; } set { useDefaultSettings = value; } }

        [Tooltip("Fade Duration")]
        [SerializeField] protected float fadeDuration = 0.5f;
        
        [Tooltip("Movement Duration")]
        [SerializeField] protected float moveDuration = 1f;
        
        [Tooltip("Shift Offset")]
        [SerializeField] protected Vector2 shiftOffset;
        
        [Tooltip("Move")]
        [SerializeField] protected bool move;
        public virtual bool Move { get { return move; } set { move = value; } }

        [Tooltip("Start from offset")]
        [SerializeField] protected bool shiftIntoPlace;
        public virtual bool ShiftIntoPlace { get { return shiftIntoPlace; } set { shiftIntoPlace = value; } }

        [Tooltip("Wait until the tween has finished before executing the next command")]
        [SerializeField] protected bool waitUntilFinished = false;

        public override void OnEnter()
        {
            // Selected "use default Portrait Stage"
            if (stage == null)
            {
                // If no default specified, try to get any portrait stage in the scene
                stage = FindObjectOfType<Stage>();
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
    }
}
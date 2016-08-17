/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
    
    [CommandInfo("Narrative", 
                 "Portrait", 
                 "Controls a character portrait. ")]
    public class Portrait : ControlWithDisplay<DisplayType>
    {
        [Tooltip("Stage to display portrait on")]
        public Stage stage;
        
        [Tooltip("Character to display")]
        public Character character;
        
        [Tooltip("Character to swap with")]
        public Character replacedCharacter;
        
        [Tooltip("Portrait to display")]
        public Sprite portrait;
        
        [Tooltip("Move the portrait from/to this offset position")]
        public PositionOffset offset;
        
        [Tooltip("Move the portrait from this position")]
        public RectTransform fromPosition;

        [Tooltip("Move the portrait to this positoin")]
        public RectTransform toPosition;

        [Tooltip("Direction character is facing")]
        public FacingDirection facing;
        
        [Tooltip("Use Default Settings")]
        public bool useDefaultSettings = true;
        
        [Tooltip("Fade Duration")]
        public float fadeDuration = 0.5f;
        
        [Tooltip("Movement Duration")]
        public float moveDuration = 1f;
        
        [Tooltip("Shift Offset")]
        public Vector2 shiftOffset;
        
        [Tooltip("Move")]
        public bool move;
        
        [Tooltip("Start from offset")]
        public bool shiftIntoPlace;
        
        [Tooltip("Wait until the tween has finished before executing the next command")]
        public bool waitUntilFinished = false;

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
/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Fungus
{
    [CommandInfo("Sprite",
                 "Set Draggable 2D",
                 "Sets a Draggable2D component to be draggable / non-draggable.")]
    [AddComponentMenu("")]
    public class SetDraggable2D : Command
    {      
        [Tooltip("Reference to Draggable2D component on a gameobject")]
        public Draggable2D targetDraggable2D;

        [Tooltip("Set to true to enable the component")]
        public BooleanData activeState;

        public override void OnEnter() 
        {
            if (targetDraggable2D != null)         
            {
                targetDraggable2D.dragEnabled = activeState.Value;     
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (targetDraggable2D == null)         
            {
                return "Error: No Draggable2D component selected";     
            }

            return targetDraggable2D.gameObject.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }

}
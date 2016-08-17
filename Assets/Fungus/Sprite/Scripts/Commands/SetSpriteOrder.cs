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
                 "Set Sprite Order", 
                 "Controls the render order of sprites by setting the Order In Layer property of a list of sprites.")]
    [AddComponentMenu("")]
    public class SetSpriteOrder : Command 
    {
        [Tooltip("List of sprites to set the order in layer property on")]
        public List<SpriteRenderer> targetSprites = new List<SpriteRenderer>();

        [Tooltip("The order in layer value to set on the target sprites")]
        public IntegerData orderInLayer;

        public override void OnEnter()
        {
            foreach (SpriteRenderer spriteRenderer in targetSprites)
            {
                spriteRenderer.sortingOrder = orderInLayer;
            }

            Continue();
        }
        
        public override string GetSummary()
        {
            string summary = "";
            foreach (SpriteRenderer spriteRenderer in targetSprites)
            {
                if (spriteRenderer == null)
                {
                    continue;
                }

                if (summary.Length > 0)
                {
                    summary += ", ";
                }

                summary += spriteRenderer.name;
            }

            if (summary.Length == 0)
            {
                return "Error: No cursor sprite selected";
            }

            return summary + " = " + orderInLayer.Value;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool IsReorderableArray(string propertyName)
        {
            if (propertyName == "targetSprites")
            {
                return true;
            }

            return false;
        }

        public override void OnCommandAdded(Block parentBlock)
        {
            // Add a default empty entry
            targetSprites.Add(null);
        }
    }
}

// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Controls the render order of sprites by setting the Order In Layer property of a list of sprites.
    /// </summary>
    [CommandInfo("Sprite", 
                 "Set Sprite Order", 
                 "Controls the render order of sprites by setting the Order In Layer property of a list of sprites.")]
    [AddComponentMenu("")]
    public class SetSpriteOrder : Command 
    {
        [Tooltip("List of sprites to set the order in layer property on")]
        [SerializeField] protected List<SpriteRenderer> targetSprites = new List<SpriteRenderer>();

        [Tooltip("The order in layer value to set on the target sprites")]
        [SerializeField] protected IntegerData orderInLayer;

        #region Public members

        public override void OnEnter()
        {
            for (int i = 0; i < targetSprites.Count; i++)
            {
                var spriteRenderer = targetSprites[i];
                spriteRenderer.sortingOrder = orderInLayer;
            }

            Continue();
        }
        
        public override string GetSummary()
        {
            string summary = "";
            for (int i = 0; i < targetSprites.Count; i++)
            {
                var spriteRenderer = targetSprites[i];
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

        #endregion
    }
}

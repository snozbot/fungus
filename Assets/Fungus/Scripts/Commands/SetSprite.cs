// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

// Snippet added by ducksonthewater, 2019-01-03 - www.ducks-on-the-water.com

using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Changes the sprite on a SpriteRenderer.
    /// </summary>
    [CommandInfo("Sprite", 
                 "Set Sprite", 
                 "Changes the sprite of a SpriteRenderer.")]
    [AddComponentMenu("")]
    public class SetSprite : Command 
    {
        [Tooltip("List of sprites to set the sprite property on")]
        [SerializeField] protected List<SpriteRenderer> targetSprites = new List<SpriteRenderer>();

        [Tooltip("The sprite set on the target sprites")]
        [SerializeField] protected Sprite spriteRendererSprite;

        #region Public members

        public override void OnEnter()
        {
            for (int i = 0; i < targetSprites.Count; i++)
            {
                var spriteRenderer = targetSprites[i];
                spriteRenderer.sprite = spriteRendererSprite;
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

            return summary + " = " + spriteRendererSprite;
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

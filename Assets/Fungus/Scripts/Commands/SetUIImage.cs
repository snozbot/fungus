// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// Changes the Image on a UI-element.
    /// </summary>
    [CommandInfo("UI", 
                 "Set UI-Image", 
                 "Changes the Image of a UI-Element.")]
    [AddComponentMenu("")]
    public class SetUIImage : Command 
    {
        [Tooltip("List of UI-objects to set the image property on")]
        [SerializeField] protected List<Image> targetImages = new List<Image>();

        [Tooltip("The sprite set on the target Image")]
        [SerializeField] protected Sprite imageSprite;

        #region Public members

        public override void OnEnter()
        {
            for (int i = 0; i < targetImages.Count; i++)
            {
                var targetImage = targetImages[i];
                targetImage.sprite = imageSprite;
            }

            Continue();
        }
        
        public override string GetSummary()
        {
            string summary = "";
            for (int i = 0; i < targetImages.Count; i++)
            {
                var targetImage = targetImages[i];
                if (targetImage == null)
                {
                    continue;
                }
                if (summary.Length > 0)
                {
                    summary += ", ";
                }
                summary += targetImage.name;
            }

            if (summary.Length == 0)
            {
                return "Error: No cursor sprite selected";
            }

            return summary + " = " + imageSprite;
        }
        
        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool IsReorderableArray(string propertyName)
        {
            if (propertyName == "targetImages")
            {
                return true;
            }

            return false;
        }

        public override void OnCommandAdded(Block parentBlock)
        {
            // Add a default empty entry
            targetImages.Add(null);
        }

        #endregion
    }
}

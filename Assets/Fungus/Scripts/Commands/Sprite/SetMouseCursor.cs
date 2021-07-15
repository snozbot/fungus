// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets the mouse cursor sprite.
    /// </summary>
    [CommandInfo("Sprite", 
                 "Set Mouse Cursor", 
                 "Sets the mouse cursor sprite.")]
    [AddComponentMenu("")]
    public class SetMouseCursor : Command 
    {
        [Tooltip("Texture to use for cursor. Will use default mouse cursor if no sprite is specified")]
        [SerializeField] protected Texture2D cursorTexture;

        [Tooltip("The offset from the top left of the texture to use as the target point")]
        [SerializeField] protected Vector2 hotSpot;

        // Cached static cursor settings
        protected static Texture2D activeCursorTexture;
        protected static Vector2 activeHotspot;

        #region Public members

        public static void ResetMouseCursor()
        {
            // Change mouse cursor back to most recent settings
            Cursor.SetCursor(activeCursorTexture, activeHotspot, CursorMode.Auto);
        }

        public override void OnEnter()
        {
            Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);

            activeCursorTexture = cursorTexture;
            activeHotspot = hotSpot;

            Continue();
        }

        public override string GetSummary()
        {
            if (cursorTexture == null)
            {
                return "Error: No cursor sprite selected";
            }

            return cursorTexture.name;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        #endregion
    }
}
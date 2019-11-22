// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;
using MoonSharp.Interpreter;

namespace Fungus
{
    /// <summary>
    /// Contains all options to run a portrait command.
    /// </summary>
    public class PortraitOptions
    {
        public Character character;
        public Character replacedCharacter;
        public Sprite portrait;
        public DisplayType display;
        public PositionOffset offset;
        public RectTransform fromPosition;
        public RectTransform toPosition;
        public FacingDirection facing;
        public bool useDefaultSettings;
        public float fadeDuration;
        public float moveDuration;
        public Vector2 shiftOffset;
        public bool move; //sets to position to be the same as from
        public bool shiftIntoPlace;
        public bool waitUntilFinished;
        public System.Action onComplete;

        public PortraitOptions(bool useDefaultSettings = true)
        {
            character = null;
            replacedCharacter = null;
            portrait = null;
            display = DisplayType.None;
            offset = PositionOffset.None;
            fromPosition = null;
            toPosition = null;
            facing = FacingDirection.None;
            shiftOffset = new Vector2(0, 0);
            move = false;
            shiftIntoPlace = false;
            waitUntilFinished = false;
            onComplete = null;

            // Special values that can be overridden
            fadeDuration = 0.5f;
            moveDuration = 1f;
            this.useDefaultSettings = useDefaultSettings;
        }
    }

    /// <summary>
    /// Represents the current state of a character portrait on the stage.
    /// </summary>
    public class PortraitState
    {
        public bool onScreen;
        public bool dimmed;
        public DisplayType display;
        public Sprite portrait;
        public RectTransform position;
        public FacingDirection facing;
        public Image portraitImage;
    }

    /// <summary>
    /// Util functions for working with portraits.
    /// </summary>
    public static class PortraitUtil 
    {
        #region Public members

        /// <summary>
        /// Convert a Moonsharp table to portrait options
        /// If the table returns a null for any of the parameters, it should keep the defaults
        /// </summary>
        /// <param name="table">Moonsharp Table</param>
        /// <param name="stage">Stage</param>
        /// <returns></returns>
        public static PortraitOptions ConvertTableToPortraitOptions(Table table, Stage stage)
        {
            PortraitOptions options = new PortraitOptions(true);

            // If the table supplies a nil, keep the default
            options.character = table.Get("character").ToObject<Character>() 
                ?? options.character;

            options.replacedCharacter = table.Get("replacedCharacter").ToObject<Character>()
                ?? options.replacedCharacter;

            if (!table.Get("portrait").IsNil())
            {
                options.portrait = options.character.GetPortrait(table.Get("portrait").CastToString());
            }

            if (!table.Get("display").IsNil())
            {
                options.display = table.Get("display").ToObject<DisplayType>();
            }

            if (!table.Get("offset").IsNil())
            {
                options.offset = table.Get("offset").ToObject<PositionOffset>();
            }

            if (!table.Get("fromPosition").IsNil())
            {
                options.fromPosition = stage.GetPosition(table.Get("fromPosition").CastToString());
            }

            if (!table.Get("toPosition").IsNil())
            {
                options.toPosition = stage.GetPosition(table.Get("toPosition").CastToString());
            }

            if (!table.Get("facing").IsNil())
            {
                var facingDirection = FacingDirection.None;
                DynValue v = table.Get("facing");
                if (v.Type == DataType.String)
                {
                    if (string.Compare(v.String, "left", true) == 0)
                    {
                        facingDirection = FacingDirection.Left;
                    }
                    else if (string.Compare(v.String, "right", true) == 0)
                    {
                        facingDirection = FacingDirection.Right;
                    }
                }
                else
                {
                    facingDirection = table.Get("facing").ToObject<FacingDirection>();
                }

                options.facing = facingDirection;
            }

            if (!table.Get("useDefaultSettings").IsNil())
            {
                options.useDefaultSettings = table.Get("useDefaultSettings").CastToBool();
            }

            if (!table.Get("fadeDuration").IsNil())
            {
                options.fadeDuration = table.Get("fadeDuration").ToObject<float>();
            }

            if (!table.Get("moveDuration").IsNil())
            {
                options.moveDuration = table.Get("moveDuration").ToObject<float>();
            }

            if (!table.Get("move").IsNil())
            {
                options.move = table.Get("move").CastToBool();
            }
            else if (options.fromPosition != options.toPosition)
            {
                options.move = true;
            }

            if (!table.Get("shiftIntoPlace").IsNil())
            {
                options.shiftIntoPlace = table.Get("shiftIntoPlace").CastToBool();
            }

            if (!table.Get("waitUntilFinished").IsNil())
            {
                options.waitUntilFinished = table.Get("waitUntilFinished").CastToBool();
            }

            return options;
        }

        #endregion
    }
}

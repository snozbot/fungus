// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using System.Collections.Generic;
using Fungus.Utils;

namespace Fungus
{
    /// <summary>
    /// A Character that can be used in dialogue via the Say, Conversation and Portrait commands.
    /// </summary>
    public interface ICharacter
    {
        /// <summary>
        /// Character name as displayed in Say Dialog.
        /// </summary>
        string NameText { get; }

        /// <summary>
        /// Color to display the character name in Say Dialog.
        /// </summary>
        Color NameColor { get; }

        /// <summary>
        /// Sound effect to play when this character is speaking.
        /// </summary>
        /// <value>The sound effect.</value>
        AudioClip SoundEffect { get; }

        /// <summary>
        /// List of portrait images that can be displayed for this character.
        /// </summary>
        List<Sprite> Portraits { get; }

        /// <summary>
        /// Direction that portrait sprites face.
        /// </summary>
        FacingDirection PortraitsFace { get; }

        /// <summary>
        /// Currently display profile sprite for this character.
        /// </summary>
        /// <value>The profile sprite.</value>
        Sprite ProfileSprite { get; set; }

        /// <summary>
        /// Current display state of this character's portrait.
        /// </summary>
        /// <value>The state.</value>
        PortraitState State { get; }

        /// <summary>
        /// Sets the active Say dialog with a reference to a Say Dialog object in the scene. This Say Dialog will be used whenever the character speaks.
        /// </summary>
        ISayDialog SetSayDialog { get; }

        /// <summary>
        /// Returns the name of the game object.
        /// </summary>
        string GetObjectName();

        /// <summary>
        /// Returns true if the character name starts with the specified string. Case insensitive.
        /// </summary>
        bool NameStartsWith(string matchString);

        /// <summary>
        /// Looks for a portrait by name on a character
        /// If none is found, give a warning and return a blank sprite
        /// </summary>
        Sprite GetPortrait(string portraitString);
    }
}
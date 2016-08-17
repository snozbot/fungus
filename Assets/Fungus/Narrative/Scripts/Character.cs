/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Fungus
{

    [ExecuteInEditMode]
    public class Character : MonoBehaviour, ILocalizable
    {
        public string nameText; // We need a separate name as the object name is used for character variations (e.g. "Smurf Happy", "Smurf Sad")
        public Color nameColor = Color.white;
        public AudioClip soundEffect;
        public Sprite profileSprite;
        public List<Sprite> portraits;
        public FacingDirection portraitsFace;
        public PortraitState state = new PortraitState();

        [Tooltip("Sets the active Say dialog with a reference to a Say Dialog object in the scene. All story text will now display using this Say Dialog.")]
        public SayDialog setSayDialog;

        [FormerlySerializedAs("notes")]
        [TextArea(5,10)]
        public string description;

        static public List<Character> activeCharacters = new List<Character>();

        protected virtual void OnEnable()
        {
            if (!activeCharacters.Contains(this))
            {
                activeCharacters.Add(this);
            }
        }

        protected virtual void OnDisable()
        {
            activeCharacters.Remove(this);
        }

        //
        // ILocalizable implementation
        //

        public virtual string GetStandardText()
        {
            return nameText;
        }

        public virtual void SetStandardText(string standardText)
        {
            nameText = standardText;
        }

        public bool NameStartsWith(string matchString)
        {
            return name.StartsWith(matchString, true, System.Globalization.CultureInfo.CurrentCulture)
                || nameText.StartsWith(matchString, true, System.Globalization.CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Looks for a portrait by name on a character
        /// If none is found, give a warning and return a blank sprite
        /// </summary>
        /// <param name="portrait_string"></param>
        /// <returns>Character portrait sprite</returns>
        public virtual Sprite GetPortrait(string portrait_string)
        {
            if (String.IsNullOrEmpty(portrait_string))
            {
                return null;
            }

            for (int i = 0; i < portraits.Count; i++)
            {
                if (portraits[i] != null && String.Compare(portraits[i].name, portrait_string, true) == 0)
                {
                    return portraits[i];
                }
            }
            return null;
        }

        public virtual string GetDescription()
        {
            return description;
        }

        public virtual string GetStringId()
        {
            // String id for character names is CHARACTER.<Character Name>
            return "CHARACTER." + nameText;
        }
    }

}

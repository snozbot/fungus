// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;
using System;
using Fungus.Utils;

namespace Fungus
{
    /// <summary>
    /// A Character that can be used in dialogue via the Say, Conversation and Portrait commands.
    /// </summary>
    [ExecuteInEditMode]
    public class Character : MonoBehaviour, ICharacter, ILocalizable
    {
        [Tooltip("Character name as displayed in Say Dialog.")]
        [SerializeField] protected string nameText; // We need a separate name as the object name is used for character variations (e.g. "Smurf Happy", "Smurf Sad")

        [Tooltip("Color to display the character name in Say Dialog.")]
        [SerializeField] protected Color nameColor = Color.white;

        [Tooltip("Sound effect to play when this character is speaking.")]
        [SerializeField] protected AudioClip soundEffect;

        [Tooltip("List of portrait images that can be displayed for this character.")]
        [SerializeField] protected List<Sprite> portraits;

        [Tooltip("Direction that portrait sprites face.")]
        [SerializeField] protected FacingDirection portraitsFace;

        [Tooltip("Sets the active Say dialog with a reference to a Say Dialog object in the scene. This Say Dialog will be used whenever the character speaks.")]
        [SerializeField] protected SayDialog setSayDialog;

        [FormerlySerializedAs("notes")]
        [TextArea(5,10)]
        [SerializeField] protected string description;

        protected PortraitState portaitState = new PortraitState();

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

        #region ICharacter implementation

        public virtual string NameText { get { return nameText; } }

        public virtual Color NameColor { get { return nameColor; } }

        public virtual AudioClip SoundEffect { get { return soundEffect; } }

        public virtual Sprite ProfileSprite { get; set; }

        public virtual List<Sprite> Portraits { get { return portraits; } }

        public virtual FacingDirection PortraitsFace { get { return portraitsFace; } }

        public virtual PortraitState State { get { return portaitState; } }

        public virtual ISayDialog SetSayDialog { get { return setSayDialog; } }

        public string GetObjectName() { return gameObject.name; }

        public virtual bool NameStartsWith(string matchString)
        {
            return name.StartsWith(matchString, true, System.Globalization.CultureInfo.CurrentCulture)
                || nameText.StartsWith(matchString, true, System.Globalization.CultureInfo.CurrentCulture);
        }

        public virtual Sprite GetPortrait(string portraitString)
        {
            if (String.IsNullOrEmpty(portraitString))
            {
                return null;
            }

            for (int i = 0; i < portraits.Count; i++)
            {
                if (portraits[i] != null && String.Compare(portraits[i].name, portraitString, true) == 0)
                {
                    return portraits[i];
                }
            }
            return null;
        }

        #endregion

        #region ILocalizable implementation

        public virtual string GetStandardText()
        {
            return nameText;
        }

        public virtual void SetStandardText(string standardText)
        {
            nameText = standardText;
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

        #endregion
    }
}

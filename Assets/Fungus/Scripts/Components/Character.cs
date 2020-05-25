// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// A Character that can be used in dialogue via the Say, Conversation and Portrait commands.
    /// </summary>
    [ExecuteInEditMode]
    public class Character : MonoBehaviour, ILocalizable, IComparer<Character>
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

        protected static List<Character> activeCharacters = new List<Character>();

        protected virtual void OnEnable()
        {
            if (!activeCharacters.Contains(this))
            {
                activeCharacters.Add(this);
                activeCharacters.Sort(this);
            }
        }

        protected virtual void OnDisable()
        {
            activeCharacters.Remove(this);
        }

        #region Public members

        /// <summary>
        /// Gets the list of active characters.
        /// </summary>
        public static List<Character> ActiveCharacters { get { return activeCharacters; } }

        /// <summary>
        /// Character name as displayed in Say Dialog.
        /// </summary>
        public virtual string NameText { get { return nameText; } }

        /// <summary>
        /// Color to display the character name in Say Dialog.
        /// </summary>
        public virtual Color NameColor { get { return nameColor; } }

        /// <summary>
        /// Sound effect to play when this character is speaking.
        /// </summary>
        /// <value>The sound effect.</value>
        public virtual AudioClip SoundEffect { get { return soundEffect; } }

        /// <summary>
        /// List of portrait images that can be displayed for this character.
        /// </summary>
        public virtual List<Sprite> Portraits { get { return portraits; } }

        /// <summary>
        /// Direction that portrait sprites face.
        /// </summary>
        public virtual FacingDirection PortraitsFace { get { return portraitsFace; } }

        /// <summary>
        /// Currently display profile sprite for this character.
        /// </summary>
        /// <value>The profile sprite.</value>
        public virtual Sprite ProfileSprite { get; set; }

        /// <summary>
        /// Current display state of this character's portrait.
        /// </summary>
        /// <value>The state.</value>
        public virtual PortraitState State { get { return portaitState; } }

        /// <summary>
        /// Sets the active Say dialog with a reference to a Say Dialog object in the scene. This Say Dialog will be used whenever the character speaks.
        /// </summary>
        public virtual SayDialog SetSayDialog { get { return setSayDialog; } }

        /// <summary>
        /// Returns the name of the game object.
        /// </summary>
        public string GetObjectName() { return gameObject.name; }

        /// <summary>
        /// Returns true if the character name starts with the specified string. Case insensitive.
        /// </summary>
        public virtual bool NameStartsWith(string matchString)
        {
#if NETFX_CORE
            return name.StartsWith(matchString, StringComparison.CurrentCultureIgnoreCase)
                || nameText.StartsWith(matchString, StringComparison.CurrentCultureIgnoreCase);
#else
            return name.StartsWith(matchString, true, System.Globalization.CultureInfo.CurrentCulture)
                || nameText.StartsWith(matchString, true, System.Globalization.CultureInfo.CurrentCulture);
#endif
        }
        
        public int Compare(Character x, Character y)
        {
            if (x == y)
                return 0;
            if (y == null)
                return 1;
            if (x == null)
                return -1;

            return x.name.CompareTo(y.name);
        }

        /// <summary>
        /// Looks for a portrait by name on a character
        /// If none is found, give a warning and return a blank sprite
        /// </summary>
        public virtual Sprite GetPortrait(string portraitString)
        {
            if (string.IsNullOrEmpty(portraitString))
            {
                return null;
            }

            for (int i = 0; i < portraits.Count; i++)
            {
                if (portraits[i] != null && string.Compare(portraits[i].name, portraitString, true) == 0)
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

        protected virtual void OnValidate()
        {
            if (portraits != null && portraits.Count > 1)
            {
                portraits.Sort(PortraitUtil.PortraitCompareTo);
            }
        }
    }
}

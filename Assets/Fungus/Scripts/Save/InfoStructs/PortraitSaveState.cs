using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Groups together the parts of the PortraitStates that the save system needs to use
    /// to be able to restore portraits to what they should be upon load.
    /// </summary>
    [System.Serializable]
    public class PortraitSaveState
    {
        public string CharacterName
        {
            get { return characterName; }
            set { characterName = value; }
        }

        [SerializeField]
        string characterName = "[NullCharacterName]";

        public string StageName
        {
            get { return stageName; }
            set { stageName = value; }
        }

        [SerializeField]
        string stageName = "[NullStageName]";

        public bool OnScreen
        {
            get { return onScreen; }
            set { onScreen = value; }
        }

        [SerializeField]
        bool onScreen;

        public bool Dimmed
        {
            get { return dimmed; }
            set { dimmed = value; }
        }

        [SerializeField]
        bool dimmed;

        public Vector3 Position
        {
            get { return position; }
            set { position.Set(value.x, value.y, value.z); }
        }

        [SerializeField]
        Vector3 position;
        
        public FacingDirection FacingDirection
        {
            get { return facingDirection; }
            set { facingDirection = value; }
        }

        [SerializeField]
        FacingDirection facingDirection;

        public string PortraitName
        {
            get { return portraitName; }
            set { portraitName = value; }
        }

        [SerializeField]
        string portraitName = "[NullPortraitName]"; 

    }
}
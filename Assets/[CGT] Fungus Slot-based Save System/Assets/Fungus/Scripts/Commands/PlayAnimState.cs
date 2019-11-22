// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Plays a state of an animator according to the state name.
    /// </summary>
    [CommandInfo("Animation", 
                 "Play Anim State", 
                 "Plays a state of an animator according to the state name")]
    [AddComponentMenu("")]
    public class PlayAnimState : Command 
    {
        [Tooltip("Reference to an Animator component in a game object")]
        [SerializeField] protected AnimatorData animator = new AnimatorData();

        [Tooltip("Name of the state you want to play")]
        [SerializeField] protected StringData stateName = new StringData();

        [Tooltip("Layer to play animation on")]
        [SerializeField] protected IntegerData layer = new IntegerData(-1);

        [Tooltip("Start time of animation")]
        [SerializeField] protected FloatData time = new FloatData(0f);

        #region Public members

        public override void OnEnter()
        {
            if (animator.Value != null)
            {
                animator.Value.Play(stateName.Value, layer.Value, time.Value);
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (animator.Value == null)
            {
                return "Error: No animator selected";
            }

            return animator.Value.name + " (" + stateName.Value + ")";
        }

        public override Color GetButtonColor()
        {
            return new Color32(170, 204, 169, 255);
        }

        #endregion
    }    
}


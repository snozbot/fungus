// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the desired OnAnimator* message for the monobehaviour is received.
    /// </summary>
    [EventHandlerInfo("MonoBehaviour",
                      "Animator",
                      "The block will execute when the desired OnAnimator* message for the monobehaviour is received.")]
    [AddComponentMenu("")]
    public class AnimatorState : EventHandler
    {

        [System.Flags]
        public enum AnimatorMessageFlags
        {
            OnAnimatorIK = 1 << 0,
            OnAnimatorMove = 1 << 1,
        }

        [Tooltip("Which of the OnAnimator messages to trigger on.")]
        [SerializeField]
        [EnumFlag]
        protected AnimatorMessageFlags FireOn = AnimatorMessageFlags.OnAnimatorMove;

        [Tooltip("IK layer to trigger on. Negative is all.")]
        [SerializeField]
        protected int IKLayer = 1;
        
        private void OnAnimatorIK(int layer)
        {
            if ((FireOn & AnimatorMessageFlags.OnAnimatorIK) != 0 && 
                (IKLayer == layer || IKLayer < 0) )
            {
                ExecuteBlock();
            }
        }

        private void OnAnimatorMove()
        {
            if ((FireOn & AnimatorMessageFlags.OnAnimatorMove) != 0)
            {
                ExecuteBlock();
            }
        }
    }
}

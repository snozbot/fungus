// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when tag filtered OnControllerColliderHit is received.
    /// </summary>
    [EventHandlerInfo("MonoBehaviour",
                      "CharacterCollider",
                      "The block will execute when tag filtered OnCharacterColliderHit is received")]
    [AddComponentMenu("")]
    public class CharacterControllerCollide : TagFilteredEventHandler
    {
        [Tooltip("Optional variable to store the ControllerColliderHit")]
        [VariableProperty(typeof(ControllerColliderHitVariable))]
        [SerializeField] protected ControllerColliderHitVariable colHitVar;

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (DoesPassFilter(hit.gameObject.tag))
            {
                if (colHitVar != null)
                {
                    colHitVar.Value = hit;
                }

                ExecuteBlock();
            }
        }
    }
}
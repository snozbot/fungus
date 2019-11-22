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

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            ProcessTagFilter(hit.gameObject.tag);
        }
    }
}

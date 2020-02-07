// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{

    /// <summary>
    /// The block will execute when a 3d physics collision matching some basic conditions is met 
    /// </summary>
    [EventHandlerInfo("MonoBehaviour",
                      "Collision",
                      "The block will execute when a 3d physics collision matching some basic conditions is met.")]
    [AddComponentMenu("")]
    public class Collision : BasePhysicsEventHandler
    {
        [Tooltip("Optional variable to store the collision object that is provided by Unity.")]
        [VariableProperty(typeof(CollisionVariable))]
        [SerializeField] protected CollisionVariable collisionVar;

        private void OnCollisionEnter(UnityEngine.Collision collision)
        {
            ProcessCollisionEvent(PhysicsMessageType.Enter, collision);
        }

        private void OnCollisionStay(UnityEngine.Collision collision)
        {
            ProcessCollisionEvent(PhysicsMessageType.Stay, collision);
        }

        private void OnCollisionExit(UnityEngine.Collision collision)
        {
            ProcessCollisionEvent(PhysicsMessageType.Exit, collision);
        }

        private void ProcessCollisionEvent(PhysicsMessageType from, UnityEngine.Collision collision)
        {
            if ((from & FireOn) != 0 &&
                DoesPassFilter(collision.collider.tag))
            {
                if (collisionVar != null)
                    collisionVar.Value = collision;

                ExecuteBlock();
            }
        }
    }
}
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
        private void OnCollisionEnter(UnityEngine.Collision collision)
        {
            ProcessCollider(PhysicsMessageType.Enter, collision.collider.tag);
        }

        private void OnCollisionStay(UnityEngine.Collision collision)
        {
            ProcessCollider(PhysicsMessageType.Stay, collision.collider.tag);
        }

        private void OnCollisionExit(UnityEngine.Collision collision)
        {
            ProcessCollider(PhysicsMessageType.Exit, collision.collider.tag);
        }
    }
}

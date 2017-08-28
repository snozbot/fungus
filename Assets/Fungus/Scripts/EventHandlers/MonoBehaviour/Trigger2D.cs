using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{
    /// <summary>
    /// The block will execute when a 2d physics trigger matching some basic conditions is met. 
    /// </summary>
    [EventHandlerInfo("MonoBehaviour",
                      "Trigger2D",
                      "The block will execute when a 2d physics trigger matching some basic conditions is met.")]
    [AddComponentMenu("")]
    public class Trigger2D : BasePhysicsEventHandler
    {
        private void OnTriggerEnter2D(Collider2D col)
        {
            ProcessCollider(PhysicsMessageType.Enter, col.tag);
        }
        
        private void OnTriggerStay2D(Collider2D col)
        {
            ProcessCollider(PhysicsMessageType.Stay, col.tag);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            ProcessCollider(PhysicsMessageType.Exit, col.tag);
        }

    }
}

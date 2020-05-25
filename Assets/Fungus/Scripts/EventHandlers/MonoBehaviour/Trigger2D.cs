// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

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
        [Tooltip("Optional variable to store the collider that caused the trigger to occur.")]
        [VariableProperty(typeof(Collider2DVariable))]
        [SerializeField] protected Collider2DVariable colliderVar;

        private void OnTriggerEnter2D(Collider2D col)
        {
            ProcessCollider(PhysicsMessageType.Enter, col);
        }

        private void OnTriggerStay2D(Collider2D col)
        {
            ProcessCollider(PhysicsMessageType.Stay, col);
        }

        private void OnTriggerExit2D(Collider2D col)
        {
            ProcessCollider(PhysicsMessageType.Exit, col);
        }

        protected void ProcessCollider(PhysicsMessageType from, Collider2D other)
        {
            if ((from & FireOn) != 0 && DoesPassFilter(other.tag))
            {
                if (colliderVar != null)
                {
                    colliderVar.Value = other;
                }

                ExecuteBlock();
            }
        }
    }
}
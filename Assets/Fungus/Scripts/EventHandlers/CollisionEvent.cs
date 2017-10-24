using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Event is triggered when the collision occurs between GameObject and specified tagged Object
    /// </summary>
    public enum CollisionType
    {
        Collision,
        Trigger
    }

    [EventHandlerInfo("Scripting",
                    "Collision Event",
                    "Event is triggered when a collision event happens between the current GameObject and another GameObject")]
    public class CollisionEvent : EventHandler
    {
        public CollisionType Type = CollisionType.Collision;
        [Tooltip("Gametag of object to detect collisions with")]
        public string Tag;

        void OnCollisionEnter(Collision other)
        {
            if (Type == CollisionType.Collision)
            {
                if (Tag != "")
                {
                    if (other.gameObject.CompareTag(Tag))
                    {
                        ExecuteBlock();
                    }
                }
                else
                {
                    ExecuteBlock();
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (Type == CollisionType.Trigger)
            {
                if (Tag != "")
                {
                    if (other.gameObject.CompareTag(Tag))
                    {
                        ExecuteBlock();
                    }
                }
                else
                {
                    ExecuteBlock();
                }
            }
        }

    }
}
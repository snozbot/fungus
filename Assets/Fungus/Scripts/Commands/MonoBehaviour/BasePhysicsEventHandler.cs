using UnityEngine;
using UnityEngine.UI;

namespace Fungus
{

    /// <summary>
    /// Base class for all of our physics event handlers
    /// </summary>
    [AddComponentMenu("")]
    public abstract class BasePhysicsEventHandler : TagFilteredEventHandler
    {
        [System.Flags]
        public enum PhysicsMessageType
        {
            Enter = 1 << 0,
            Stay = 1 << 1,
            Exit = 1 << 2,
        }

        [Tooltip("Which of the 3d physics messages to we trigger on.")]
        [SerializeField]
        [EnumFlag]
        protected PhysicsMessageType FireOn = PhysicsMessageType.Enter;

        protected void ProcessCollider(PhysicsMessageType from, string tagOnOther)
        {
            if ((from & FireOn) != 0)
            {
                ProcessTagFilter(tagOnOther);
            }
        }
    }
}

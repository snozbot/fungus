// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

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

        [Tooltip("Which of the physics messages do we trigger on.")]
        [SerializeField]
        [EnumFlag]
        protected PhysicsMessageType FireOn = PhysicsMessageType.Enter;
    }
}
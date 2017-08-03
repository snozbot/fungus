using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the desired OnParticle message for the monobehaviour is received.
    /// </summary>
    [EventHandlerInfo("MonoBehaviour",
                      "Particle",
                      "The block will execute when the desired OnParticle message for the monobehaviour is received.")]
    [AddComponentMenu("")]
    public class Particle : TagFilteredEventHandler
    {

        [System.Flags]
        public enum ParticleMessageFlags
        {
            OnParticleCollision = 1 << 0,
            OnParticleTrigger = 1 << 1,
           
        }

        [Tooltip("Which of the Rendering messages to trigger on.")]
        [SerializeField]
        [EnumFlag]
        protected ParticleMessageFlags FireOn = ParticleMessageFlags.OnParticleCollision;

        private void OnParticleCollision(GameObject other)
        {
            if ((FireOn & ParticleMessageFlags.OnParticleCollision) != 0)
            {
                ProcessTagFilter(other.tag);
            }
        }

        private void OnParticleTrigger()
        {
            if ((FireOn & ParticleMessageFlags.OnParticleTrigger) != 0)
            {
                ExecuteBlock();
            }
        }
    }
}

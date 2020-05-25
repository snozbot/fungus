// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

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

        [Tooltip("Optional variable to store the gameobject that particle collided with.")]
        [VariableProperty(typeof(GameObjectVariable))]
        [SerializeField] protected GameObjectVariable GOcolliderVar;

        private void OnParticleCollision(GameObject other)
        {
            if ((FireOn & ParticleMessageFlags.OnParticleCollision) != 0)
            {
                if (DoesPassFilter(other.tag))
                {
                    if (GOcolliderVar != null)
                    {
                        GOcolliderVar.Value = other;
                    }

                    ExecuteBlock();
                }
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

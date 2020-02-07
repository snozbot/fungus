// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    // <summary>
    /// Normalise a vector3, output can be the same as the input
    /// </summary>
    [CommandInfo("Vector3",
                 "Normalise",
                 "Normalise a Vector3")]
    [AddComponentMenu("")]
    public class Vector3Normalise : Command
    {
        [SerializeField]
        protected Vector3Data vec3In, vec3Out;

        public override void OnEnter()
        {
            vec3Out.Value = vec3In.Value.normalized;

            Continue();
        }

        public override string GetSummary()
        {
            if (vec3Out.vector3Ref == null)
                return "";
            else
                return vec3Out.vector3Ref.Key;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            if (vec3In.vector3Ref == variable || vec3Out.vector3Ref == variable)
                return true;

            return false;
        }
    }
}
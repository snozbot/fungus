// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Convert fungus vec3 to vec2
    /// </summary>
    [CommandInfo("Vector3",
                 "ToVector2",
                 "Convert Fungus Vector3 to Fungus Vector2")]
    [AddComponentMenu("")]
    public class Vector3ToVector2 : Command
    {
        [SerializeField]
        protected Vector3Data vec3;


        [SerializeField]
        protected Vector2Data vec2;

        public override void OnEnter()
        {
            vec2.Value = vec3.Value;

            Continue();
        }

        public override string GetSummary()
        {
            if (vec3.vector3Ref != null && vec2.vector2Ref != null)
            {
                return "Converting " + vec3.vector3Ref.Key + " to " + vec2.vector2Ref.Key;
            }

            return "Error: variables not set";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }


        public override bool HasReference(Variable variable)
        {
            if (variable == vec3.vector3Ref || variable == vec2.vector2Ref)
                return true;

            return false;
        }
    }
}
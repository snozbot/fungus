// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    // <summary>
    /// Get or Set the x,y,z fields of a vector3 via floatvars
    /// </summary>
    [CommandInfo("Vector3",
                 "Fields",
                 "Get or Set the x,y,z fields of a vector3 via floatvars")]
    [AddComponentMenu("")]
    public class Vector3Fields : Command
    {
        public enum GetSet
        {
            Get,
            Set,
        }
        public GetSet getOrSet = GetSet.Get;

        [SerializeField]
        protected Vector3Data vec3;

        [SerializeField]
        protected FloatData x, y, z;

        public override void OnEnter()
        {
            switch (getOrSet)
            {
                case GetSet.Get:

                    var v = vec3.Value;

                    x.Value = v.x;
                    y.Value = v.y;
                    z.Value = v.z;
                    break;
                case GetSet.Set:
                    vec3.Value = new Vector3(x.Value, y.Value, z.Value);
                    break;
                default:
                    break;
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (vec3.vector3Ref == null)
            {
                return "Error: vec3 not set";
            }

            return getOrSet.ToString() + " (" + vec3.vector3Ref.Key + ")";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            if (vec3.vector3Ref == variable || x.floatRef == variable || y.floatRef == variable || z.floatRef == variable)
                return true;

            return false;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    // <summary>
    /// Get or Set the x,y,z fields of a vector3 via floatvars
    /// </summary>
    [CommandInfo("Vector3Int",
                 "Fields",
                 "Get or Set the x,y,z fields of a vector3Int via floatvars")]
    [AddComponentMenu("")]
    public class Vector3IntFields : Command
    {
        public enum GetSet
        {
            Get,
            Set,
        }
        public GetSet getOrSet = GetSet.Get;

        [SerializeField]
        protected Vector3IntData vec3Int;

        [SerializeField]
        protected IntegerData x, y, z;

        public override void OnEnter()
        {
            switch (getOrSet)
            {
                case GetSet.Get:

                    var v = vec3Int.Value;

                    x.Value = v.x;
                    y.Value = v.y;
                    z.Value = v.z;
                    break;
                case GetSet.Set:
                    vec3Int.Value = new Vector3Int(x.Value, y.Value, z.Value);
                    break;
                default:
                    break;
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (vec3Int.vector3IntRef == null)
            {
                return "Error: vec3 not set";
            }

            return getOrSet.ToString() + " (" + vec3Int.vector3IntRef.Key + ")";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            if (vec3Int.vector3IntRef == variable || x.integerRef == variable || y.integerRef == variable || z.integerRef == variable)
                return true;

            return false;
        }
    }
}
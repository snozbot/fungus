using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    // <summary>
    /// Get or Set the x,y,z fields of a vector3 via floatvars
    /// </summary>
    [CommandInfo("Vector2",
                 "Fields",
                 "Get or Set the x,y fields of a vector2 via floatvars")]
    [AddComponentMenu("")]
    public class Vector2Fields : Command
    {
        public enum GetSet
        {
            Get,
            Set,
        }
        public GetSet getOrSet = GetSet.Get;

        [SerializeField]
        protected Vector2Data vec2;

        [SerializeField]
        protected FloatData x, y;

        public override void OnEnter()
        {
            switch (getOrSet)
            {
                case GetSet.Get:

                    var v = vec2.Value;

                    x.Value = v.x;
                    y.Value = v.y;
                    break;
                case GetSet.Set:
                    vec2.Value = new Vector2(x.Value, y.Value);
                    break;
                default:
                    break;
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (vec2.vector2Ref == null)
            {
                return "Error: vec3 not set";
            }

            return getOrSet.ToString() + " (" + vec2.vector2Ref.Key + ")";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            if (vec2.vector2Ref == variable || x.floatRef == variable || y.floatRef == variable)
                return true;

            return false;
        }
    }
}
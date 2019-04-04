using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    // <summary>
    /// Get or Set the x,y,z fields of a vector3 via floatvars
    /// </summary>
    [CommandInfo("Vector2Int",
                 "Fields",
                 "Get or Set the x,y,z fields of a vector3 via floatvars")]
    [AddComponentMenu("")]
    public class Vector2IntFields : Command
    {
        public enum GetSet
        {
            Get,
            Set,
        }
        public GetSet getOrSet = GetSet.Get;

        [SerializeField]
        protected Vector2IntData vec2Int;

        [SerializeField]
        protected IntegerData x, y;

        public override void OnEnter()
        {
            switch (getOrSet)
            {
                case GetSet.Get:

                    var v = vec2Int.Value;

                    x.Value = v.x;
                    y.Value = v.y;
                    break;
                case GetSet.Set:
                    vec2Int.Value = new Vector2Int(x.Value, y.Value);
                    break;
                default:
                    break;
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (vec2Int.vector2IntRef == null)
            {
                return "Error: vec2Int not set";
            }

            return getOrSet.ToString() + " (" + vec2Int.vector2IntRef.Key + ")";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            if (vec2Int.vector2IntRef == variable || x.integerRef == variable || y.integerRef == variable)
                return true;

            return false;
        }
    }
}
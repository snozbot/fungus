using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    // <summary>
    /// Convert fungus vec3 to vec2
    /// </summary>
    [CommandInfo("Vector2Int",
                 "ToVector2",
                 "Convert Fungus Vector2Int to Fungus Vector2")]
    [AddComponentMenu("")]
    public class Vector2IntToVector2 : Command
    {
        [SerializeField]
        protected Vector2IntData vec2Int;


        [SerializeField]
        protected Vector2Data vec2;

        public override void OnEnter()
        {
            vec2.Value = vec2Int.Value;

            Continue();
        }

        public override string GetSummary()
        {
            if (vec2Int.vector2IntRef != null && vec2.vector2Ref != null)
            {
                return "Converting " + vec2Int.vector2IntRef.Key + " to " + vec2.vector2Ref.Key;
            }

            return "Error: variables not set";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }


        public override bool HasReference(Variable variable)
        {
            if (variable == vec2Int.vector2IntRef || variable == vec2.vector2Ref)
                return true;

            return false;
        }
    }
}
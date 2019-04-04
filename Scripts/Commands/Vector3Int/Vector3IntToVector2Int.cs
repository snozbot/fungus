using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    // <summary>
    /// Convert fungus vec3 to vec2
    /// </summary>
    [CommandInfo("Vector3Int",
                 "ToVector2Int",
                 "Convert Fungus Vector3Int to Fungus Vector2Int")]
    [AddComponentMenu("")]
    public class Vector3IntToVector2Int : Command
    {
        [SerializeField]
        protected Vector3IntData vec3Int;


        [SerializeField]
        protected Vector2IntData vec2Int;

        public override void OnEnter()
        {
            vec2Int.Value = new Vector2Int(vec3Int.Value.x, vec3Int.Value.y);

            Continue();
        }

        public override string GetSummary()
        {
            if (vec3Int.vector3IntRef != null && vec2Int.vector2IntRef != null)
            {
                return "Converting " + vec3Int.vector3IntRef.Key + " to " + vec2Int.vector2IntRef.Key;
            }

            return "Error: variables not set";
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }


        public override bool HasReference(Variable variable)
        {
            if (variable == vec3Int.vector3IntRef || variable == vec2Int.vector2IntRef)
                return true;

            return false;
        }
    }
}
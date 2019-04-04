using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    // <summary>
    /// Normalise a vector2, output can be the same as the input
    /// </summary>
    [CommandInfo("Vector2",
                 "Normalise",
                 "Normalise a Vector2")]
    [AddComponentMenu("")]
    public class Vector2Normalise : Command
    {
        [SerializeField]
        protected Vector2Data vec2In, vec2Out;

        public override void OnEnter()
        {
            vec2Out.Value = vec2In.Value.normalized;

            Continue();
        }

        public override string GetSummary()
        {
            if (vec2Out.vector2Ref == null)
                return "";
            else
                return vec2Out.vector2Ref.Key;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            if (vec2In.vector2Ref == variable || vec2Out.vector2Ref == variable)
                return true;

            return false;
        }
    }
}
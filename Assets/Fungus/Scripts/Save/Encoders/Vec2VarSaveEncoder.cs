using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Type = System.Type;

namespace Fungus
{
    public class Vec2VarSaveEncoder : VarSaveEncoder
    {
        protected override IList<Type> SupportedTypes { get; } = new Type[]
        {
            typeof(Vector2Variable)
        };
    }
}
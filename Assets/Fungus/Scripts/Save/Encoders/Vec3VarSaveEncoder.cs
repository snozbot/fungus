using System.Collections.Generic;
using Type = System.Type;

namespace Fungus
{
    public class Vec3VarSaveEncoder : VarSaveEncoder
    {
        protected override IList<Type> SupportedTypes { get; } = new Type[]
        {
            typeof(Vector3Variable)
        };
    }
}
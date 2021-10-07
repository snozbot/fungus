using System.Collections.Generic;
using Type = System.Type;

namespace Fungus
{
    public class Vec3VarSaver : VarSaver
    {
        protected override IList<Type> SupportedTypes { get; } = new Type[]
        {
            typeof(Vector3Variable)
        };
    }
}
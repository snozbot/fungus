//from http://wiki.unity3d.com/index.php/EnumFlagPropertyDrawer

//placed in fungus namespace to avoid collisions with your own

using UnityEngine;

namespace Fungus
{
    public class EnumFlagAttribute : PropertyAttribute
    {
        public string enumName;

        public EnumFlagAttribute() { }

        public EnumFlagAttribute(string name)
        {
            enumName = name;
        }
    }
}
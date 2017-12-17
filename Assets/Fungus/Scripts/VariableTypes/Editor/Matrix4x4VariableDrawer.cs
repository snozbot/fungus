using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomPropertyDrawer(typeof(Matrix4x4Data))]
    public class Matrix4x4DataDrawer : VariableDataDrawer<Matrix4x4Variable>
    { }
}
using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    [CustomPropertyDrawer(typeof(QuaternionData))]
    public class QuaternionDataDrawer : VariableDataDrawer<QuaternionVariable>
    { }
}
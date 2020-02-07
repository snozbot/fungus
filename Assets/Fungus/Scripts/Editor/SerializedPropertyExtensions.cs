// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEditor;
using UnityEngine;

namespace Fungus.EditorUtils
{
    public static class SerializedPropertyExtensions
    {
        public static void ResetValue(this SerializedProperty element) {
            switch (element.type) {
                case "string":
                    element.stringValue = "";
                    break;
                case "Vector2f":
                    element.vector2Value = Vector2.zero;
                    break;
                case "Vector3f":
                    element.vector3Value = Vector3.zero;
                    break;
                case "Rectf":
                    element.rectValue = new Rect();
                    break;
                case "Quaternionf":
                    element.quaternionValue = Quaternion.identity;
                    break;
                case "int":
                    element.intValue = 0;
                    break;
                case "float":
                    element.floatValue = 0f;
                    break;
                case "UInt8":
                    element.boolValue = false;
                    break;
                case "ColorRGBA":
                    element.colorValue = Color.black;
                    break;
                
                default:
                    if (element.type.StartsWith("PPtr"))
                        element.objectReferenceValue = null;
                    break;
            }
        }
    }
}
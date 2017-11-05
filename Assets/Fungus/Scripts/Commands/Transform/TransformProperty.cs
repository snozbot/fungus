using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    // <summary>
    /// Get or Set a property of a transform component
    /// </summary>
    [CommandInfo("Transform",
                 "Property",
                 "Get or Set a property of a transform component")]
    [AddComponentMenu("")]
    public class TransformProperty : Command
    {
        public enum GetSet
        {
            Get,
            Set,
        }
        public GetSet getOrSet = GetSet.Get;

        public enum Property
        {
            ChildCount,
            EulerAngles,
            Forward,
            HasChanged,
            HierarchyCapacity,
            HierarchyCount,
            LocalEulerAngles,
            LocalPosition,
            LocalScale,
            LossyScale,
            Parent,
            Position,
            Right,
            Root,
            Up,
            //no quat or mat4 yet
            //LocalRotation,
            //Rotation,
            //LocalToWorldMatrix,
            //WorldToLocalMatrix
        }
        [SerializeField]
        protected Property property = Property.Position;

        [SerializeField]
        protected TransformData transformData;

        [SerializeField]
        [VariableProperty(typeof(BooleanVariable),
                          typeof(IntegerVariable),
                          typeof(Vector3Variable),
                          typeof(TransformVariable))]
        protected Variable inOutVar;

        public override void OnEnter()
        {
            var iob = inOutVar as BooleanVariable;
            var ioi = inOutVar as IntegerVariable;
            var iov = inOutVar as Vector3Variable;
            var iot = inOutVar as TransformVariable;

            var t = transformData.Value;

            switch (getOrSet)
            {
                case GetSet.Get:
                    switch (property)
                    {
                        case Property.ChildCount:
                            ioi.Value = t.childCount;
                            break;
                        case Property.EulerAngles:
                            iov.Value = t.eulerAngles;
                            break;
                        case Property.Forward:
                            iov.Value = t.forward;
                            break;
                        case Property.HasChanged:
                            iob.Value = t.hasChanged;
                            break;
                        case Property.HierarchyCapacity:
                            ioi.Value = t.hierarchyCapacity;
                            break;
                        case Property.HierarchyCount:
                            ioi.Value = t.hierarchyCount;
                            break;
                        case Property.LocalEulerAngles:
                            iov.Value = t.localEulerAngles;
                            break;
                        case Property.LocalPosition:
                            iov.Value = t.localPosition;
                            break;
                        case Property.LocalScale:
                            iov.Value = t.localScale;
                            break;
                        case Property.LossyScale:
                            iov.Value = t.lossyScale;
                            break;
                        case Property.Parent:
                            iot.Value = t.parent;
                            break;
                        case Property.Position:
                            iov.Value = t.position;
                            break;
                        case Property.Right:
                            iov.Value = t.right;
                            break;
                        case Property.Root:
                            iot.Value = t.parent;
                            break;
                        case Property.Up:
                            iov.Value = t.up;
                            break;
                        default:
                            break;
                    }
                    break;
                case GetSet.Set:
                    switch (property)
                    {
                        case Property.ChildCount:
                            Debug.LogWarning("Cannot Set childCount, it is read only");
                            break;
                        case Property.EulerAngles:
                            t.eulerAngles = iov.Value;
                            break;
                        case Property.Forward:
                            t.forward = iov.Value;
                            break;
                        case Property.HasChanged:
                            t.hasChanged = iob.Value;
                            break;
                        case Property.HierarchyCapacity:
                            t.hierarchyCapacity = ioi.Value;
                            break;
                        case Property.HierarchyCount:
                            Debug.LogWarning("Cannot Set HierarchyCount, it is read only");
                            break;
                        case Property.LocalEulerAngles:
                            t.localEulerAngles = iov.Value;
                            break;
                        case Property.LocalPosition:
                            t.localPosition = iov.Value;
                            break;
                        case Property.LocalScale:
                            t.localScale = iov.Value;
                            break;
                        case Property.LossyScale:
                            Debug.LogWarning("Cannot Set LossyScale, it is read only");
                            break;
                        case Property.Parent:
                            t.parent = iot.Value;
                            break;
                        case Property.Position:
                            t.position = iov.Value;
                            break;
                        case Property.Right:
                            t.right = iov.Value;
                            break;
                        case Property.Root:
                            Debug.LogWarning("Cannot Set Root, it is read only");
                            break;
                        case Property.Up:
                            t.up = iov.Value;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (transformData.Value == null)
            {
                return "Error: no transform set";
            }

            var iob = inOutVar as BooleanVariable;
            var ioi = inOutVar as IntegerVariable;
            var iov = inOutVar as Vector3Variable;
            var iot = inOutVar as TransformVariable;

            if (iob == null && ioi == null && iov == null && iot == null)
            {
                return "Error: no variable set to push or pull data to or from";
            }

            //We could do further checks here, eg, you have selected childcount but set a vec3variable

            return getOrSet.ToString() + " " + property.ToString();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            if (transformData.transformRef == variable || inOutVar == variable)
                return true;

            return false;
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Vector3 add, sub, mul, div arithmetic
    /// </summary>
    [CommandInfo("Vector3Int",
                 "Arithmetic",
                 "Vector3Int add, sub, mul, div arithmetic")]
    [AddComponentMenu("")]
    public class Vector3IntArithmetic : Command
    {
        [SerializeField]
        protected Vector3IntData lhs, rhs, output;
        [SerializeField]
        protected IntegerData xvalue, yvalue, zvalue;

        public enum Operation
        {
            Add,
            AddInt,
            Sub,
            SubInt,
            Mul,
            MulInt,
            Div,
            DivInt
        }

        [SerializeField]
        protected Operation operation = Operation.Add;

        public override void OnEnter()
        {
            Vector3Int tmp;
            switch (operation)
            {
                case Operation.Add:
                    output.Value = lhs.Value + rhs.Value;
                    break;
                case Operation.AddInt:
                    output.Value = lhs.Value + new Vector3Int(xvalue.Value, yvalue.Value, zvalue.Value);
                    break;
                case Operation.Sub:
                    output.Value = lhs.Value - rhs.Value;
                    break;
                case Operation.SubInt:
                    output.Value = lhs.Value - new Vector3Int(xvalue.Value, yvalue.Value, zvalue.Value);
                    break;
                case Operation.Mul:
                    tmp = lhs.Value;
                    tmp.Scale(rhs.Value);
                    output.Value = tmp;
                    break;
                case Operation.MulInt:
                    tmp = lhs.Value;
                    tmp.Scale(new Vector3Int(xvalue.Value, yvalue.Value, zvalue.Value));
                    output.Value = tmp;
                    break;
                case Operation.Div:
                    tmp = lhs.Value;
                    Vector3 vector3float = new Vector3(1.0f / rhs.Value.x, 1.0f / rhs.Value.y, 1.0f / rhs.Value.z);
                    tmp.Scale(new Vector3Int(Mathf.RoundToInt(vector3float.x), Mathf.RoundToInt(vector3float.y), Mathf.RoundToInt(vector3float.z)));
                    output.Value = tmp;
                    break;
                case Operation.DivInt:
                    tmp = lhs.Value;
                    Vector3 vector3floatb = new Vector3(1.0f / xvalue.Value, 1.0f / yvalue.Value, 1.0f / zvalue.Value);
                    tmp.Scale(new Vector3Int(Mathf.RoundToInt(vector3floatb.x), Mathf.RoundToInt(vector3floatb.y), Mathf.RoundToInt(vector3floatb.z)));
                    output.Value = tmp;
                    break;
                default:
                    break;
            }
            Continue();
        }

        public override string GetSummary()
        {
            if (output.vector3IntRef == null)
            {
                return "Error: no output set";
            }

            return operation.ToString() + ": stored in " + output.vector3IntRef.Key;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            if (lhs.vector3IntRef == variable || rhs.vector3IntRef == variable || output.vector3IntRef == variable)
                return true;

            return false;
        }
    }
}
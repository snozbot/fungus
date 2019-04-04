using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Vector3 add, sub, mul, div arithmetic
    /// </summary>
    [CommandInfo("Vector2Int",
                 "Arithmetic",
                 "Vector2Int add, sub, mul, div arithmetic")]
    [AddComponentMenu("")]
    public class Vector2IntArithmetic : Command
    {
        [SerializeField]
        protected Vector2IntData lhs, rhs, output;
        [SerializeField]
        protected IntegerData xvalue, yvalue;

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
            Vector2Int tmp;
            switch (operation)
            {
                case Operation.Add:
                    output.Value = lhs.Value + rhs.Value;
                    break;
                case Operation.AddInt:
                    output.Value = lhs.Value + new Vector2Int(xvalue.Value, yvalue.Value);
                    break;
                case Operation.Sub:
                    output.Value = lhs.Value - rhs.Value;
                    break;
                case Operation.SubInt:
                    output.Value = lhs.Value - new Vector2Int(xvalue.Value, yvalue.Value);
                    break;
                case Operation.Mul:
                    tmp = lhs.Value;
                    tmp.Scale(rhs.Value);
                    output.Value = tmp;
                    break;
                case Operation.MulInt:
                    tmp = lhs.Value;
                    tmp.Scale(new Vector2Int(xvalue.Value, yvalue.Value));
                    output.Value = tmp;
                    break;
                case Operation.Div:
                    tmp = lhs.Value;
                    Vector2 vector2float = new Vector2(1.0f / rhs.Value.x,   1.0f / rhs.Value.y);
                    tmp.Scale(new Vector2Int(Mathf.RoundToInt(vector2float.x), Mathf.RoundToInt(vector2float.y)));
                    output.Value = tmp;
                    break;
                case Operation.DivInt:
                    tmp = lhs.Value;
                    Vector2 vector2floatb = new Vector2(1.0f / xvalue.Value, 1.0f / yvalue.Value);
                    tmp.Scale(new Vector2Int(Mathf.RoundToInt(vector2floatb.x), Mathf.RoundToInt(vector2floatb.y)));
                    output.Value = tmp;
                    break;
                default:
                    break;
            }
            Continue();
        }

        public override string GetSummary()
        {
            if (output.vector2IntRef == null)
            {
                return "Error: no output set";
            }

            return operation.ToString() + ": stored in " + output.vector2IntRef.Key;
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }

        public override bool HasReference(Variable variable)
        {
            if (lhs.vector2IntRef == variable || rhs.vector2IntRef == variable || output.vector2IntRef == variable)
                return true;

            return false;
        }
    }
}
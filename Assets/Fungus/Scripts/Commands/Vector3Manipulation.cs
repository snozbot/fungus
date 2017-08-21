using UnityEngine;

namespace Fungus
{
    public enum Operation
    {
        /// <summary> += operator. </summary>
        Add,
        /// <summary> -= operator. </summary>
        Subtract,
        /// <summary> *= operator. </summary>
        Multiply,
        /// <summary> /= operator. </summary>
        Divide
    }

    public enum Vector3Attribute
    {
        X,
        Y,
        Z,
        ALL
    }

    /// <summary>
    /// Mathematical operations executed on Vector3
    /// </summary>
    [CommandInfo("Scripting", "Vector3 Manipulation", "Operations conducted on a Vector3 variable")]
    public class Vector3Manipulation : Command
    {

        [SerializeField]
        protected Vector3Data vector3var;

        [SerializeField]
        protected Operation operation;

        [SerializeField]
        protected Vector3Attribute property;

        [SerializeField]
        protected float changeValue;

        public override void OnEnter()
        {
            if (vector3var.Value == null)
            {
                return;
            }

            Vector3 newValue = vector3var.Value;

            switch (operation)
            {
                case Operation.Add:
                    if (property == Vector3Attribute.X)
                    {
                        newValue.x += changeValue;
                    }
                    else if (property == Vector3Attribute.Y)
                    {
                        newValue.y += changeValue;
                    }
                    else if (property == Vector3Attribute.Z)
                    {
                        newValue.z += changeValue;
                    }
                    else
                    {
                        newValue += new Vector3(changeValue, changeValue, changeValue);
                    }
                    break;
                case Operation.Subtract:
                    if (property == Vector3Attribute.X)
                    {
                        newValue.x -= changeValue;
                    }
                    else if (property == Vector3Attribute.Y)
                    {
                        newValue.y -= changeValue;
                    }
                    else if (property == Vector3Attribute.Z)
                    {
                        newValue.z -= changeValue;
                    }
                    else
                    {
                        newValue -= new Vector3(changeValue, changeValue, changeValue);
                    }
                    break;
                case Operation.Multiply:
                    if (property == Vector3Attribute.X)
                    {
                        newValue.x *= changeValue;
                    }
                    else if (property == Vector3Attribute.Y)
                    {
                        newValue.y *= changeValue;
                    }
                    else if (property == Vector3Attribute.Z)
                    {
                        newValue.z *= changeValue;
                    }
                    else
                    {
                        newValue *= changeValue;
                    }
                    break;
                case Operation.Divide:
                    if (property == Vector3Attribute.X)
                    {
                        newValue.x /= changeValue;
                    }
                    else if (property == Vector3Attribute.Y)
                    {
                        newValue.y /= changeValue;
                    }
                    else if (property == Vector3Attribute.Z)
                    {
                        newValue.z /= changeValue;
                    }
                    else
                    {
                        newValue /= changeValue;
                    }
                    break;
            }
            vector3var.Value = newValue;
            Continue();
        }

        public override string GetSummary()
        {
            if (vector3var.Value == null)
            {
                return "Error: A Vector3 variable needs to be selected";
            }

            if (changeValue == 0)
            {
                return "Error: A change value needs to be specified";
            }

            return "Vector3 being " + operation.ToString() + " by the change value of " + changeValue;
        }
    }
}
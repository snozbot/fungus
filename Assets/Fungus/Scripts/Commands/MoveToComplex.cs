using UnityEngine;

namespace Fungus
{
    public enum VariableType
    {
        Vector3,
        Transform,
        GameObject
    }

    /// <summary>
    /// Move towards an object with a specified speed
    /// </summary>
    [CommandInfo("Scripting",
                "MoveToObject",
                "Move towards an object at a speed that is specified.")]
    public class MoveToComplex : Command
    {
        [VariableProperty(typeof(TransformVariable),
                        typeof(Vector3Variable),
                        typeof(GameObjectVariable))]
        [SerializeField]
        protected Variable variable;

        [Tooltip("The speed that will be move the object in every frame")]
        public float speed;

        private bool started = false;
        private Vector3 target;

        public override void OnEnter()
        {
            DoSetOperation();
        }

        protected virtual void DoSetOperation()
        {
            //Identify target variable
            if (variable == null)
            {
                return;
            }

            if (variable.GetType() == typeof(Vector3Variable))
            {
                target = (variable as Vector3Variable).Value;
            }
            else if (variable.GetType() == typeof(TransformVariable))
            {
                target = (variable as TransformVariable).Value.position;
            }
            else if (variable.GetType() == typeof(GameObjectVariable))
            {
                target = (variable as GameObjectVariable).Value.transform.position;
            }
            else
            {
                return;
            }

            started = true;
        }

        void FixedUpdate()
        {
            if (started)
            {
                if (this.gameObject.transform.position != target)
                {
                    this.gameObject.transform.position = Vector3.MoveTowards(this.gameObject.transform.position, target, speed * Time.deltaTime);
                }
                else if (this.gameObject.transform.position == target)
                {
                    Continue();
                }
            }
        }

        public override string GetSummary()
        {
            return "Move towards target at speed of " + speed.ToString();
        }
    }
}
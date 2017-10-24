using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Generates a random Vector3 between specified values to a Variable
    /// </summary>
    [CommandInfo("Scripting",
                  "Get Random Vector3",
                  "Sets a variable with a random vector 3")]

    [AddComponentMenu("")]
    public class RandomVector3 : Command
    {
        [Tooltip("The variable whos value will be set")]
        [VariableProperty(typeof(Vector3Variable))]
        [SerializeField]
        protected Variable variable;

        [SerializeField]
        protected float minX;

        [SerializeField]
        protected float maxX;

        [SerializeField]
        protected float minY;

        [SerializeField]
        protected float maxY;

        [SerializeField]
        protected float minZ;

        [SerializeField]
        protected float maxZ;

        public override void OnEnter()
        {
            Vector3Variable randomVector = (variable as Vector3Variable);

            float xVal = UnityEngine.Random.Range(minX, maxX);
            float yVal = UnityEngine.Random.Range(minY, maxY);
            float zVal = UnityEngine.Random.Range(minZ, maxZ);

            randomVector.Value = new Vector3(xVal, yVal, zVal);

            Continue();
        }
    }
}
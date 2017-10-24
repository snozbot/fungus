using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Spawn GameObject using a Vector3, Transform or GameObject variable
    /// </summary>
    [CommandInfo("Scripting",
                "Spawn Object With Object or Transform",
                "Spawn a GameObject using a transform rather than a Vector 3")]
    public class SpawnObjectTransform : Command
    {
        public GameObjectData SourceObject;

        [VariableProperty(typeof(TransformVariable),
                        typeof(GameObjectVariable))]
        [SerializeField]
        public Variable parentVariable;

        [VariableProperty(typeof(TransformVariable),
                        typeof(Vector3Variable),
                        typeof(GameObjectVariable))]
        [SerializeField]
        public Variable spawnVariable;

        [SerializeField]
        public Vector3 SpawnRotation;

        public override void OnEnter()
        {
            if (spawnVariable == null || SourceObject.Value == null)
            {
                return;
            }

            GameObject newObject = Instantiate(SourceObject.Value);

            if (parentVariable != null)
            {
                if (parentVariable.GetType() == typeof(TransformVariable))
                {
                    newObject.transform.parent = (parentVariable as TransformVariable).Value;
                }
                else if (parentVariable.GetType() == typeof(GameObjectVariable))
                {
                    newObject.transform.parent = (parentVariable as GameObjectVariable).Value.transform;
                }
            }

            if (spawnVariable.GetType() == typeof(TransformVariable))
            {
                newObject.transform.position = (spawnVariable as TransformVariable).Value.position;
            }
            else if (spawnVariable.GetType() == typeof(Vector3Variable))
            {
                newObject.transform.position = (spawnVariable as Vector3Variable).Value;
            }
            else
            {
                newObject.transform.position = (spawnVariable as GameObjectVariable).Value.transform.position;
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (SourceObject.Value == null)
            {
                return "Error: No source GameObject specified";
            }

            if (spawnVariable == null)
            {
                return "Error: No spawn variable specfied";
            }

            return SourceObject.ToString();
        }

        public override Color GetButtonColor()
        {
            return new Color32(235, 191, 217, 255);
        }
    }
}
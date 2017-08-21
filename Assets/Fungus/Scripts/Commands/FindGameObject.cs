using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Finds an game object with tag and returns a random game object to the specified variable
    /// </summary>
    [CommandInfo("Scripting",
        "Find Game Objects With Tag",
        "Find game objects with a tag and select a random one out of them")]


    [AddComponentMenu("")]
    public class FindGameObject : Command
    {
        [VariableProperty(typeof(GameObjectVariable))]
        [SerializeField]
        protected Variable targetGameObject;

        [Tooltip("Input text or a string variable")]
        [SerializeField]
        protected StringData objectTagName;

        public override void OnEnter()
        {
            if (targetGameObject == null)
            {
                return;
            }

            GameObject[] varArray = GameObject.FindGameObjectsWithTag(objectTagName);
            if (varArray.Length == 0)
            {
                return;
            }
            else
            {
                GameObjectVariable gameobject = (targetGameObject as GameObjectVariable);
                gameobject.Value = varArray[Random.Range(0, varArray.Length)];
            }

            Continue();
        }
    }
}

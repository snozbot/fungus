using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Sets the tag of the current GameObject
    /// </summary>
    [CommandInfo("Scripting",
                  "Set Tag",
                  "Set the tag of a game object. Be sure to add the tag to the Tags/Layers Manager")]

    [AddComponentMenu("")]
    public class SetTag : Command
    {
        public GameObject targetGameObject;

        [Tooltip("Input text or a string variable")]
        [SerializeField]
        protected StringData newTagName;

        [SerializeField]
        protected bool self;

        public override void OnEnter()
        {
            if (self == false)
            {
                targetGameObject.tag = newTagName;
            }
            if (self)
            {
                gameObject.tag = newTagName;
            }

            Continue();
        }

        public override string GetSummary()
        {
            if (newTagName == "")
            {
                return "Error: No Tag Name inputed";
            }
            if (self)
            {
                return "This GameObject";
            }

            return newTagName;
        }
    }
}
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Find the closest game object with specified tag to a Vector3 location
    /// </summary>
    [CommandInfo("Scripting",
                  "Get Closest Game Object With Tag",
                  "Sets a variable with the closest object from a point")]

    [AddComponentMenu("")]
    public class GetClosestGameObject : Command
    {

        [Tooltip("Location of point origin")]
        [SerializeField]
        protected Vector3Data location;

        [Tooltip("Closest Game Object With Tag")]
        [SerializeField]
        protected GameObjectData targetGameObject;

        [SerializeField]
        protected string Tag;

        [SerializeField]
        protected float maxRange;

        public override void OnEnter()
        {
            //Retrieve all game objects with tag
            GameObject[] list = GameObject.FindGameObjectsWithTag(Tag);
            GameObject closest = null;
            float minDist = maxRange;
            //Iterate through game objects list and find closest to position.
            for (int i = 0; i < list.Length; i++)
            {
                float distance = Vector3.Distance(location.Value, list[i].transform.position);
                if (minDist > distance)
                {
                    minDist = distance;
                    closest = list[i];
                }
            }

            if (closest == null)
            {
                return;
            }

            targetGameObject.Value = closest;

            Continue();
        }
    }
}
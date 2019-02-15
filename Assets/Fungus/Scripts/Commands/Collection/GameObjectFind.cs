using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// 
    /// </summary>
    [CommandInfo("GameObject",
                    "FindAll",
                    "Find all gameobjects by tag and store in a collection")]
    [AddComponentMenu("")]
    public class GameObjectFind : CollectionBaseCommand
    {
        [Tooltip("Find all gameobjects of tag")]
        [SerializeField]
        protected StringData tagString;

        public override void OnEnter()
        {
            var col = collection.Value;

            if(col != null)
            {
                var res = GameObject.FindGameObjectsWithTag(tagString.Value);

                for (int i = 0; i < res.Length; i++)
                {
                    col.Add(res[i]);
                }
            }
        
            Continue();
        }

        public override bool HasReference(Variable variable)
        {
            return variable == tagString.stringRef || base.HasReference(variable);
        }

        public override string GetSummary()
        {
            if (collection.Value == null)
                return "Error: no collection selected";

            if (!(collection.Value is GameObjectCollection))
                return "Error: collection is not GameObjectCollection";

            return tagString.Value + " GOs, store in " + collection.Value.name;
        }
    }
}
using UnityEngine;

namespace Fungus
{
    [VariableInfo("", "Collection")]
    [AddComponentMenu("")]
    [System.Serializable]
    public class CollectionVariable : VariableBase<Collection>
    {
    }

    [System.Serializable]
    public struct CollectionData
    {
        [SerializeField]
        [VariableProperty("<Value>", typeof(CollectionVariable))]
        public CollectionVariable collectionRef;

        [SerializeField]
        public Collection collectionVal;

        public CollectionData(Collection v)
        {
            collectionVal = v;
            collectionRef = null;
        }

        public static implicit operator Collection(CollectionData objectData)
        {
            return objectData.Value;
        }

        public Collection Value
        {
            get { return (collectionRef == null) ? collectionVal : collectionRef.Value; }
            set { if (collectionRef == null) { collectionVal = value; } else { collectionRef.Value = value; } }
        }

        public string GetDescription()
        {
            if (collectionRef == null)
            {
                return collectionVal.ToString();
            }
            else
            {
                return collectionRef.Key;
            }
        }
    }
}
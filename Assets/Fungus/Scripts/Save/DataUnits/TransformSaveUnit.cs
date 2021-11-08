using UnityEngine;

namespace Fungus
{
    [System.Serializable]
    public struct TransformSaveUnit : ISaveUnit<TransformSaveUnit>
    {
        public TransformSaveUnit Contents => this;
        object ISaveUnit.Contents => this;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [SerializeField]
        private string name;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        [SerializeField]
        private Vector3 position;

        public Vector3 LocalScale
        {
            get { return localScale; }
            set { localScale = value; }
        }

        [SerializeField]
        private Vector3 localScale;

        public Quaternion Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        [SerializeField]
        private Quaternion rotation;

        public TransformSaveUnit(Transform trans)
        {
            this.name = trans.name;
            this.position = trans.position;
            this.rotation = trans.rotation;
            this.localScale = trans.localScale;
        }

        public void SetFrom(Transform trans)
        {
            this.name = trans.name;
            this.position = trans.position;
            this.rotation = trans.rotation;
            this.localScale = trans.localScale;
        }

    }
}
using UnityEngine;
using Object = System.Object;
using Type = System.Type;

namespace Fungus
{
    public struct VariableInfo
    {
        /// <summary>
        /// Name of the Flowchart variable
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [SerializeField]
        private string name;

        /// <summary>
        /// Name of the Flowchart variable's type
        /// </summary>
        public string TypeName
        {
            get { return typeName; }
            private set { typeName = value; }
        }

        [SerializeField]
        private string typeName;
        // ^ We need this to restore the Type field upon deserialization

        public Type Type
        {
            get { return type; }
            set
            {
                type = value;
                TypeName = type.Name;
            }
        }

        private Type type;

        public string Value
        {
            get { return varValue; }
            set { varValue = value; }
        }

        private string varValue;

        public VariableInfo(Variable varInput)
        {
            this.name = varInput.name;
            this.varValue = varInput.GetValue().ToString();
            this.type = typeof(Variable);
            this.typeName = this.type.Name;
        }

        public void OnDeserialize()
        {
            this.Type = Type.GetType(typeName);
        }


    }

}

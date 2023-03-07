using UnityEngine;
using Type = System.Type;

namespace Fungus.LionManeSaveSys
{
    public class VariableSaveUnit : SaveUnit
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
        public override string TypeName
        {
            get { return typeName; }
            set { typeName = value; }
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

        public VariableSaveUnit(Variable varInput)
        {
            this.name = varInput.Key;
            this.varValue = varInput.GetValue().ToString();
            this.type = varInput.GetType();
            this.typeName = this.type.Name;
        }

        public override void OnDeserialize()
        {
            base.OnDeserialize();
            this.Type = Type.GetType(typeName);
        }

    }

}

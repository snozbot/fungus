using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Type = System.Type;

namespace Fungus
{
    /// <summary>
    /// Base class for save units that contain the state of Fungus Flowchart variables.
    /// </summary>
    public abstract class VariableSaveUnit<TVarType, TVarValue> : SaveUnit<TVarValue> where TVarType: Variable
    {
        /// <summary>
        /// The name of the type of the Flowchart variable that this unit holds the state of.
        /// </summary>
        public virtual string TypeName => typeName;

        protected string typeName = typeof(TVarType).Name;

        public abstract void SetFrom(TVarType variable);

    }

    

}
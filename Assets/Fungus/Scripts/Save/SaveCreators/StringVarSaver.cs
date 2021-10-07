using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;

namespace Fungus
{
    public class StringVarSaver : MonoBehaviour, ISaveCreator<StringVarSaveUnit, StringVariable>
    {
        public virtual ISaveUnit CreateSaveFrom(Object input)
        {
            bool isCorrectType = input.GetType() == typeof(StringVariable);

            if (isCorrectType)
                return CreateSaveFrom(input as StringVariable);
            else
            {
                string messageFormat = "{0} is not a Flowchart string variable. StringVarSaver on GameObject {1} cannot work with it.";
                string errorMessage = string.Format(messageFormat, input, this.gameObject.name);
                throw new System.InvalidOperationException(errorMessage);
            }

        }

        public virtual StringVarSaveUnit CreateSaveFrom(StringVariable input)
        {
            var newUnit = new StringVarSaveUnit();
            newUnit.Contents = input.Value;
            return newUnit;
        }
    }
}
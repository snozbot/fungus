using System.Collections.Generic;
using UnityEngine;
using Type = System.Type;
using Object = System.Object;

namespace Fungus
{
    /// <summary>
    /// Creates save units for Fungus variables
    /// </summary>
    public class VarSaver : DataSaver
    {
        public override ISaveUnit CreateSaveFrom(Object input)
        {
            EnsureCorrectInput(input);
            return CreateSaveFrom(input as Variable);
        }

        public virtual ISaveUnit<VariableInfo> CreateSaveFrom(Variable input)
        {
            VariableSaveUnit newSaveUnit = new VariableSaveUnit(input);
            return newSaveUnit;
        }

        protected virtual void EnsureCorrectInput(Object maybeAVar)
        {
            bool correctInput = maybeAVar is Variable;
            if (!correctInput)
            {
                string messageFormat = "VarSaver cannot create save unit from {0}, for it isn't a Fungus Variable or subclass thereof.";
                string errorMessage = string.Format(messageFormat, maybeAVar);
                throw new System.InvalidOperationException(errorMessage);
            }
        }

        public virtual IList<ISaveUnit<VariableInfo>> CreateSavesFrom(IList<Variable> inputs)
        {
            IList<ISaveUnit<VariableInfo>> result = new VariableSaveUnit[inputs.Count];

            for (int i = 0; i < inputs.Count; i++)
            {
                Variable currentVar = inputs[i];
                VariableSaveUnit newSaveUnit = new VariableSaveUnit(currentVar);
                result[i] = newSaveUnit;
            }

            return result;

        }

        protected override bool IsValid(object input)
        {
            return input is Variable;
        }
    }
}
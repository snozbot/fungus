using System.Collections.Generic;
using Object = System.Object;
using JsonUtility = UnityEngine.JsonUtility;

namespace Fungus.LionManeSaveSys
{
    /// <summary>
    /// Base class for savers that make Save Units for Fungus variables
    /// </summary>
    public abstract class VarSaver : DataSaver
    {
        // For the format of the string value stored in the VariableInfos that the save units will contain 
        public enum ContentType
        {
            regularString, jsonString
        }

        protected abstract ContentType SetContentAs { get; }

        public override ISaveUnit CreateSaveFrom(Object input)
        {
            EnsureCorrectInput(input);
            return CreateSaveFrom(input as Variable);
        }

        protected virtual void EnsureCorrectInput(Object maybeAVar)
        {
            if (!IsValid(maybeAVar))
            {
                string messageFormat = "VarSaver cannot create save unit from {0}, for it isn't a Fungus Variable or subclass thereof.";
                string errorMessage = string.Format(messageFormat, maybeAVar);
                throw new System.InvalidOperationException(errorMessage);
            }
        }

        public virtual VariableSaveUnit CreateSaveFrom(Variable input)
        {
            var newSaveUnit = new VariableSaveUnit(input);
            SetUpContentsFor(input, ref newSaveUnit);
            return newSaveUnit;
        }

        protected virtual void SetUpContentsFor(Variable varInput, ref VariableSaveUnit info)
        {
            info.Value = GetValueInRightFormatFrom(varInput);
        }

        protected virtual string GetValueInRightFormatFrom(Variable input)
        {
            string varValue = "";

            if (SetContentAs == ContentType.regularString)
            {
                varValue = input.GetValue().ToString();
            }
            else if (SetContentAs == ContentType.jsonString)
            {
                varValue = GetJsonStringOfValueIn(input);
            }

            return varValue;
        }

        protected virtual string GetJsonStringOfValueIn(Variable input)
        {
            bool weWantItPrettyPrinted = true;
            return JsonUtility.ToJson(input.GetValue(), weWantItPrettyPrinted);
        }

        protected override bool IsValid(object input)
        {
            return input is Variable;
        }

        public virtual IList<VariableSaveUnit> CreateSavesFrom(IList<Variable> inputs)
        {
            IList<VariableSaveUnit> result = new VariableSaveUnit[inputs.Count];

            for (int i = 0; i < inputs.Count; i++)
            {
                Variable currentVar = inputs[i];
                VariableSaveUnit newSaveUnit = CreateSaveFrom(currentVar);
                result[i] = newSaveUnit;
            }

            return result;
        }
    }
}
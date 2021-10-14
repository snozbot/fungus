using System.Collections.Generic;
using Object = System.Object;
using JsonUtility = UnityEngine.JsonUtility;

namespace Fungus
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

        public virtual IList<ISaveUnit<VariableInfo>> CreateSavesFrom(IList<Variable> inputs)
        {
            IList<ISaveUnit<VariableInfo>> result = new ISaveUnit<VariableInfo>[inputs.Count];

            for (int i = 0; i < inputs.Count; i++)
            {
                Variable currentVar = inputs[i];
                ISaveUnit<VariableInfo> newSaveUnit = CreateSaveFrom(currentVar);
                result[i] = newSaveUnit;
            }

            return result;

        }

        public virtual ISaveUnit<VariableInfo> CreateSaveFrom(Variable input)
        {
            VariableSaveUnit newSaveUnit = new VariableSaveUnit(input);
            newSaveUnit.Contents = SetUpContentsFor(newSaveUnit, input);
            return newSaveUnit;
        }

        protected virtual VariableInfo SetUpContentsFor(VariableSaveUnit saveUnit, Variable hasValue)
        {
            VariableInfo contents = saveUnit.Contents;
            contents.Value = GetValueInRightFormatFrom(hasValue);
            return contents;
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
    }
}
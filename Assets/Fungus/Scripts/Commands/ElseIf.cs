// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Marks the start of a command block to be executed when the preceding If statement is False and the test expression is true.
    /// </summary>
    [CommandInfo("Flow",
                 "Else If",
                 "Marks the start of a command block to be executed when the preceding If statement is False and the test expression is true.")]
    [AddComponentMenu("")]
    public class ElseIf : VariableCondition
    {
        protected override bool IsElseIf { get { return true; } }

        #region Public members

        public override bool CloseBlock()
        {
            return true;
        }

        public override void ErrorCheck() {
            if (!HasNeededProperties()) {
                Debug.LogError("Return value empty for IF in block " + ParentBlock.BlockName);
            }
        }

        public override List<string> GetReturnValueName() {
            List<string> _listOfVariableNames = new List<string>();

            for (int i = 0; i < conditions.Count; i++) {
                _listOfVariableNames.Add(conditions[i].AnyVar.variable.key);
            }

            return _listOfVariableNames;
        }


        #endregion
    }
}
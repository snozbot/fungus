// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// If the test expression is true, execute the following command block.
    /// </summary>
    [CommandInfo("Flow",
                 "If",
                 "If the test expression is true, execute the following command block.")]
    [AddComponentMenu("")]
    public class If : VariableCondition
    {

        public override void ErrorCheck()
        {
            if (!HasNeededProperties()) {
                Debug.LogError("Return value empty for IF in block " + ParentBlock.BlockName);
            }
        }

        public override List<string> GetReturnValueName()
        {
            List<string> _listOfVariableNames = new List<string>();

            for (int i = 0; i < conditions.Count; i++) {
                _listOfVariableNames.Add(conditions[i].AnyVar.variable.key);
            }

            return _listOfVariableNames;
        }

    }
}
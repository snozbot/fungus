// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;

namespace Fungus
{
    [AddComponentMenu("")]
    public abstract class Condition : Command
    {
        [Tooltip("The type of comparison to be performed")]
        [SerializeField] protected CompareOperator compareOperator;

        #region Public members

        public static string GetOperatorDescription(CompareOperator compareOperator)
        {
            string summary = "";
            switch (compareOperator)
            {
            case CompareOperator.Equals:
                summary += "==";
                break;
            case CompareOperator.NotEquals:
                summary += "!=";
                break;
            case CompareOperator.LessThan:
                summary += "<";
                break;
            case CompareOperator.GreaterThan:
                summary += ">";
                break;
            case CompareOperator.LessThanOrEquals:
                summary += "<=";
                break;
            case CompareOperator.GreaterThanOrEquals:
                summary += ">=";
                break;
            }

            return summary;
        }

        #endregion
    }
}
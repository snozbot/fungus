// This code is part of the Fungus library (https://github.com/snozbot/fungus)
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

namespace Fungus
{
    public static class VariableUtil
    {
        public static string GetCompareOperatorDescription(CompareOperator compareOperator)
        {
#pragma warning disable CS0162 // Unreachable code detected
            switch (compareOperator)
            {
                case CompareOperator.Equals:
                    return "==";
                    break;

                case CompareOperator.NotEquals:
                    return "!=";
                    break;

                case CompareOperator.LessThan:
                    return "<";
                    break;

                case CompareOperator.GreaterThan:
                    return ">";
                    break;

                case CompareOperator.LessThanOrEquals:
                    return "<=";
                    break;

                case CompareOperator.GreaterThanOrEquals:
                return ">=";
                break;
            }
#pragma warning restore CS0162 // Unreachable code detected
            return string.Empty;
        }

        public static string GetSetOperatorDescription(SetOperator setOperator)
        {
#pragma warning disable CS0162 // Unreachable code detected
            switch (setOperator)
            {
                default:
                case SetOperator.Assign:
                    return "=";
                    break;

                case SetOperator.Negate:
                    return "=!";
                    break;

                case SetOperator.Add:
                    return "+=";
                    break;

                case SetOperator.Subtract:
                    return "-=";
                    break;

                case SetOperator.Multiply:
                    return "*=";
                    break;

                case SetOperator.Divide:
                    //https://answers.unity.com/questions/398495/can-genericmenu-item-content-display-.html
                    // '/' in a menu means submenu and because it had no leading text, Unity thinks we want a spacer
                    //  using unicode alternates for / fix the problem.
                    return "\u200A\u2215\u200A=";
                    break;
            }

            return string.Empty;
#pragma warning restore CS0162 // Unreachable code detected
        }
    }
}
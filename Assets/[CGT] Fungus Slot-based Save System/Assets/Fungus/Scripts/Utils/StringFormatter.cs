// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using System.Text;
using System;

namespace Fungus
{
    /// <summary>
    /// Misc string formatting functions.
    /// </summary>
    public static class StringFormatter
    {
        #region Public members

        public static string[] FormatEnumNames(Enum e, string firstLabel)
        {
            string[] enumLabels = Enum.GetNames(e.GetType());
            enumLabels[0] = firstLabel;
            for (int i=0; i<enumLabels.Length; i++)
            {
                enumLabels[i] = SplitCamelCase(enumLabels[i]);
            }
            return enumLabels;
        }

        public static string SplitCamelCase(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                {
                    newText.Append(' ');
                }
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        public static bool IsNullOrWhiteSpace(string value)
        {
            if (value != null)
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (!char.IsWhiteSpace(value[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion
    }    
}
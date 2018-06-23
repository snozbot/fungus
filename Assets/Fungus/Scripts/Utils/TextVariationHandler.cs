using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Fungus
{
    /// <summary>
    /// Handles replacing vary text segments. Keeps history of previous replacements to allow for ordered 
    /// sequence of variation. Inspired by https://github.com/inkle/ink/blob/master/Documentation/WritingWithInk.md#6-variable-text
    /// 
    /// [] mark the bounds of the vary section
    /// | divide elements within the variation
    /// 
    /// Default behaviour is to show one element after another and hold the final element. Such that [a|b|c] will show
    /// a the first time it is parsed, b the second and every subsequent time it will show c.
    /// 
    /// This behaviour can be modified with certain characters at the start of the [], eg. [&a|b|c];
    /// - & does not hold the final element it wraps back around to the begining in a looping fashion
    /// - ! does not hold the final element, it instead returns empty for the varying section
    /// - ~ chooses a random element every time it is encountered 
    /// </summary>
    public static class TextVariationHandler
    {
        static Dictionary<int, int> hashedSections = new Dictionary<int, int>();
        static StringBuilder sb = new StringBuilder();
        const string pattern = @"\[([^]]+?)\]";
        static Regex r = new Regex(pattern);

        private enum VaryType
        {
            Sequence,
            Cycle,
            Once,
            Random
        }

        static public string SelectVariations(string input)
        {
            sb.Length = 0;
            sb.Append(input);

            // Match the regular expression pattern against a text string.
            var results = r.Matches(input);
            for (int i = 0; i < results.Count; i++)
            {
                Match match = results[i];

                //determine type
                VaryType t = VaryType.Sequence;

                var typedIndicatingChar = match.Value[1];
                switch (typedIndicatingChar)
                {
                    case '~':
                        t = VaryType.Random;
                        break;
                    case '&':
                        t = VaryType.Cycle;
                        break;
                    case '!':
                        t = VaryType.Once;
                        break;
                    default:
                        break;
                }

                //explode and remove the control char
                int startSubStrIndex = t != VaryType.Sequence ? 2 : 1;
                int subStrLen = match.Value.Length - 1 - startSubStrIndex;
                var exploded = match.Value.Substring(startSubStrIndex, subStrLen).Split('|');
                string selected = string.Empty;

                //fetched hashed value
                int index = -1;
                int key = input.GetHashCode() ^ match.Value.GetHashCode();

                int foundVal = 0;
                if(hashedSections.TryGetValue(key, out foundVal))
                {
                    index = foundVal;
                }
                
                index++;

                switch (t)
                {
                    case VaryType.Sequence:
                        index = UnityEngine.Mathf.Min(index, exploded.Length - 1);
                        break;
                    case VaryType.Cycle:
                        index = index % exploded.Length;
                        break;
                    case VaryType.Once:
                        //clamp to 1 more than options
                        index = UnityEngine.Mathf.Min(index, exploded.Length);
                        break;
                    case VaryType.Random:
                        index = UnityEngine.Random.Range(0, exploded.Length - 1);
                        break;
                    default:
                        break;
                }

                //update hashed value
                hashedSections[key] = index;

                //selected updated if valid
                if(index >=0 && index < exploded.Length)
                    selected = exploded[index];


                sb.Replace(match.Value, selected);
            }

            return sb.ToString();
        }
    }
}
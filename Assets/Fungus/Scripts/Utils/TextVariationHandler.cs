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
    /// Empty sections are allowed, such that [a||c], on second showing it will have 0 characters.
    /// 
    /// Supports nested sections, that are only evaluated if their parent element is chosen. 
    /// 
    /// This behaviour can be modified with certain characters at the start of the [], eg. [&a|b|c];
    /// - & does not hold the final element it wraps back around to the begining in a looping fashion
    /// - ! does not hold the final element, it instead returns empty for the varying section
    /// - ~ chooses a random element every time it is encountered 
    /// </summary>
    public static class TextVariationHandler
    {
        public class Section
        {
            public VaryType type = VaryType.Sequence;

            public enum VaryType
            {
                Sequence,
                Cycle,
                Once,
                Random
            }

            public string entire = string.Empty;
            public List<string> elements = new List<string>();

            public string Select(ref int index)
            {
                switch (type)
                {
                    case VaryType.Sequence:
                        index = UnityEngine.Mathf.Min(index, elements.Count - 1);
                        break;
                    case VaryType.Cycle:
                        index = index % elements.Count;
                        break;
                    case VaryType.Once:
                        //clamp to 1 more than options
                        index = UnityEngine.Mathf.Min(index, elements.Count);
                        break;
                    case VaryType.Random:
                        index = UnityEngine.Random.Range(0, elements.Count);
                        break;
                    default:
                        break;
                }

                if (index >= 0 && index < elements.Count)
                    return elements[index];

                return string.Empty;
            }
        }

        static Dictionary<int, int> hashedSections = new Dictionary<int, int>();

        static public void ClearHistory()
        {
            hashedSections.Clear();
        }

        /// <summary>
        /// Simple parser to extract depth matched []. 
        /// 
        /// Such that a string of "[Hail and well met|Hello|[Good |]Morning] Traveler" will return
        /// "[Hail and well met|Hello|[Good |]Morning]"
        /// and string of "Hail and well met|Hello|[Good |]Morning"
        /// will return [Good |]
        /// </summary>
        /// <param name="input"></param>
        /// <param name="varyingSections"></param>
        /// <returns></returns>
        static public bool TokenizeVarySections(string input, List<Section> varyingSections)
        {
            varyingSections.Clear();
            int currentDepth = 0;
            int curStartIndex = 0;
            int curPipeIndex = 0;
            Section curSection = null;

            for (int i = 0; i < input.Length; i++)
            {
                switch (input[i])
                {
                    case '[':
                        if (currentDepth == 0)
                        {
                            curSection = new Section();
                            varyingSections.Add(curSection);

                            //determine type and skip control char
                            var typedIndicatingChar = input[i + 1];
                            switch (typedIndicatingChar)
                            {
                                case '~':
                                    curSection.type = Section.VaryType.Random;
                                    break;
                                case '&':
                                    curSection.type = Section.VaryType.Cycle;
                                    break;
                                case '!':
                                    curSection.type = Section.VaryType.Once;
                                    break;
                                default:
                                    break;
                            }


                            //mark start
                            curStartIndex = i;
                            curPipeIndex = i + 1;
                        }
                        currentDepth++;
                        break;
                    case ']':
                        if (currentDepth == 1)
                        {

                            //extract, including the ]
                            curSection.entire = input.Substring(curStartIndex, i - curStartIndex + 1);


                            //close an element if we started one
                            if (curStartIndex != curPipeIndex - 1)
                            {
                                curSection.elements.Add(input.Substring(curPipeIndex, i - curPipeIndex));
                            }

                            //if has control var, clean first element
                            if(curSection.type != Section.VaryType.Sequence)
                            {
                                curSection.elements[0] = curSection.elements[0].Substring(1);
                            }
                        }
                        currentDepth--;
                        break;
                    case '|':
                        if (currentDepth == 1)
                        {
                            //split
                            curSection.elements.Add(input.Substring(curPipeIndex, i - curPipeIndex));

                            //over the | on the next one
                            curPipeIndex = i + 1;
                        }
                        break;
                    default:
                        break;
                }
            }

            return varyingSections.Count > 0;
        }

        /// <summary>
        /// Uses the results of a run of tokenisation to choose the appropriate elements
        /// </summary>
        /// <param name="input"></param>
        /// <param name="parentHash">When called recursively, we pass down the current objects hash so as to 
        /// avoid similar sub /sub sub/ etc. variations</param>
        /// <returns></returns>
        static public string SelectVariations(string input, int parentHash = 0)
        {
            // Match the regular expression pattern against a text string.
            List<Section> sections = new List<Section>();
            bool foundSections = TokenizeVarySections(input, sections);

            if (!foundSections)
                return input;

            StringBuilder sb = new StringBuilder();
            sb.Length = 0;
            sb.Append(input);

            for (int i = 0; i < sections.Count; i++)
            {
                var curSection = sections[i];
                string selected = string.Empty;

                //fetched hashed value
                int index = -1;
                
                //as input and entire can be the same thing we need to shuffle these bits
                //we use some xorshift style mixing
                int inputHash = input.GetHashCode();
                inputHash ^= inputHash << 13;
                int curSecHash = curSection.entire.GetHashCode();
                curSecHash ^= curSecHash >> 17;
                int key = inputHash ^ curSecHash ^ parentHash;

                int foundVal = 0;
                if (hashedSections.TryGetValue(key, out foundVal))
                {
                    index = foundVal;
                }

                index++;

                selected = curSection.Select(ref index);

                //update hashed value
                hashedSections[key] = index;

                //handle sub vary within selected section
                selected = SelectVariations(selected, key);

                //update with selecton
                sb.Replace(curSection.entire, selected);
            }

            return sb.ToString();
        }
    }
}
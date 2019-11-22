// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if NETFX_CORE
using MarkerMetro.Unity.WinLegacy.Reflection;
#endif

namespace Fungus
{
    /// <summary>
    /// Replaces special tokens in a string with substituted values (typically variables or localisation strings).
    /// </summary>
    public class StringSubstituter : IStringSubstituter
    {
        protected static List<ISubstitutionHandler> substitutionHandlers = new List<ISubstitutionHandler>();

        /// <summary>
        /// The StringBuilder instance used to substitute strings optimally.
        /// </summary>
        protected StringBuilder stringBuilder;

        protected int recursionDepth;

        #region Public members

        public static void RegisterHandler(ISubstitutionHandler handler)
        {
            if (!substitutionHandlers.Contains(handler))
            {
                substitutionHandlers.Add(handler);
            }
        }

        public static void UnregisterHandler(ISubstitutionHandler handler)
        {
            substitutionHandlers.Remove(handler);
        }

        /// <summary>
        /// Constructor which caches all components in the scene that implement ISubstitutionHandler.
        /// <param name="recursionDepth">Number of levels of recursively embedded keys to resolve.</param>
        /// </summary>
        public StringSubstituter(int recursionDepth = 5)
        {
            stringBuilder = new StringBuilder(1024);
            this.recursionDepth = recursionDepth;
        }

        #endregion
            
        #region IStringSubstituter implementation

        public virtual StringBuilder _StringBuilder { get { return stringBuilder; } }

        public virtual string SubstituteStrings(string input)
        {
            stringBuilder.Length = 0;
            stringBuilder.Append(input);

            if (SubstituteStrings(stringBuilder))
            {
                return stringBuilder.ToString();
            }
            else
            {
                return input; // String wasn't modified
            }
        }

        public virtual bool SubstituteStrings(StringBuilder input)
        {
            bool result = false;

            // Perform the substitution multiple times to expand nested keys
            int loopCount = 0; // Avoid infinite recursion loops
            while (loopCount < recursionDepth)
            {
                bool modified = false;
                foreach (ISubstitutionHandler handler in substitutionHandlers)
                {
                    if (handler.SubstituteStrings(input))
                    {
                        modified = true;
                        result = true;
                    }
                }

                if (!modified)
                {
                    break;
                }

                loopCount++;
            }

            return result;
        }

        #endregion
    }
}
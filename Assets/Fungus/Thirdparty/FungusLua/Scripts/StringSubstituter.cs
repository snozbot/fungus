/**
 * This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
 * It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)
 */

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fungus
{
    /// <summary>
    /// Replaces special tokens in a string with substituted values (typically variables or localisation strings).
    /// </summary>
    public class StringSubstituter
    {
        /// <summary>
        /// Interface for components that support substituting strings.
        /// </summary>
        public interface ISubstitutionHandler
        {
            /// <summary>
            /// Modifies a StringBuilder so that tokens are replaced by subsituted values.
            /// It's up to clients how to implement substitution but the convention looks like:
            /// "Hi {$VarName}" => "Hi John" where VarName == "John"
            /// <returns>True if the input was modified</returns>
            /// </summary>
            bool SubstituteStrings(StringBuilder input);
        }

        protected List<ISubstitutionHandler> substitutionHandlers = new List<ISubstitutionHandler>();

        /**
         * The StringBuilder instance used to substitute strings optimally.
         * This property is public to support client code optimisations.
         */
        public StringBuilder stringBuilder;

        private int recursionDepth;

        /// <summary>
        /// Constructor which caches all components in the scene that implement ISubstitutionHandler.
        /// <param name="recursionDepth">Number of levels of recursively embedded keys to resolve.</param>
        /// </summary>
        public StringSubstituter(int recursionDepth = 5)
        {
            CacheSubstitutionHandlers();
            stringBuilder = new StringBuilder(1024);
            this.recursionDepth = recursionDepth;
        }

        /// <summary>
        /// Populates a cache of all components in the scene that implement ISubstitutionHandler.
        /// </summary>
        public virtual void CacheSubstitutionHandlers()
        {
            // Use reflection to find all components in the scene that implement ISubstitutionHandler
            var types = this.GetType().Assembly.GetTypes().Where(type => type.IsClass &&
                !type.IsAbstract && 
                typeof(ISubstitutionHandler).IsAssignableFrom(type));

            substitutionHandlers.Clear();
            foreach (System.Type t in types)
            {
                Object[] objects = GameObject.FindObjectsOfType(t);
                foreach (Object o in objects)
                {
                    ISubstitutionHandler handler = o as ISubstitutionHandler;
                    if (handler != null)
                    {
                        substitutionHandlers.Add(handler);
                    }
                }
            }
        }

        /// <summary>
        /// Returns a new string that has been processed by all substitution handlers in the scene.
        /// </summary>
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

    }

}
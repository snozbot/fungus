using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
			/// Returns a new string with tokens replaced by subsituted values.
			/// It's up to clients how to implement substitution but the convention looks like:
			/// "Hi {$VarName}" => "Hi John" where VarName == "John"
			/// </summary>
			string SubstituteStrings(string input);
		}

		protected List<ISubstitutionHandler> substitutionHandlers = new List<ISubstitutionHandler>();

		/// <summary>
		/// Constructor which caches all components in the scene that implement ISubstitutionHandler.
		/// </summary>
		public StringSubstituter()
		{
			CacheSubstitutionHandlers();
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
			string newString = input;
			foreach (ISubstitutionHandler handler in substitutionHandlers)
			{
				newString = handler.SubstituteStrings(newString);
			}

			return newString;
		}
	}

}
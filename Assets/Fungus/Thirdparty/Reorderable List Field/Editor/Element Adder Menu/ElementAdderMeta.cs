// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Rotorz.ReorderableList {

	/// <summary>
	/// Provides meta information which is useful when creating new implementations of
	/// the <see cref="IElementAdderMenuBuilder{TContext}"/> interface.
	/// </summary>
	public static class ElementAdderMeta {

		#region Adder Menu Command Types

		private static Dictionary<Type, Dictionary<Type, List<Type>>> s_ContextMap = new Dictionary<Type, Dictionary<Type, List<Type>>>();

		private static IEnumerable<Type> GetMenuCommandTypes<TContext>() {
			return
				from a in AppDomain.CurrentDomain.GetAssemblies()
				from t in a.GetTypes()
				where t.IsClass && !t.IsAbstract && t.IsDefined(typeof(ElementAdderMenuCommandAttribute), false)
				where typeof(IElementAdderMenuCommand<TContext>).IsAssignableFrom(t)
				select t;
		}

		/// <summary>
		/// Gets an array of the <see cref="IElementAdderMenuCommand{TContext}"/> types
		/// that are associated with the specified <paramref name="contractType"/>.
		/// </summary>
		/// <typeparam name="TContext">Type of the context object that elements can be added to.</typeparam>
		/// <param name="contractType">Contract type of addable elements.</param>
		/// <returns>
		/// An array containing zero or more <see cref="System.Type"/>.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="contractType"/> is <c>null</c>.
		/// </exception>
		/// <seealso cref="GetMenuCommands{TContext}(Type)"/>
		public static Type[] GetMenuCommandTypes<TContext>(Type contractType) {
			if (contractType == null)
				throw new ArgumentNullException("contractType");

			Dictionary<Type, List<Type>> contractMap;
			List<Type> commandTypes;
			if (s_ContextMap.TryGetValue(typeof(TContext), out contractMap)) {
				if (contractMap.TryGetValue(contractType, out commandTypes))
					return commandTypes.ToArray();
			}
			else {
				contractMap = new Dictionary<Type, List<Type>>();
				s_ContextMap[typeof(TContext)] = contractMap;
			}

			commandTypes = new List<Type>();

			foreach (var commandType in GetMenuCommandTypes<TContext>()) {
				var attributes = (ElementAdderMenuCommandAttribute[])Attribute.GetCustomAttributes(commandType, typeof(ElementAdderMenuCommandAttribute));
				if (!attributes.Any(a => a.ContractType == contractType))
					continue;

				commandTypes.Add(commandType);
			}

			contractMap[contractType] = commandTypes;
			return commandTypes.ToArray();
		}

		/// <summary>
		/// Gets an array of <see cref="IElementAdderMenuCommand{TContext}"/> instances
		/// that are associated with the specified <paramref name="contractType"/>.
		/// </summary>
		/// <typeparam name="TContext">Type of the context object that elements can be added to.</typeparam>
		/// <param name="contractType">Contract type of addable elements.</param>
		/// <returns>
		/// An array containing zero or more <see cref="IElementAdderMenuCommand{TContext}"/> instances.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="contractType"/> is <c>null</c>.
		/// </exception>
		/// <seealso cref="GetMenuCommandTypes{TContext}(Type)"/>
		public static IElementAdderMenuCommand<TContext>[] GetMenuCommands<TContext>(Type contractType) {
			var commandTypes = GetMenuCommandTypes<TContext>(contractType);
			var commands = new IElementAdderMenuCommand<TContext>[commandTypes.Length];
			for (int i = 0; i < commandTypes.Length; ++i)
				commands[i] = (IElementAdderMenuCommand<TContext>)Activator.CreateInstance(commandTypes[i]);
			return commands;
		}

		#endregion

		#region Concrete Element Types

		private static Dictionary<Type, Type[]> s_ConcreteElementTypes = new Dictionary<Type, Type[]>();

		private static IEnumerable<Type> GetConcreteElementTypesHelper(Type contractType) {
			if (contractType == null)
				throw new ArgumentNullException("contractType");

			Type[] concreteTypes;
			if (!s_ConcreteElementTypes.TryGetValue(contractType, out concreteTypes)) {
				concreteTypes =
					(from a in AppDomain.CurrentDomain.GetAssemblies()
					 from t in a.GetTypes()
					 where t.IsClass && !t.IsAbstract && contractType.IsAssignableFrom(t)
					 orderby t.Name
					 select t
					).ToArray();
				s_ConcreteElementTypes[contractType] = concreteTypes;
			}

			return concreteTypes;
		}

		/// <summary>
		/// Gets a filtered array of the concrete element types that implement the
		/// specified <paramref name="contractType"/>.
		/// </summary>
		/// <remarks>
		/// <para>A type is excluded from the resulting array when one or more of the
		/// specified <paramref name="filters"/> returns a value of <c>false</c>.</para>
		/// </remarks>
		/// <param name="contractType">Contract type of addable elements.</param>
		/// <param name="filters">An array of zero or more filters.</param>
		/// <returns>
		/// An array of zero or more concrete element types.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="contractType"/> is <c>null</c>.
		/// </exception>
		/// <seealso cref="GetConcreteElementTypes(Type)"/>
		public static Type[] GetConcreteElementTypes(Type contractType, Func<Type, bool>[] filters) {
			return
				(from t in GetConcreteElementTypesHelper(contractType)
				 where IsTypeIncluded(t, filters)
				 select t
				).ToArray();
		}

		/// <summary>
		/// Gets an array of all the concrete element types that implement the specified
		/// <paramref name="contractType"/>.
		/// </summary>
		/// <param name="contractType">Contract type of addable elements.</param>
		/// <returns>
		/// An array of zero or more concrete element types.
		/// </returns>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="contractType"/> is <c>null</c>.
		/// </exception>
		/// <seealso cref="GetConcreteElementTypes(Type, Func{Type, bool}[])"/>
		public static Type[] GetConcreteElementTypes(Type contractType) {
			return GetConcreteElementTypesHelper(contractType).ToArray();
		}

		private static bool IsTypeIncluded(Type concreteType, Func<Type, bool>[] filters) {
			if (filters != null)
				foreach (var filter in filters)
					if (!filter(concreteType))
						return false;
			return true;
		}

		#endregion

	}

}

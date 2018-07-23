// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System;

namespace Rotorz.ReorderableList {

	/// <summary>
	/// Interface for building an <see cref="IElementAdderMenu"/>.
	/// </summary>
	/// <typeparam name="TContext">Type of the context object that elements can be added to.</typeparam>
	public interface IElementAdderMenuBuilder<TContext> {

		/// <summary>
		/// Sets contract type of the elements that can be included in the <see cref="IElementAdderMenu"/>.
		/// Only non-abstract class types that are assignable from the <paramref name="contractType"/>
		/// will be included in the built menu.
		/// </summary>
		/// <param name="contractType">Contract type of addable elements.</param>
		void SetContractType(Type contractType);

		/// <summary>
		/// Set the <see cref="IElementAdder{TContext}"/> implementation which is used
		/// when adding new elements to the context object.
		/// </summary>
		/// <remarks>
		/// <para>Specify a value of <c>null</c> for <paramref name="elementAdder"/> to
		/// prevent the selection of any types.</para>
		/// </remarks>
		/// <param name="elementAdder">Element adder.</param>
		void SetElementAdder(IElementAdder<TContext> elementAdder);

		/// <summary>
		/// Set the function that formats the display of type names in the user interface.
		/// </summary>
		/// <remarks>
		/// <para>Specify a value of <c>null</c> for <paramref name="formatter"/> to
		/// assume the default formatting function.</para>
		/// </remarks>
		/// <param name="formatter">Function that formats display name of type; or <c>null</c>.</param>
		void SetTypeDisplayNameFormatter(Func<Type, string> formatter);

		/// <summary>
		/// Adds a filter function which determines whether types can be included or
		/// whether they need to be excluded.
		/// </summary>
		/// <remarks>
		/// <para>If a filter function returns a value of <c>false</c> then that type
		/// will not be included in the menu interface.</para>
		/// </remarks>
		/// <param name="typeFilter">Filter function.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="typeFilter"/> is <c>null</c>.
		/// </exception>
		void AddTypeFilter(Func<Type, bool> typeFilter);

		/// <summary>
		/// Adds a custom command to the menu.
		/// </summary>
		/// <param name="command">The custom command.</param>
		/// <exception cref="System.ArgumentNullException">
		/// If <paramref name="command"/> is <c>null</c>.
		/// </exception>
		void AddCustomCommand(IElementAdderMenuCommand<TContext> command);

		/// <summary>
		/// Builds and returns a new <see cref="IElementAdderMenu"/> instance.
		/// </summary>
		/// <returns>
		/// A new <see cref="IElementAdderMenu"/> instance each time the method is invoked.
		/// </returns>
		IElementAdderMenu GetMenu();

	}

}

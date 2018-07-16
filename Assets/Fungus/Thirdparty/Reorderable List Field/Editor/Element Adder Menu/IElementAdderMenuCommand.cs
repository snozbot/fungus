// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using UnityEngine;

namespace Rotorz.ReorderableList {

	/// <summary>
	/// Interface for a menu command that can be included in an <see cref="IElementAdderMenu"/>
	/// either by annotating an implementation of the <see cref="IElementAdderMenuCommand{TContext}"/>
	/// interface with <see cref="ElementAdderMenuCommandAttribute"/> or directly by
	/// calling <see cref="IElementAdderMenuBuilder{TContext}.AddCustomCommand"/>.
	/// </summary>
	/// <typeparam name="TContext">Type of the context object that elements can be added to.</typeparam>
	public interface IElementAdderMenuCommand<TContext> {

		/// <summary>
		/// Gets the content of the menu command.
		/// </summary>
		GUIContent Content { get; }

		/// <summary>
		/// Determines whether the command can be executed.
		/// </summary>
		/// <param name="elementAdder">The associated element adder provides access to
		/// the <typeparamref name="TContext"/> instance.</param>
		/// <returns>
		/// A value of <c>true</c> if the command can execute; otherwise, <c>false</c>.
		/// </returns>
		bool CanExecute(IElementAdder<TContext> elementAdder);

		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <param name="elementAdder">The associated element adder provides access to
		/// the <typeparamref name="TContext"/> instance.</param>
		void Execute(IElementAdder<TContext> elementAdder);

	}

}

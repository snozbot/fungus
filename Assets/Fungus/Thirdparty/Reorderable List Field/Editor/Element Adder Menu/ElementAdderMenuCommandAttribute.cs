// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System;

namespace Rotorz.ReorderableList {

	/// <summary>
	/// Annotate <see cref="IElementAdderMenuCommand{TContext}"/> implementations with a
	/// <see cref="ElementAdderMenuCommandAttribute"/> to associate it with the contract
	/// type of addable elements.
	/// </summary>
	/// <example>
	/// <para>The following source code demonstrates how to add a helper menu command to
	/// the add element menu of a shopping list:</para>
	/// <code language="csharp"><![CDATA[
	/// [ElementAdderMenuCommand(typeof(ShoppingItem))]
	/// public class AddFavoriteShoppingItemsCommand : IElementAdderMenuCommand<ShoppingList> {
	///     public AddFavoriteShoppingItemsCommand() {
	///         Content = new GUIContent("Add Favorite Items");
	///     }
	///
	///     public GUIContent Content { get; private set; }
	///
	///     public bool CanExecute(IElementAdder<ShoppingList> elementAdder) {
	///         return true;
	///     }
	///     public void Execute(IElementAdder<ShoppingList> elementAdder) {
	///         // TODO: Add favorite items to the shopping list!
	///     }
	/// }
	/// ]]></code>
	/// <code language="unityscript"><![CDATA[
	/// @ElementAdderMenuCommand(ShoppingItem)
	/// class AddFavoriteShoppingItemsCommand extends IElementAdderMenuCommand.<ShoppingList> {
	///     var _content:GUIContent = new GUIContent('Add Favorite Items');
	///
	///     function get Content():GUIContent { return _content; }
	///
	///     function CanExecute(elementAdder:IElementAdder.<ShoppingList>):boolean {
	///         return true;
	///     }
	///     function Execute(elementAdder:IElementAdder.<ShoppingList>) {
	///         // TODO: Add favorite items to the shopping list!
	///     }
	/// }
	/// ]]></code>
	/// </example>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
	public sealed class ElementAdderMenuCommandAttribute : Attribute {

		/// <summary>
		/// Initializes a new instance of the <see cref="ElementAdderMenuCommandAttribute"/> class.
		/// </summary>
		/// <param name="contractType">Contract type of addable elements.</param>
		public ElementAdderMenuCommandAttribute(Type contractType) {
			ContractType = contractType;
		}

		/// <summary>
		/// Gets the contract type of addable elements.
		/// </summary>
		public Type ContractType { get; private set; }

	}

}

// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System;

namespace Rotorz.ReorderableList {

	/// <summary>
	/// Factory methods that create <see cref="IElementAdderMenuBuilder{TContext}"/>
	/// instances that can then be used to build element adder menus.
	/// </summary>
	/// <example>
	/// <para>The following example demonstrates how to build and display a menu which
	/// allows the user to add elements to a given context object upon clicking a button:</para>
	/// <code language="csharp"><![CDATA[
	/// public class ShoppingListElementAdder : IElementAdder<ShoppingList> {
	///     public ShoppingListElementAdder(ShoppingList shoppingList) {
	///         Object = shoppingList;
	///     }
	///
	///     public ShoppingList Object { get; private set; }
	///
	///     public bool CanAddElement(Type type) {
	///         return true;
	///     }
	///     public object AddElement(Type type) {
	///         var instance = Activator.CreateInstance(type);
	///         shoppingList.Add((ShoppingItem)instance);
	///         return instance;
	///     }
	/// }
	/// 
	/// private void DrawAddMenuButton(ShoppingList shoppingList) {
	///     var content = new GUIContent("Add Menu");
	///     Rect position = GUILayoutUtility.GetRect(content, GUI.skin.button);
	///     if (GUI.Button(position, content)) {
	///         var builder = ElementAdderMenuBuilder.For<ShoppingList>(ShoppingItem);
	///         builder.SetElementAdder(new ShoppingListElementAdder(shoppingList));
	///         var menu = builder.GetMenu();
	///         menu.DropDown(buttonPosition);
	///     }
	/// }
	/// ]]></code>
	/// <code language="unityscript"><![CDATA[
	/// public class ShoppingListElementAdder extends IElementAdder.<ShoppingList> {
	///     var _object:ShoppingList;
	///
	///     function ShoppingListElementAdder(shoppingList:ShoppingList) {
	///         Object = shoppingList;
	///     }
	///
	///     function get Object():ShoppingList { return _object; }
	///
	///     function CanAddElement(type:Type):boolean {
	///         return true;
	///     }
	///     function AddElement(type:Type):System.Object {
	///         var instance = Activator.CreateInstance(type);
	///         shoppingList.Add((ShoppingItem)instance);
	///         return instance;
	///     }
	/// }
	/// 
	/// function DrawAddMenuButton(shoppingList:ShoppingList) {
	///     var content = new GUIContent('Add Menu');
	///     var position = GUILayoutUtility.GetRect(content, GUI.skin.button);
	///     if (GUI.Button(position, content)) {
	///         var builder = ElementAdderMenuBuilder.For.<ShoppingList>(ShoppingItem);
	///         builder.SetElementAdder(new ShoppingListElementAdder(shoppingList));
	///         var menu = builder.GetMenu();
	///         menu.DropDown(buttonPosition);
	///     }
	/// }
	/// ]]></code>
	/// </example>
	public static class ElementAdderMenuBuilder {

		/// <summary>
		/// Gets a <see cref="IElementAdderMenuBuilder{TContext}"/> to build an element
		/// adder menu for a context object of the type <typeparamref name="TContext"/>.
		/// </summary>
		/// <typeparam name="TContext">Type of the context object that elements can be added to.</typeparam>
		/// <returns>
		/// A new <see cref="IElementAdderMenuBuilder{TContext}"/> instance.
		/// </returns>
		/// <seealso cref="IElementAdderMenuBuilder{TContext}.SetContractType(Type)"/>
		public static IElementAdderMenuBuilder<TContext> For<TContext>() {
			return new GenericElementAdderMenuBuilder<TContext>();
		}

		/// <summary>
		/// Gets a <see cref="IElementAdderMenuBuilder{TContext}"/> to build an element
		/// adder menu for a context object of the type <typeparamref name="TContext"/>.
		/// </summary>
		/// <typeparam name="TContext">Type of the context object that elements can be added to.</typeparam>
		/// <param name="contractType">Contract type of addable elements.</param>
		/// <returns>
		/// A new <see cref="IElementAdderMenuBuilder{TContext}"/> instance.
		/// </returns>
		/// <seealso cref="IElementAdderMenuBuilder{TContext}.SetContractType(Type)"/>
		public static IElementAdderMenuBuilder<TContext> For<TContext>(Type contractType) {
			var builder = For<TContext>();
			builder.SetContractType(contractType);
			return builder;
		}

	}

}

// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Rotorz.ReorderableList {

	internal sealed class GenericElementAdderMenuBuilder<TContext> : IElementAdderMenuBuilder<TContext> {

		private static string NicifyTypeName(Type type) {
			return ObjectNames.NicifyVariableName(type.Name);
		}

		private Type _contractType;
		private IElementAdder<TContext> _elementAdder;
		private Func<Type, string> _typeDisplayNameFormatter;
		private List<Func<Type, bool>> _typeFilters = new List<Func<Type, bool>>();
		private List<IElementAdderMenuCommand<TContext>> _customCommands = new List<IElementAdderMenuCommand<TContext>>();

		public GenericElementAdderMenuBuilder() {
			_typeDisplayNameFormatter = NicifyTypeName;
		}

		public void SetContractType(Type contractType) {
			_contractType = contractType;
		}

		public void SetElementAdder(IElementAdder<TContext> elementAdder) {
			_elementAdder = elementAdder;
		}

		public void SetTypeDisplayNameFormatter(Func<Type, string> formatter) {
			_typeDisplayNameFormatter = formatter ?? NicifyTypeName;
		}

		public void AddTypeFilter(Func<Type, bool> typeFilter) {
			if (typeFilter == null)
				throw new ArgumentNullException("typeFilter");

			_typeFilters.Add(typeFilter);
		}

		public void AddCustomCommand(IElementAdderMenuCommand<TContext> command) {
			if (command == null)
				throw new ArgumentNullException("command");

			_customCommands.Add(command);
		}

		public IElementAdderMenu GetMenu() {
			var menu = new GenericElementAdderMenu();

			AddCommandsToMenu(menu, _customCommands);

			if (_contractType != null) {
				AddCommandsToMenu(menu, ElementAdderMeta.GetMenuCommands<TContext>(_contractType));
				AddConcreteTypesToMenu(menu, ElementAdderMeta.GetConcreteElementTypes(_contractType, _typeFilters.ToArray()));
			}

			return menu;
		}

		private void AddCommandsToMenu(GenericElementAdderMenu menu, IList<IElementAdderMenuCommand<TContext>> commands) {
			if (commands.Count == 0)
				return;

			if (!menu.IsEmpty)
				menu.AddSeparator();

			foreach (var command in commands) {
				if (_elementAdder != null && command.CanExecute(_elementAdder))
					menu.AddItem(command.Content, () => command.Execute(_elementAdder));
				else
					menu.AddDisabledItem(command.Content);
			}
		}

		private void AddConcreteTypesToMenu(GenericElementAdderMenu menu, Type[] concreteTypes) {
			if (concreteTypes.Length == 0)
				return;

			if (!menu.IsEmpty)
				menu.AddSeparator();

			foreach (var concreteType in concreteTypes) {
				var content = new GUIContent(_typeDisplayNameFormatter(concreteType));
				if (_elementAdder != null && _elementAdder.CanAddElement(concreteType))
					menu.AddItem(content, () => {
						if (_elementAdder.CanAddElement(concreteType))
							_elementAdder.AddElement(concreteType);
					});
				else
					menu.AddDisabledItem(content);
			}
		}

	}

}

// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using UnityEditor;
using UnityEngine;

namespace Rotorz.ReorderableList {

	internal sealed class GenericElementAdderMenu : IElementAdderMenu {

		private GenericMenu _innerMenu = new GenericMenu();

		public GenericElementAdderMenu() {
		}

		public void AddItem(GUIContent content, GenericMenu.MenuFunction handler) {
			_innerMenu.AddItem(content, false, handler);
		}

		public void AddDisabledItem(GUIContent content) {
			_innerMenu.AddDisabledItem(content);
		}

		public void AddSeparator(string path = "") {
			_innerMenu.AddSeparator(path);
		}

		public bool IsEmpty {
			get { return _innerMenu.GetItemCount() == 0; }
		}

		public void DropDown(Rect position) {
			_innerMenu.DropDown(position);
		}

	}

}

// Copyright (c) Rotorz Limited. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root.

using UnityEngine;

namespace Rotorz.ReorderableList {

	/// <summary>
	/// Interface for a menu interface.
	/// </summary>
	public interface IElementAdderMenu {

		/// <summary>
		/// Gets a value indicating whether the menu contains any items.
		/// </summary>
		/// <value>
		/// <c>true</c> if the menu contains one or more items; otherwise, <c>false</c>.
		/// </value>
		bool IsEmpty { get; }

		/// <summary>
		/// Displays the drop-down menu inside an editor GUI.
		/// </summary>
		/// <remarks>
		/// <para>This method should only be used during <b>OnGUI</b> and <b>OnSceneGUI</b>
		/// events; for instance, inside an editor window, a custom inspector or scene view.</para>
		/// </remarks>
		/// <param name="position">Position of menu button in the GUI.</param>
		void DropDown(Rect position);

	}

}

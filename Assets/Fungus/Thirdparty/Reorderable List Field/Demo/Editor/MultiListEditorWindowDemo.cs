using UnityEngine;
using UnityEditor;
using Rotorz.ReorderableList;
using System.Collections.Generic;

public class MultiListEditorWindowDemo : EditorWindow {

	[MenuItem("Window/Multi List Demo (C#)")]
	private static void ShowWindow() {
		GetWindow<MultiListEditorWindowDemo>("Multi List");
    }

	private class ExampleListAdaptor : GenericListAdaptor<string>, IReorderableListDropTarget {

		private const float MouseDragThresholdInPixels = 0.6f;

		// Static reference to the list adaptor of the selected item.
		private static ExampleListAdaptor s_SelectedList;
		// Static reference limits selection to one item in one list.
		private static string s_SelectedItem;
		// Position in GUI where mouse button was anchored before dragging occurred.
		private static Vector2 s_MouseDownPosition;

		// Holds data representing the item that is being dragged.
		private class DraggedItem {

			public static readonly string TypeName = typeof(DraggedItem).FullName;

			public readonly ExampleListAdaptor SourceListAdaptor;
			public readonly int Index;
			public readonly string ShoppingItem;

			public DraggedItem(ExampleListAdaptor sourceList, int index, string shoppingItem) {
				SourceListAdaptor = sourceList;
				Index = index;
				ShoppingItem = shoppingItem;
			}

		}
		
		public ExampleListAdaptor(IList<string> list) : base(list, null, 16f) {
        }

		public override void DrawItemBackground(Rect position, int index) {
			if (this == s_SelectedList && List[index] == s_SelectedItem) {
				Color restoreColor = GUI.color;
				GUI.color = ReorderableListStyles.SelectionBackgroundColor;
				GUI.DrawTexture(position, EditorGUIUtility.whiteTexture);
				GUI.color = restoreColor;
			}
        }

		public override void DrawItem(Rect position, int index) {
			string shoppingItem = List[index];

			int controlID = GUIUtility.GetControlID(FocusType.Passive);

			switch (Event.current.GetTypeForControl(controlID)) {
				case EventType.MouseDown:
					Rect totalItemPosition = ReorderableListGUI.CurrentItemTotalPosition;
					if (totalItemPosition.Contains(Event.current.mousePosition)) {
						// Select this list item.
						s_SelectedList = this;
						s_SelectedItem = shoppingItem;
					}

					// Calculate rectangle of draggable area of the list item.
					// This example excludes the grab handle at the left.
					Rect draggableRect = totalItemPosition;
					draggableRect.x = position.x;
					draggableRect.width = position.width;

					if (Event.current.button == 0 && draggableRect.Contains(Event.current.mousePosition)) {
						// Select this list item.
						s_SelectedList = this;
						s_SelectedItem = shoppingItem;

						// Lock onto this control whilst left mouse button is held so
						// that we can start a drag-and-drop operation when user drags.
                        GUIUtility.hotControl = controlID;
						s_MouseDownPosition = Event.current.mousePosition;
                        Event.current.Use();
					}
					break;

				case EventType.MouseDrag:
					if (GUIUtility.hotControl == controlID) {
						GUIUtility.hotControl = 0;

						// Begin drag-and-drop operation when the user drags the mouse
						// pointer across the threshold. This threshold helps to avoid
						// inadvertently starting a drag-and-drop operation.
						if (Vector2.Distance(s_MouseDownPosition, Event.current.mousePosition) >= MouseDragThresholdInPixels) {
							// Prepare data that will represent the item.
							var item = new DraggedItem(this, index, shoppingItem);

							// Start drag-and-drop operation with the Unity API.
							DragAndDrop.PrepareStartDrag();
							// Need to reset `objectReferences` and `paths` because `PrepareStartDrag`
							// doesn't seem to reset these (at least, in Unity 4.x).
							DragAndDrop.objectReferences = new Object[0];
							DragAndDrop.paths = new string[0];

							DragAndDrop.SetGenericData(DraggedItem.TypeName, item);
							DragAndDrop.StartDrag(shoppingItem);
                        }

						// Use this event so that the host window gets repainted with
						// each mouse movement.
                        Event.current.Use();
					}
                    break;

				case EventType.Repaint:
					EditorStyles.label.Draw(position, shoppingItem, false, false, false, false);
					break;
			}
		}

		public bool CanDropInsert(int insertionIndex) {
			if (!ReorderableListControl.CurrentListPosition.Contains(Event.current.mousePosition))
				return false;

			// Drop insertion is possible if the current drag-and-drop operation contains
			// the supported type of custom data.
			return DragAndDrop.GetGenericData(DraggedItem.TypeName) is DraggedItem;
        }

		public void ProcessDropInsertion(int insertionIndex) {
            if (Event.current.type == EventType.DragPerform) {
				var draggedItem = DragAndDrop.GetGenericData(DraggedItem.TypeName) as DraggedItem;

				// Are we just reordering within the same list?
				if (draggedItem.SourceListAdaptor == this) {
					Move(draggedItem.Index, insertionIndex);
                }
				else {
					// Nope, we are moving the item!
					List.Insert(insertionIndex, draggedItem.ShoppingItem);
					draggedItem.SourceListAdaptor.Remove(draggedItem.Index);

					// Ensure that the item remains selected at its new location!
					s_SelectedList = this;
				}
			}
		}

	}

	private List<string> _shoppingList;
	private ExampleListAdaptor _shoppingListAdaptor;

	private List<string> _purchaseList;
	private ExampleListAdaptor _purchaseListAdaptor;

	private void OnEnable() {
		_shoppingList = new List<string>() { "Bread", "Carrots", "Beans", "Steak", "Coffee", "Fries" };
		_shoppingListAdaptor = new ExampleListAdaptor(_shoppingList);

		_purchaseList = new List<string>() { "Cheese", "Crackers" };
		_purchaseListAdaptor = new ExampleListAdaptor(_purchaseList);
	}

	private void OnGUI() {
		GUILayout.BeginHorizontal();

		var columnWidth = GUILayout.Width(position.width / 2f - 6);

		// Draw list control on left side of the window.
		GUILayout.BeginVertical(columnWidth);
		ReorderableListGUI.Title("Shopping List");
		ReorderableListGUI.ListField(_shoppingListAdaptor);
		GUILayout.EndVertical();

		// Draw list control on right side of the window.
		GUILayout.BeginVertical(columnWidth);
		ReorderableListGUI.Title("Purchase List");
		ReorderableListGUI.ListField(_purchaseListAdaptor);
		GUILayout.EndVertical();

		GUILayout.EndHorizontal();
	}

}

README
======

List control for Unity allowing editor developers to add reorderable list controls to
their GUIs. Supports generic lists and serialized property arrays, though additional
collection types can be supported by implementing `Rotorz.ReorderableList.IReorderableListAdaptor`.

Licensed under the MIT license. See LICENSE file in the project root for full license
information. DO NOT contribute to this project unless you accept the terms of the
contribution agreement.

![screenshot](https://bitbucket.org/rotorz/reorderable-list-editor-field-for-unity/raw/master/screenshot.png)

Features
--------

- Drag and drop reordering!
- Automatically scrolls if inside a scroll view whilst reordering.
- Easily customized using flags.
- Adaptors for `IList<T>` and `SerializedProperty`.
- Subscribe to add/remove item events.
- Supports mixed item heights.
- Disable drag and/or removal on per-item basis.
- [Drop insertion](<https://youtu.be/gtdPvLaGTNI>) (for use with `UnityEditor.DragAndDrop`).
- Styles can be overridden on per-list basis if desired.
- Subclass list control to override context menu.
- Add drop-down to add menu (or instead of add menu).
- Helper functionality to build element adder menus.
- User guide (Asset Path/Support/User Guide.pdf).
- API reference documentation (Asset Path/Support/API Reference.chm).

Installing scripts
------------------

This control can be added to your project by importing the Unity package which
contains a compiled class library (DLL). This can be used by C# and UnityScript
developers.

- [Download RotorzReorderableList_v0.4.3 Package (requires Unity 4.5.5+)](<https://bitbucket.org/rotorz/reorderable-list-editor-field-for-unity/downloads/RotorzReorderableList_v0.4.3.unitypackage>)

If you would prefer to use the non-compiled source code version in your project,
copy the contents of this repository somewhere into your project.

**Note to UnityScript (*.js) developers:**

UnityScript will not work with the source code version of this project unless
the contents of this repository is placed at the path "Assets/Plugins/ReorderableList"
due to compilation ordering.

Example 1: Serialized array of strings (C#)
-------------------------------------------

    :::csharp
    SerializedProperty _wishlistProperty;
    SerializedProperty _pointsProperty;

    void OnEnable() {
        _wishlistProperty = serializedObject.FindProperty("wishlist");
        _pointsProperty = serializedObject.FindProperty("points");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        ReorderableListGUI.Title("Wishlist");
        ReorderableListGUI.ListField(_wishlistProperty);

        ReorderableListGUI.Title("Points");
        ReorderableListGUI.ListField(_pointsProperty, ReorderableListFlags.ShowIndices);

        serializedObject.ApplyModifiedProperties();
    }

Example 2: List of strings (UnityScript)
----------------------------------------

    :::javascript
    var yourList:List.<String> = new List.<String>();
    
    function OnGUI() {
        ReorderableListGUI.ListField(yourList, CustomListItem, DrawEmpty);
    }
    
    function CustomListItem(position:Rect, itemValue:String):String {
        // Text fields do not like null values!
        if (itemValue == null)
            itemValue = '';
        return EditorGUI.TextField(position, itemValue);
    }
    
    function DrawEmpty() {
        GUILayout.Label('No items in list.', EditorStyles.miniLabel);
    }

Refer to API reference for further examples!

Submission to the Unity Asset Store
-----------------------------------

If you wish to include this asset as part of a package for the asset store, please
include the latest package version as-is to avoid conflict issues in user projects.
It is important that license and documentation files are included and remain intact.

**To include a modified version within your package:**

- Ensure that license and documentation files are included and remain intact. It should
  be clear that these relate to the reorderable list field library.

- Copyright and license information must remain intact in source files.

- Change the namespace `Rotorz.ReorderableList` to something unique and DO NOT use the
  name "Rotorz". For example, `YourName.ReorderableList` or `YourName.Internal.ReorderableList`.

- Place files somewhere within your own asset folder to avoid causing conflicts with
  other assets which make use of this project.

Useful links
------------

- [Rotorz Website](<http://rotorz.com>)

Contribution Agreement
----------------------

This project is licensed under the MIT license (see LICENSE). To be in the best
position to enforce these licenses the copyright status of this project needs to
be as simple as possible. To achieve this the following terms and conditions
must be met:

- All contributed content (including but not limited to source code, text,
  image, videos, bug reports, suggestions, ideas, etc.) must be the
  contributors own work.

- The contributor disclaims all copyright and accepts that their contributed
  content will be released to the public domain.

- The act of submitting a contribution indicates that the contributor agrees
  with this agreement. This includes (but is not limited to) pull requests, issues,
  tickets, e-mails, newsgroups, blogs, forums, etc.
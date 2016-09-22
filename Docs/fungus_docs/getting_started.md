# Getting Started # {#getting_started}
[TOC]

Follow these steps to get up and running with %Fungus quickly, then learn more about what %Fungus can do and how to do it from the other documentation pages and videos.

<!-- **************************************************** -->
# The Flowchart window # {#flowchart_window}
You'll need the %Fungus Flowchart window when working with %Fungus. Open and dock this window somewhere handy by following these steps:

1. Choose menu: ``Tools | %Fungus | Flowchart Window``

![Menu open %Fungus window]

2. Drag-and-drop the Flowchart window to the location you wish to dock it:

![Drag %Fungus window]

3. The Flowchart window is now docked and part of your Unity window layout:

![Docked %Fungus window]

<!-- **************************************************** -->
# Example scenes # {#examples}
Two folders are created when you install %Fungus, the %Fungus features themslves (in folder '%Fungus') and a set of examples (in folder 'FungusExamples').

Examples include Drag and Drop, Sherlock and %Fungus Town:

![%Fungus Examples]

You can use the left-hand side of the Unity Project window to explore each example folder:

![%Fungus Examples Project window]

Alternatively, you can 'filter' the Project view to show all scenes (and no other files) by clicking the scene filter icon to the right of the search bar:

![%Fungus Examples Project window filter scenes]

You can cancel the filter by clicking the 'x' in the search bar:

![%Fungus Examples Project window filter scenes cancel]

<!-- **************************************************** -->
# Running the examples # {#running_examples}
To **load** an example scene, double click the desired example's scene object in the Project window, and the scene should load. For example, this screenshot shows the scene and Flowchart windows when the DragAndDrop example scene has been loaded:

![%Fungus Examples Drag Drop]

To **run** the currently loaded scene (i.e. to enter **Play-mode**), click the Unity 'play' triangle button at the center top of the Unity application window, and then do whatever makes sense in that scene (e..g click/type text/drag-and-drop objects etc.!):

![Unity play scene]

Note: you click the 'play' button a second time to end **Play-mode**.

<!-- **************************************************** -->
# Playmode changes # {#playmode}
As with all Unity projects, you can **change** the properties of gameObjects while a scene is running, but these changes are 'ephemeral' - they only last while the scene is running. As soon as you end play mode the properties of all objects in the Hierarchy will revert to those saved in the Scene file.

This makes it easy to 'tweak' values of objects in **Play-mode**, and then when the desired behaviour is achieved, those values can be set for the saved scene properties.

Values set when Unity is in **Edit-mode** will be saved when you saved your scene (``CTRL-S`` / ``Command-S``, or menu: ``File | Save Scene``).

<!-- **************************************************** -->
# Playmode tint # {#playmode_tint}
Sometimes we can forget we are in Unity **Play-mode**, and then make changes to Hierarchy gameObject values that are then 'fogotton' when we do stop playing the scene. A good way to avoid this problem is to to set a 'tint' to the Unity editor to make it visually very clear to us when we are in **Play-mode**. To add a tint to **Play-mode** do the following:

1. Open the Unity preferences dialog by choosing menu: ```File | Preferences ...```

2. Select the ```Colors``` preferences, and choose a light colored tint (we chose a light green in this case):

![Unity preferences dialog]

3. Close the dialog (changes are saved automatically).

4. When you next enter **Play-mode** you'll see most of the Unity Editor windows turn green (apart from the Game and Flowchart windows):

![Unity Play Mode tinted]

<!-- **************************************************** -->
# Creating a new scene # {#new_scene}
To create a new scene in Unity do the following:

1. Choose menu: ```File | New Scene```

2. Note: if you have any unsaved changes for the current scene you need to either save or abandon them before a new scene can be created.

3. You should now have a shiny new scene, with a Hierarchy containing just one gameObject, a Main Camera. The new scene will have been give the default name "Untitled", which you can see in the title of the Application window:

![New Scene]

4. Good practice is to save your scene (in the right place, with the right name), before creating your work in the scene. Let's save this scene in the root of our project "Assets" folder, naming it "demo1". First choose menu: ```File | Save Scene As...```

5. Choose the location and name (we'll choose folders "Assets" and scene name "demo1"):

![Save Scene As dialog]

6. Once you have successfully saved the scene you should now see the new scene file "demo1" in your Assets folder in the Project window, and you should also see in the Application window title that you are currently editing the scene named "demo1":

![Editing newly saved scene]

<!-- **************************************************** -->
# Menu: Tools | Fungus # {#fungus_menu}
The core %Fungus operations are available from the Unity ```Tools``` menu.

Choose menu: ```Tools | %Fungus``` to see the options available:

![%Fungus Tools menu]

As can be seen, there are 2 submenus, ```Create``` and ```Utilities```, plus the ```Flowchart Window``` action (which reveals the window if already open, or opens a new window if the Flowchart window was not previously opened).

<!-- *********** -->
# Menu: Tools | Fungus | Create # {#create_menu}
The %Fungus Tools ```Create``` submenu offers the following actions:

![%Fungus Tools Create menu]

<!-- *********** -->
# Menu: Tools | Fungus | Utilities # {#utilities_menu}
The %Fungus Tools ```Utilties``` submenu offers the following actions:

![%Fungus Tools Utilties menu]

<!-- **************************************************** -->
# Create a Flowchart # {#create_flowchart}
To create a %Fungus Flowchart do the following:

1. Choose menu: ```Tools | %Fungus | Create Flowchart```

![menu create Flowchart]

2. A new **Flowchart** gameObject should appear in the Hierarchy window.

![new Flowchart gameobject]

3. Select the **Flowchart** gameObject in the Hierarchy window, and you'll see the **Flowchart's** properties in the Inspector Window:

![Flowchart properties]

4. If you have not already displayed the Flowchart Window, you can do so by clicking the Flowchart Window button in the Inspector.

5. As you can see, when a new Flowchat is created a single command Block named "New Block" is automatically created, with the Event handler "Game Started" (so it will start executing %Fungus commands as soon as the scene goes into **Play Mode**).

<!-- **************************************************** -->
# Block properties # {#block_properties}
Let's change the name of the default command Block of a new Flowchart in the Flowchart window to "hello". Do the following:

1. Create a new %Fungus Flowchart (if you haven't already done so).

2. Click to select the Block in the Flowchart window (when multiple blocks are present, the selected one gets a green highlight border).

3. In the Inspector change the text for the Block Name property to "hello". You should see the Block name change in the Flowchart window:

![rename Block]

<!-- **************************************************** -->
# Add a command # {#add_command}
To add a "Say" command to a Block do the following:

1. (setup) Create a new scene, add a %Fungus Flowchart to the scene.

2. Ensure the Block is selected, and you can see its properties in the Inspector, and ensure the name of the Block is "hello".

3. Click the Plus button in the bottom half of the Inspector window, to add a new Command to the Block's properties:

![new command button]

4. Choose menu: ```Narrative | Say```:

![add Say command]

5. Since this Block only has one Command, that command is automatically selected (shown with a green highlight).

6. In the "Story Text" textbox in the bottom half of the Inspector window type in "hello Fugus world":

![story text]

7. Run the scene, and see %Fungus create a dialog window, and output the text contents of your Say command:

![story text output]

[Menu open %Fungus window]: ./getting_started/002_docking/1_menu.png "Menu open %Fungus window"
[Drag %Fungus window]: ./getting_started/002_docking/2_window.png "Drag %Fungus window"
[Docked %Fungus window]: ./getting_started/002_docking/3_docked.png "Docked %Fungus window"
[%Fungus Examples]: ./getting_started/004_examples/1_examples.png "%Fungus Examples"
[%Fungus Examples Project window]: ./getting_started/004_examples/3_project_window.png "%Fungus Examples Project window"
[%Fungus Examples Project window filter scenes]: ./getting_started/004_examples/2_filter_scenes.png "%Fungus Examples Project window filter scenes"
[%Fungus Examples Project window filter scenes cancel]: ./getting_started/004_examples/4_filter_scenes_cancel.png "%Fungus Examples Project window filter scenes cancel"
[%Fungus Examples Drag Drop]: ./getting_started/004_examples/5_drag_drop.png "%Fungus Examples Drag Drop"
[Unity play scene]: ./getting_started/004_examples/6_drag_running.png "Unity play scene"
[Unity preferences dialog]: ./getting_started/005_highlight_play_mode/1_prefs_tint.png "Unity preferences dialog"
[Unity Play Mode tinted]: ./getting_started/005_highlight_play_mode/2_green_play_mode.png "Unity Play Mode tinted"
[New Scene]: ./getting_started/006_new_scene/1_default.png "New Scene"
[Save Scene As dialog]: ./getting_started/006_new_scene/2_save_as.png "Save Scene As dialog"
[Editing newly saved scene]: ./getting_started/006_new_scene/3_saved_scene.png "Editing newly saved scene"
[%Fungus Tools menu]: ./getting_started/007_tools_menu/3_fungus_tools.png "%Fungus Tools menu"
[%Fungus Tools Create menu]: ./getting_started/007_tools_menu/1_tools_create.png "%Fungus Tools Create menu"
[%Fungus Tools Utilties menu]: ./getting_started/007_tools_menu/2_tools_utilities.png "%Fungus Tools Utilities menu"
[menu create Flowchart]: ./getting_started/008_create_flowchart/1_tools_create.png "menu create Flowchart"
[new Flowchart gameobject]: ./getting_started/008_create_flowchart/2_flowchart_gameobject.png "new Flowchart gameobject"
[Flowchart properties]: ./getting_started/008_create_flowchart/3_flowchart_properties.png "Flowchart properties"
[rename Block]: ./getting_started/009_rename_block/1_rename.png "rename Block"
[new command button]: ./getting_started/010_say_command/1_plus.png "new command button"
[add Say command]: ./getting_started/010_say_command/2_narrative_say.png "add Say command"
[story text]: ./getting_started/010_say_command/3_hello_fungus_world.png "story text"
[story text output]: ./getting_started/010_say_command/4_scene_running.png "story text output"

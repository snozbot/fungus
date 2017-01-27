# Saving and Loading # {#save_system}
[TOC]

# Introduction # {#save_introduction}

**N.B. The Save System is currently in Beta to get feedback from the community. There will probably be breaking changes in future updates as we improve the design.**

The %Fungus Save System provides an easy way to save and load the execution state of Flowcharts in your game. 

This [tutorial video](https://www.youtube.com/watch?v=Bd4RDcCc0lE&t=2s&list=PLiMlyObJfJmUohJ_M2pJhtrNKuNECo2Uk&index=20) shows how you can quickly add saving functionality to a %Fungus game.

The save system works by storing a series of Save Points as the player progresses through the game, to build up a Save History. A basic Save Menu UI is provided to allow the player to Save, Load, Restart, Rewind and Fast Forward through the Save History.

![img save_history]

A Save Point is created by executing a Save Point command in a Flowchart. When a Save Point is loaded back later, execution resumes from immediately after this Save Point command. The save system looks after restoring the state of Flowchart variables at each Save Point and you can also be notified when a particular Save Point has loaded via the Save Point Loaded event handler.

If you are using the old commands for saving individual variables (Set Save Profile, Save Variable, Load Variable), they still work but they’re separate to this new save system.

# The Save Game example scene # {#save_example_scene}

To see how the save system works, open the example scene *FungusExamples/Savegame/SaveGame.unity*

Press play in Unity and click through the short story. The Save Menu can be accessed at any time by clicking the small gears icon in the top right. Try saving, loading, rewinding, fast forwarding and restarting at different points as you play through the story. Also try saving the game in the middle of the story and stopping the game. Press Play again and notice that the game resumes where you left off.

The Save Menu also supports an auto-saving mode where the game is saved to disk at every Save Point. You can enable this by selecting the Save Menu object and selecting the Auto Save property. The Save and Load buttons are disabled when using Auto Save.

The following are the key elements that are used to implement saving in this example scene.

## The Save Menu UI ## {#save_menu_ui}

![img save_menu]

The Save Menu object can be seen In the root of the hierarchy window. This object controls the UI menu that the player uses to interact with the save system. The Save Menu is a singleton object that persists across scene loads, so you only need to add it once in the first scene of your game.

To add a Save Menu to your game, select *Tools > %Fungus > Create > Save Menu*.

## Creating Save Points ## {#save_creating_save_points}

Save Points are created by executing Save Point commands in a Flowchart.

To see this in the example scene, ensure the Flowchart window is displayed (via *Tools > %Fungus > Flowchart Window*), then select the Flowchart object in the hierarchy window and select each of the Blocks in the Flowchart. The first command in each Block is a Save Point command (added via *Flow > Save Point*).

When each Save Point command executes, it adds a new Save Point to the Save History. When you load a previously saved game, execution resumes from immediately after the Save Point command that created that Save Point.

In the example scene, select the ‘Start’ Block in the Flowchart window, and select the Save Point command at the top of the command list. Notice that the Is Start Point property is enabled and that this is the only Save Point command in the Flowchart which has this option enabled. There should only ever be one Start Point in a scene.

When you start a new game, %Fungus looks for a Save Point command with the Is Start Point property enabled and executes it. When loading a previously saved game, %Fungus starts execution at the relevant Save Point command and ignores the start point. 

This means that if your game supports saving then you should always have exactly one Save Point command with the Is Start Point property enabled in every scene.

N.B. The Game Started event handler will fire for both new games and loaded games which is generally not what you want, so avoid using it in games that support saving.

## Handling Save Point Loaded events ## {#save_point_loaded_events}

You often need to do some additional work when a saved game loads up to ensure the scene is in the correct state. E.g. The camera might need to be moved to the appropriate location or a certain music track played at this point in the game. An easy way to do this is via the Save Point Loaded event handler.

In the example scene, select the ‘Play Music 1’ Block in the Flowchart, and see it has a Save Point Loaded event handler. This will execute the Block when any of the Save Points in the Save Point Keys list loads. In this case we simply play the correct piece of music for this part of the game, but you could do any setup needed here.

The Save Point Loaded event handler will also fire when a matching Save Point command executes (if the Fire Event property is enabled). This allows you to place all the scene setup commands into a single shared Block which will be called when a Save Point command is first reached or when loading a previously saved game at that Save Point.

## Saving Flowchart variables ## {#save_flowchart_variables}

Each Save Point can store the state of Flowchart variables at that point in time. You use a Save Data object to let the save system know which Flowcharts are to be included in this. Note that only Boolean, Integer, Float and String variables are saved at present.

In the example scene, the Save Data object can be seen in the root of the hierarchy window. The Flowcharts property contains a list of the Flowchart objects to be saved in this scene. 

To add a Save Data object to your scene, select *Tools > %Fungus > Create > Save Data*. You can add as many Flowcharts as you like to the list, but make sure that each one has a unique name (e.g. Flowchart1, Flowchart2, etc.) or loading won’t work correctly.

If you are interested in extending the save system to support saving other types of data (besides Flowchart variables), you can either modify or subclass the SaveData component to achieve this.

# The Save Menu # {#save_menu_ui}

The Save Menu is a simple UI which allows players to interact with the %Fungus save system. This section explains what each button does and how to configure the Save Menu properties.

## Save Menu properties ## {#save_menu_properties}

There are 3 main properties that you might want to configure in the Save Menu.

- Load On Start: Automatically load the previously saved game on startup.
- Auto Save: Automatically save the game to disk at every Save Point. When this option is enabled the Save and Load buttons are disabled.
- Restart Deletes Save: Delete the save data from disk when the player restarts the game. This is useful when testing your game to ensure you’re starting from a blank save state.

## Save button ## {#save_save_button}

Pressing the Save button causes the current Save History to be serialized to JSON text and written to persistent storage via the PlayerPrefs class.

## Load button ## {#save_load_button}

Pressing the Load button causes the previously stored JSON data to be deserialized and used to populate the Save History.  The most recent Save Point is then used to restore the game state in the following order. 
- Load the scene stored in the Save Point (even if it’s the currently loaded scene).
- Restore Flowchart variables to the saved values
- Call Save Point Loaded event handlers and start Flowchart execution after the appropriate Save Point command.

## Rewind and Fast Forward buttons ## {#save_rewind_button}

The Rewind and Fast Forward buttons allow you to move backwards and forwards between Save Points in the Save History. 

Each move simply loads the Save Point stored at a particular point in the Save History. This by itself doesn’t change the Save History or write anything to persistent storage. However, if you rewind to an earlier Save Point and start playing again, the next time a Save Point command is executed it will cause all Save Points that are further ahead in time to be discarded permanently.

## Restart button ## {#save_restart_button}

The Restart button clears the Save History and loads the start scene. The start scene is the scene that was active when the Save Menu was first initialized.

# Game Startup # {#save_game_startup}

Remember that the player can choose to load or restart the game at any time. Follow these simple rules to ensure game startup is handled correctly in all cases.

1. Every scene in your game should have exactly one Save Point command with the Is Start Point property enabled. If you have multiple scenes in your game, make sure each one has a start Save Point defined and that it’s the first command executed in the scene.
2. Avoid using the Game Started event handler. This will only work correctly the first time the game is played, not after a saved game is loaded. After loading a saved game, you want execution to start from the Save Point, not at the beginning of your Flowchart again.
3. Use the Save Point Loaded event handler when you want to execute a Block when specific Save Points are loaded. These event handlers are called before execution resumes at the Save Point command, so it gives you a chance to do setup work before gameplay resumes.

# Terminology # {#save_terminology}

## Save Point ## {#save_term_save_point}

A Save Point is a snapshot of the state of the game at a point in time. Each Save Point records the current scene, the current point of Flowchart execution (i.e. at the Save Point command) and the current values of Flowchart variables. Only Boolean, Integer, Float and String variables are saved at present. 

## Save Point command ## {#save_term_save_point_command}

The Save Point command is used in a Flowchart to create a Save Point at that point in the execution. Each individual Save Point command should have a unique Save Point Key. The Resume On Load option causes execution to resume from this point after the Save Point is loaded.

## Save Point Key ## {#save_term_save_point_key} 

A Save Point Key is a unique string identifier for a single Save Point. By default, the name of the parent Block is used for the Save Point Key, but you can also use a custom key if required (e.g. multiple Save Point commands in a single Block). 

N.B. Each key must be unique per scene or loading won’t work correctly!

## Save History ## {#save_term_history}

The Save History contains a list of previously recorded Save Points, stored in chronological order. When a Save Point command is executed, a new Save Point is created and appended to the Save History.

To visualize the Save History at runtime, expand the Save Menu object in the hierarchy, select Save Menu > Panel > Debug View and enable the gameobject. A summary of the Save Points in the Save History will be displayed in a text window.

[img save_menu]: ./save_system/save_menu.png
[img save_history]: ./save_system/save_history.png
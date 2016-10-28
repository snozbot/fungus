Changelog {#changelog}
=========
[TOC]

v3.4.0 {#v3_4_0}
======

## Known Issues
- FungusLua generates runtime errors running Lua when using .NET scripting backend. The IL2CPP backend works fine.

## Added
- Added accessors for GameObject and Transform variables in Flowchart.cs
- Added Flowchart.HasVariable() and GetVariableNames() - thanks murnyipl!
- Added WaitForFrames property to GameStarted event handler. Default is wait for 1 frame (should reduce startup order issues).

## Changed
- Upgraded to Moonsharp 2.0. Moonsharp source code is now included in Fungus/Thirdparty/MoonSharp
- Removed MoonSharp assembly from link.xml
- Converted MoonSharp line endings to OSX for consistency with rest of project.
- Updated documentation for Lua debugger (using VS Code)
- Removed LuaEnvironment Remote Debugger option. A VS Code debug server always starts when running in the editor.
- SayDialog now supports full variable substitution when setting character names.

## Fixed

- Fixed Command properties not copied when copying commands #546
- Fixed PODTypeFactory and FungusPrefs classes are not registered #549
- Fixed for compile errors on .NET Core scripting backend
- Fixed LuaBindings registered types in example scenes
- Fixed MoonSharp warning when not building for an AOT platform
- Fixed minor issues in some example scenes (e.g. missing sprite refs)

v3.3.0 {#v3_3_0}
======

## Known Issues
- FungusLua does not work in WebGL builds due to issues in MoonSharp 1.8.0.0
- Forum thread: http://fungusgames.com/forum/#!/general:fungus-lua-and-web-gl-uni
- FungusLua does not compile for Windows Store platform using .net scripting backend. Use the IL2CPP backend instead.

## Added
- Added test for StopTweens does not stop a Tween with loop enabled #529
- Added signals (pub-sub system) for Writer and Block events #539
- All interfaces now have their own source files.
- Added monodevelop project for editing docs files.
- Added Flip option (<<< and >>>) to conversation system #527
- Added WaitFrames command to wait for a number of frames
- Added GetToggleState, SetToggleState commands and ToggleChanged event handler
- Added Writer.Paused property to pause a writer from code.

## Changed

- Tidied up Fungus folder structure to organize scripts more logically
- Migrated documentation to use Doxygen for help and API docs
- Lots of misc improvements to documentation
- Updated to MoonSharp 1.8.0.0
- Documented using string facing parameter in stage.show() Lua function.
- Documented <<< and >>> tags for conversation system.
- Documented all public members for API docs.
- All serialized fields are now protected, exposed via public properties as needed.
- Moved all enums to namespace scope.
- Moved global constants to FungusConstants static class.
- Moved editor resources to the main resources folder
- Fungus editor code moved to Fungus.EditorUtils namespace
- Convert singletons to use a single FungusManager singleton #540
- Renamed CameraController to CameraManager and MusicController to MusicManager
- Changed float constant comparisons to use Mathf.Approximately
- Added #region Public members to all non-editor classes
- StringFormatter, TextTagParser and FungusPrefs classes are now static
- Merged MenuDialog extension methods (used for Lua) with main MenuDialog class.
- Change all public methods to use virtual
- Removed all unnecessary using statements.
- All class and member comments use standard c# xml comment style
- Replaced foreach loops with for loops (avoids allocation for iterator)
- Added changelog to Doxygen documentation

## Fixed

- Fixed Setting facing in lua only works if portraits are set to “FRONT” #528
- Fixed Say command completes instantly after menu choice #533
- Fixed broken mouse pointer in WebGL build of Drag and Drop
- Fixed ObjectField nulls reference if object is disabled #536
- Updated Unity Test Tools to v1.5.9
- Fixed missing Process class error in Unity5.5b3
- Fixed Spine.Unity namespace problem in integration scripts
- Fix Regex for character names with "." & "'" #531 (thanks to Sercan Altun)
    Old Regex expression did not capture Character names with "." and "'". As a result characters with names like "Mr. Jones" or "Ab'ar" were not registering correctly.
- Fixed Lua setlanguage() function
- Fixed namespace issue in Spine integration.
- Fixes all integration tests to pass when run on Windows Standalone.
- Fixed Block inspector displayed for inactive flowchart #544

v3.2.0 {#v3_2_0}
======

## Known Issues
- FungusLua does not work in WebGL builds due to issues in MoonSharp 1.6.0.0
   Forum thread: http://fungusgames.com/forum/#!/general:fungus-lua-and-web-gl-uni

## Added
- Added choose() and choosetimer() Lua functions for displaying list of menu options.
- Added Conversation command and Lua function to perform long dialogue exchanges in a single command.
- Added new Conversation examples

## Changed
- Clickable2D and Dragable2D components can now use legacy input or EventSystem events / raycasts. 
- Added DragAndDrop(EventSystem) example scene to show how to use the EventSystem option.
- Made it easier to resize the default SayDialog
- Updated Narrative examples to use the easier choose() function instead of menu()
- Force MenuDialog to become active when using AddOption from Lua.
- Converted tabs to spaces in all source files
- Ensure the character cache is populated before accessing it
- Added Serializable attribute to all variable classes.

## Fixed
- Added link.xml file to fix FungusLua not running on iOS builds
- Portrait hide bug in Conversation Function #526
- Unresponsive SayDialog after ClearMenu or *.StopAllBlocks(); #518
- Can't select a Public variable from another flowchart #522
- NullReferenceException with nameText in SayDialog #517
- StackOverflowException in Writer with customized text object #516
- stage.Show() not fading in a previously faded out character if portrait hasn't changed.
- Missing .Value on _parameterName property.
- Fixed missing component warnings
- Updated MoonSharp to v1.6.0.0 (this time without changing the meta files).

v3.1.0 {#v3_1_0}
======

## Added

- Flowchart automatically registers with LuaEnvironment for ExecuteLua commands #485
- Clickable2D and Draggable2D can now use EventSystem events to block clicks on UI elements. Added new drag and drop demo scene to illustrate.

## Changed

- Default dialog image is now sliced, so can be resized to any width / height required.
- Upgraded MoonSharp to v1.6.0.0
- Added [System.Serializable] attribute to all Variable classes.

## Fixed

- Default dialog box now fits in a 5:4 ratio display #515
- Dialog input causing an exception if no Event System is present in scene.
- Missing module variable on round() function
- Menu() Lua function only works once #511
- Compile error for folks upgrading using the Fungus 3 unitypackage

v3.0.0 {#v3_0_0}
======

Major release with powerful new Lua scripting support and many small improvements and fixes.
This release should be backwards compatible with projects created using Fungus 2. If you have any upgrading issues let us know on the forum.

Many thanks to the amazing Fungus community for all the suggestions, bug reports and encouragement!

Awesome github contributors:
- Leah Lee: https://github.com/lealeelu
- Gerardo Marset: https://github.com/ideka
- Konrad Gadzina: https://github.com/FeniXb3
- Kal O' Brien: https://github.com/kalenobrien15
- Hawmalt: https://github.com/hawmalt

## Added
- FungusLua: Lua scripting support for Fungus via wrapper components for using MoonSharp in Unity. #281, #317, #334, #237, #235, #232, #224
	- LuaEnvironment component: Execution environment for running Lua scripts.
	- LuaUtils component: Extends LuaEnvironment with lots of useful features.
	- LuaBindings: Maps Unity objects & components to Lua variables for use in Lua scripts.
	- LuaScript: Runs Lua code from a text file or from a string property.
	- LuaStore: Stores variables in a global table which persists across scene loads.
	- FungusModule: A set of utility functions for scripting Unity and Fungus from Lua.
	- FungusPrefs: An improved version of PlayerPrefs that can be easily used from Lua.
	- ExecuteHandler: Listens for any standard Unity event and calls a method on a component in the gameobject. #247
	- ExecuteLua command: Run some Lua script in a Fungus command. Return values can be stored in Fungus variables.
	- PODTypeFactory: Utility factory class for instantiating Plain-Old-Data (POD) types like Color, Vector3, etc.
    - Lots of FungusLua example scenes
    - Fungus documentation now has an extensive section on LuaScripting.
- StringDataMulti: Like StringData, but uses a multi-line textbox in the inspector.
- StopBlock command: Stop executing the named block.
- Improved string substitution system. Now works with Lua global variables and Lua string table, as well as Flowchart variables.
- Extend the string substitution system yourself using the new ISubstitutionHandler interface.
- Added TaskManager library to Thirdparty folder. Allows better control over coroutine execution.
- Show Line Numbers option in Flowchart. Shows the command index in the inspector (off by default). #231
- Play Animation State command. Plays an animation state directly without a transition. #378
- Open URL command #382
- Links to community articles in the help docs #385
- InfoText.cs component for displaying help information in the top-left of screen
- "Play from Selected" and "Stop All and Play" context menu options in Block command list
- Added Command Index property to Call command
- LuaStore example scene to demonstrate persisting Lua variables between scene loads
- Use stage.show(), stage.showPortrait & stage.hide() to control stage & portraits from Lua #490
  See FungusExamples/FungusLua/Narrative/PortraitController.unity example scene
- Portrait functionality moved to new PortraitController utility class for easier scripting.
- Say and Menu Dialogs now support standard input manager (joystick / controller support) #507 #210
- Menu options can now be picked with keyboard only
- Fast forward using Shift in Say Dialogs is now done using the Cancel input (Escape key by default).

## Changed
- Draggable sprite anchors at exact point user clicked.
- Replaced string with StringData, int with IntegerData, etc. in many command properties. Use variables or constants.
- Block.Execute renamed to Block.StartExecute, can now specify a command index to start at.
- Say command: Set the Character using an object field or the dropdown menu. Can now select Character prefabs.
- Improved Flowchart UpdateVersion system
- Portrait image is now hidden at startup in SayDialog
- Use DialogAudio volume property for starting volume on voiceover audio
- WriterAudio now respects the volume property in all cases
- Added short open source license info header to all source files
- SetAudioVolume.waitUntilFinished property #495
- String substitution uses StringBuilder to avoid string allocations (reduce garbage collection) #490
- Embed string substitution keys in substitution text (recursive substitution up to 5 levels) #488

## Fixed
- SetDraggable2D filename now matches class name.
- Unity 5.4 beta errors & warnings
- CsvParser.cs and InvokeMethod lineendings should be consistent with rest of project.
- Faulty indent levels when inspector is not displayed #380
- Hide Portrait before Show Portrait breaks portrait system #384
- Private variable values being reset with multiple flowcharts #389
- Stage objects blocking raycasts #391
- Writer voiceover clip always stops when text stops #393
- Size tag in UI text is not supported #400
- Clickable sprites can be clicked through UI objects #377
- Don't destroy sprite objects in Scene Loader #386
- Add links to community articles in the help docs #385
- Control volume bug #464
- Unity Test Tools compile errors in Unity 5.0
- Edge of inspector window clipped incorreclty in Unity 5.4 beta #425
- Child Object gets deleted when having a flowchart on parent and child. #475
- Fixed command summary incorrect for Fade UI command #486
- No Music clip selected error summary in Play Sound command
- Jump command properties incorrect when block duplicated #504
- menu() Lua command interactable param has no effect #493
- Set Anim Integer/Float/Bool lose property settings #492
- Can't select ExecuteBlock from Unity Event #496
- Fixed aliased commandIndex property in Call command.

## Other closed issues
- GameObjects get duplicated when flowchart is on a different scene #373
- TextMesh Pro integration #214
- Clickable3D component #195

v2.4.0 {#v2_4_0}
======

## Added
- FungusLua: Lua scripting support for Fungus via wrapper components for using MoonSharp in Unity. #281, #317, #334, #237, #235, #232, #224
	- LuaEnvironment component: Execution environment for running Lua scripts.
	- LuaUtils component: Extends LuaEnvironment with lots of useful features.
	- LuaBindings: Maps Unity objects & components to Lua variables for use in Lua scripts.
	- LuaScript: Runs Lua code from a text file or from a string property.
	- LuaStore: Stores variables in a global table which persists across scene loads.
	- FungusModule: A set of utility functions for scripting Unity and Fungus from Lua.
	- FungusPrefs: An improved version of PlayerPrefs that can be easily used from Lua.
	- ExecuteHandler: Listens for any standard Unity event and calls a method on a component in the gameobject. #247
	- ExecuteLua command: Run some Lua script in a Fungus command. Return values can be stored in Fungus variables.
	- PODTypeFactory: Utility factory class for instantiating Plain-Old-Data (POD) types like Color, Vector3, etc.
    - Lots of FungusLua example scenes
    - Fungus documentation now has an extensive section on LuaScripting.
- StringDataMulti: Like StringData, but uses a multi-line textbox in the inspector.
- StopBlock command: Stop executing the named block.
- Improved string substitution system. Now works with Lua global variables and Lua string table.
- Extend the string substitution system yourself using the new ISubstitutionHandler interface.
- Added TaskManager library to Thirdparty folder. Allows better control over coroutine execution.
- Show Line Numbers option in Flowchart. Shows the command index in the inspector (off by default). #231
- Play Animation State command. Plays an animation state directly without a transition. #378
- Open URL command #382
- Links to community articles in the help docs #385
- InfoText.cs component for displaying help information in the top-left of screen
- "Play from Selected" and "Stop All and Play" context menu options in Block command list

## Changed
- Draggable sprite anchors at exact point user clicked.
- Replaced string with StringData, int with IntegerData, etc. in many command properties.
- Bock.Execute renamed to Block.StartExecute, can now specify a command index to start at.
- Say command: Set the Character using an object field or the dropdown menu. Can now select Character prefabs.
- Improved Flowchart UpdateVersion system
- Portrait image is now hidden at startup in SayDialog
- Use DialogAudio volume property for starting volume on voiceover audio
- WriterAudio now respects the volume property in all cases

## Fixed
- SetDraggable2D filename now matches class name.
- Unity 5.4 beta errors & warnings
- CsvParser.cs and InvokeMethod lineendings should be consistent with rest of project.
- Faulty indent levels when inspector is not displayed #380
- Hide Portrait before Show Portrait breaks portrait system #384
- Private variable values being reset with multiple flowcharts #389
- Stage objects blocking raycasts #391
- Writer voiceover clip always stops when text stops #393
- Size tag in UI text is not supported #400
- Clickable sprites can be clicked through UI objects #377
- Don't destroy sprite objects in Scene Loader #386
- Add links to community articles in the help docs #385
- Control volume bug #464
- Unity Test Tools compile errors in Unity 5.0
- Edge of inspector window clipped incorreclty in Unity 5.4 beta #425

## Other closed issues
- GameObjects get duplicated when flowchart is on a different scene #373
- TextMesh Pro integration #214
- Clickable3D component #195

v2.3.1 {#v2_3_1}
======

## Fixed
- Can't click on Say Dialog when a Menu Dialog is active #374
- Set Audio Pitch: OnComplete not called when duration = 0 #369
- Fade To View can sometimes not work in Unity 5.3 #370

v2.3.0 {#v2_3_0}
======

## Added
- SetDraggable2D command #191
- WaitInput command #276
- Fade UI command to fade UI objects #349
- Read Text File command to read a text file into a string variable #344
- Added Set Audio Pitch command #340
- 'Center View' button to center Flowchart window on all Blocks #302
- Added Clear Menu command #300
- Set Slider Value command #297
- Stop Flowchart command #289
- Integration with Esoteric Spine animation system (available in Fungus/Integrations/Spine folder)

## Changed
- Added null checks in Flowchart variable accessors
- Set Say Dialog property for characters
- Can now specify the gameobject to shake for punch tag in Writer component
- PlayMusic command has a loop property
- Updated reorderable list control to v0.4.3
- Updated to LeanTween 2.30
- Added HasExecutingBlocks() and GetExecutingBlocks() to Flowchart class
- Remove unused Text Width Scale property on Say Dialog
- Can now specify a camera to use with Camera command (not just main) #319 #307
- Can now disable Camera Z being forced by View commands #319
- Text tags now support multiple parameters.
- Write command now works with Text Mesh Pro text objects.
- Writer.InstantComplete property controls click-to-complete writing.
- Shake tag shakes camera instead of Writer game object.

## Fixed
- Ensure parentBlock is set when block executes #320
- While loop with following If doesn't loop correctly #354
- Auto cross fade music in Play Music command #199
- Play Music call doesn't restart if same music already playing
- Play Music call doesn't restart if same music already playing
- Concurrent Say commands on same Say Dialog should interrupt #356
- CustomGUI class not in Fungus namespace #353
- Whitespace trimming after {wc} & {c} tags #346
- iTween commands on the same gameobject can conflict #342 #303
- Show Sprite should affect child sprites #338
- Null reference if command classes have same name #312
- End command indents when not matched with an If #308
- Draggable objects behaving incorrectly #310
- Inactive localizeable game objects are now also cached #322
- SceneManager warnings in Unity 5.3
- Fixed Windows store build 268
- Fixed Writer beep timing issues in WebGL #295
- Removed say command from Flowchart prefab

v2.2.2 {#v2_2_2}
======

## Added

- Stop Flowchart command and Flowchart.StopAllBlocks() method #288

## Changed

- Only the first Flowchart added to a scene will have a default GameStarted event handler.
- Null items in variable list are cleaned up on enable.

## Fixed

- Flowchart objects break when made into a prefab #275
- Localization language does not persist between scenes #271
- Voiceover clips not playing correctly in Say command #273
- Changing portrait facing flips incorrectly #190
- Variable substitution in Menu command #263
- Null variable reference when substituting variables #278    
- Removed empty gameobject in sherlock demo
- Removed unused GameStarted component from built-in Flowchart prefab

v2.2.1 {#v2_2_1}
======

## Added

- Set Interactable command to control UI button, input field, etc. interactivity #178
- Button Clicked event handler to execute a block when a UI button is clicked
- End Edit event handler to execute a block when user presses enter in an input field
- Hide specific commands & categories in the Add Command menu by setting the Hide Commands property on Flowchart

## Changed

- Improved Enter Name example to demonstrate the new Write, Get Text, Set Text, Set Interactable, etc. commands.
- Removed punctuation pause for last character in a sentence #168
- Removed unsupported 'shiver' item from tag help description in Say and Write commands
- Moved LICENSE and README inside the Fungus folder #185

## Fixed
            
- Concurrent Say calls cause Say Dialog to freeze up #156
- Null reference error after editing Flowchart code #181
- Markup text visible if rich text not enabled when using Write command #176    
- ControlAudio waitUntilFinished property doesn't wait for correct time #174
- Removed legacy hidden objects in example scenes #159
- Undo for delete command doesn't work #161
- Flowchart in The Hunter example has Hide Components set to false #150

v2.2.0 {#v2_2_0}
======

## Added

- Write command for writing text to any text object
- Writer, Writer Audio components for writing text to any text object with lots of new configuration options.
- Say Dialog class has been split into Say Dialog, Dialog Input components
- Dialog Input supports multiple input types; clicks & key presses
- Can hold down shift while pressing a key to fast forward through dialog text
- Say Dialog now adjusts Story Text rect when character image is visible.
- Added Stop() method to Say Dialog and Writer to cancel writing immediately
- Fullscreen command. https://trello.com/c/aA5GQlua
- InputText example in FungusExamples
- Localization support for the Set Text and Write commands.
- Custom commands can integrate with the localisation system by implementing the ILocalizable interface.
- Invoke Event command for calling script methods with a parameter using Unity's EventSystem.
- Invoke Method command for calling script methods with multiple parameters using reflection.
- Can now use UnityEvent variables in custom commands
- Flowchart versioning and initialization
- Menu command now has an Interactable property to add disabled menu options. https://trello.com/c/bFjmGfBc
- Set Sprite Order command. https://trello.com/c/5yZ88Rh0
- Added Set Sorting Layer command to set renderer sorting layer
- Set Mouse Cursor command and drag and drop support
- Constructors and implicit operators for all Fungus variable types
- Quit command to quit application - https://trello.com/c/qIVLgrDx
- New Fungus/UI command category
- Added ComboBox to Thirdparty folder
- Added Tools > Fungus > Create > Fungus Logo menu option

## Changed

- Simplifed Say Dialog hierarchy structure to be easier to customise
- Say Dialog continue image is now a UI button
- Say Dialog automatically fades in / out when writing
- Updated The Facility example to work with new Say Dialog design
- Updated Drag And Drop example to use new Set Mouse Cursor command
- Improved rendering efficiency of Block inspector window.
- Improved shift-selecting commands in Block inspector.
- Portrait command now moves portraits using world position instead of anchored position    
- Portrait cross fading is now done using image alpha instead of a custom shader
- Stage Show / Hide is now done by tweening the CanvasGroup alpha.
- Portrait MoveSpeed is now MoveDuration
- Portrait WaitUntilFinished option now works robustly
- Additional flowcharts added to the scene no longer have their initial block start with the default 'Game Started' event.
- Command array properties can now be flagged to use the reorderable list control
- Custom commands can override IsPropertyVisible() to hide specific properties as needed.
- SetActive command shows state in summary
- Block Event Handler for additional Flowcharts now defaults to none.
- Can assign default values to public Fungus variable properties 
- Can access Fungus variable values directly without using .Value accessor.
- Say Command fade in / out replaced with automatic fading and Fade When Done property
- Get Text and Set Text command now work on any text object (UI text, UI input fields & 3D text mesh)

## Fixed

- Shake Camera command timing issue #137
- CSV parser doesn't handle Windows-style line endings #131
- Intermittent init order issue with caching localizeable objects
- Control Audio stopping all untagged audiosources #132
- Intermittent null reference error from command editors
- Draggable objects don't return to start pos if drag completes #130
- Moving and fading portraits at the same time
- Character portrait images have display artefacts #92
- Duplicate Block option does not do a deep copy #129
- Event System not created after Load Scene #121
- Voice over audio only works every second time #126
- Editing multiline command properties in block inspector.
- Compile error when Fungus is used with "Draw On Screen" asset #120
- Null reference exception when spawning Fungus objects in Unity 5.1
- Story text box width is restored when using a character with no portrait #141 
- Block inspector window resizes when using cut, copy, paste shortcuts #145
- Objects spawned from Tools > Fungus > Create menu center correctly in Unity 5.1.3f

## Process Changes

We are now using the Unity Test Tools framework for unit and integration testing.
All new features and bug fixes now have automated tests where possible.

## Upgrade Notes

We made a LOT of improvements to the Say Dialogs in this release. Unfortunately these changes are likely to break existing customised Say Dialogs. If you upgrade an existing Fungus project and find the Say Dialog are no longer working you will need to delete your Say Dialogs and create new ones.

v2.1.0 {#v2_1_0}
======

First release on Unity Asset Store!

## Added

- Added Flowchart.FindBlock so you can check if a Block is executing before you try to execute it.
- Cleanup any unreferenced components in Flowchart when scene loads

## Changed

- Call Method can now execute after a delay
https://trello.com/c/a333r2QA

- Can now control swipe pan speed in Start Swipe command
https://trello.com/c/TmG9SiIa

- Improved layout of Flowchart name and description
https://trello.com/c/vwnzaOh2

## Removed

- Removed obsolete commands and other dead code

## Fixed
- Fixed using iTween and Portrait commands in same Flowchart causes null exception #116
- Fixed whitespace and newlines not being trimmed after {wc} & {c} tags in story text #115
- Fixed Standard Text import strips out newline characters #114
- Fixed {x} text tag causing a null reference error #113
- Fixed Flowchart.ExecuteBlock() not being usable with UI events (e.g. a UI button event) #112
- Fixed Set Draggable command causing null exception error #111
- Fixed conflicting Block & command item ids (was breaking localization text export) #110

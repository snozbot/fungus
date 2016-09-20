# Scripting commands {#scripting_commands}

## Comment
Use comments to record design notes and reminders about your game.

Property | Type | Description
 --- | --- | ---
Commenter Name | System.String | Name of Commenter
Comment Text | System.String | Text to display for this comment

## Call Method
Calls a named method on a GameObject using the GameObject.SendMessage() system.

Property | Type | Description
 --- | --- | ---
Target Object | UnityEngine.GameObject | Target monobehavior which contains the method we want to call
Method Name | System.String | Name of the method to call
Delay | System.Single | Delay (in seconds) before the method will be called

## Debug Log
Writes a log message to the debug console.

Property | Type | Description
 --- | --- | ---
Log Type | Fungus.Commands.DebugLogType | Display type of debug log info
Log Message | Fungus.Variables.StringDataMulti | Text to write to the debug log. Supports variable substitution, e.g. {$Myvar}

## Destroy
Destroys a specified game object in the scene.

Property | Type | Description
 --- | --- | ---
_target Game Object | Fungus.Variables.GameObjectData | Reference to game object to destroy

## Execute Lua
Executes a Lua code chunk using a Lua Environment.

Property | Type | Description
 --- | --- | ---
Lua Environment | Fungus.LuaEnvironment | Lua Environment to use to execute this Lua script
Lua File | UnityEngine.TextAsset | A text file containing Lua script to execute.
Lua Script | System.String | Lua script to execute. This text is appended to the contents of Lua file (if one is specified).
Run As Coroutine | System.Boolean | Execute this Lua script as a Lua coroutine
Wait Until Finished | System.Boolean | Pause command execution until the Lua script has finished execution
Return Variable | Fungus.Variable | A Flowchart variable to store the returned value in.

## Invoke Event
Calls a list of component methods via the Unity Event System (as used in the Unity UI). This command is more efficient than the Invoke Method command but can only pass a single parameter and doesn't support return values.

Property | Type | Description
 --- | --- | ---
Delay | System.Single | Delay (in seconds) before the methods will be called
Static Event | UnityEngine.Events.UnityEvent | List of methods to call. Supports methods with no parameters or exactly one string, int, float or object parameter.
Boolean Parameter | Fungus.Variables.BooleanData | Boolean parameter to pass to the invoked methods.
Boolean Event | Fungus.Commands.InvokeEvent+BooleanEvent | List of methods to call. Supports methods with one boolean parameter.
Integer Parameter | Fungus.Variables.IntegerData | Integer parameter to pass to the invoked methods.
Integer Event | Fungus.Commands.InvokeEvent+IntegerEvent | List of methods to call. Supports methods with one integer parameter.
Float Parameter | Fungus.Variables.FloatData | Float parameter to pass to the invoked methods.
Float Event | Fungus.Commands.InvokeEvent+FloatEvent | List of methods to call. Supports methods with one float parameter.
String Parameter | Fungus.Variables.StringDataMulti | String parameter to pass to the invoked methods.
String Event | Fungus.Commands.InvokeEvent+StringEvent | List of methods to call. Supports methods with one string parameter.

## Invoke Method
Invokes a method of a component via reflection. Supports passing multiple parameters and storing returned values in a Fungus variable.

Property | Type | Description
 --- | --- | ---
Target Object | UnityEngine.GameObject | GameObject containing the component method to be invoked
Target Component Assembly Name | System.String | Name of assembly containing the target component
Target Component Fullname | System.String | Full name of the target component
Target Component Text | System.String | Display name of the target component
Target Method | System.String | Name of target method to invoke on the target component
Target Method Text | System.String | Display name of target method to invoke on the target component
Method Parameters | Fungus.Commands.InvokeMethodParameter[] | List of parameters to pass to the invoked method
Save Return Value | System.Boolean | If true, store the return value in a flowchart variable of the same type.
Return Value Variable Key | System.String | Name of Fungus variable to store the return value in
Return Value Type | System.String | The type of the return value
Show Inherited | System.Boolean | If true, list all inherited methods for the component
Call Mode | Fungus.Commands.CallMode | The coroutine call behavior for methods that return IEnumerator

## Open URL
Opens the specified URL in the browser.

Property | Type | Description
 --- | --- | ---
Url | Fungus.Variables.StringData | URL to open in the browser

## Set Active
Sets a game object in the scene to be active / inactive.

Property | Type | Description
 --- | --- | ---
_target Game Object | Fungus.Variables.GameObjectData | Reference to game object to enable / disable
Active State | Fungus.Variables.BooleanData | Set to true to enable the game object

## Spawn Object
Spawns a new object based on a reference to a scene or prefab game object.

Property | Type | Description
 --- | --- | ---
_source Object | Fungus.Variables.GameObjectData | Game object to copy when spawning. Can be a scene object or a prefab.
_parent Transform | Fungus.Variables.TransformData | Transform to use for position of newly spawned object.
_spawn Position | Fungus.Variables.Vector3Data | Local position of newly spawned object.
_spawn Rotation | Fungus.Variables.Vector3Data | Local rotation of newly spawned object.


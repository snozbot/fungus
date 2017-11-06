# Scripting commands # {#scripting_commands}

[TOC]
# Comment # {#Comment}
Use comments to record design notes and reminders about your game.

Defined in Fungus.Comment

Property | Type | Description
 --- | --- | ---
Commenter Name | System.String | Name of Commenter
Comment Text | System.String | Text to display for this comment

# Call Method # {#CallMethod}
Calls a named method on a GameObject using the GameObject.SendMessage() system.

Defined in Fungus.CallMethod

Property | Type | Description
 --- | --- | ---
Target Object | UnityEngine.GameObject | Target monobehavior which contains the method we want to call
Method Name | System.String | Name of the method to call
Delay | System.Single | Delay (in seconds) before the method will be called

# Debug Log # {#DebugLog}
Writes a log message to the debug console.

Defined in Fungus.DebugLog

Property | Type | Description
 --- | --- | ---
Log Type | Fungus.DebugLogType | Display type of debug log info
Log Message | Fungus.StringDataMulti | Text to write to the debug log. Supports variable substitution, e.g. {$Myvar}

# Destroy # {#Destroy}
Destroys a specified game object in the scene.

Defined in Fungus.Destroy

Property | Type | Description
 --- | --- | ---
_target Game Object | Fungus.GameObjectData | Reference to game object to destroy
Destroy In X Seconds | Fungus.FloatData | Optional delay given to destroy

# Execute Lua # {#ExecuteLua}
Executes a Lua code chunk using a Lua Environment.

Defined in Fungus.ExecuteLua

Property | Type | Description
 --- | --- | ---
Lua Environment | Fungus.LuaEnvironment | Lua Environment to use to execute this Lua script
Lua File | UnityEngine.TextAsset | A text file containing Lua script to execute.
Lua Script | System.String | Lua script to execute. This text is appended to the contents of Lua file (if one is specified).
Run As Coroutine | System.Boolean | Execute this Lua script as a Lua coroutine
Wait Until Finished | System.Boolean | Pause command execution until the Lua script has finished execution
Return Variable | Fungus.Variable | A Flowchart variable to store the returned value in.

# Invoke Event # {#InvokeEvent}
Calls a list of component methods via the Unity Event System (as used in the Unity UI). This command is more efficient than the Invoke Method command but can only pass a single parameter and doesn't support return values.

Defined in Fungus.InvokeEvent

Property | Type | Description
 --- | --- | ---
Description | System.String | A description of what this command does. Appears in the command summary.
Delay | System.Single | Delay (in seconds) before the methods will be called
Invoke Type | Fungus.InvokeType | Selects type of method parameter to pass
Static Event | UnityEngine.Events.UnityEvent | List of methods to call. Supports methods with no parameters or exactly one string, int, float or object parameter.
Boolean Parameter | Fungus.BooleanData | Boolean parameter to pass to the invoked methods.
Boolean Event | Fungus.InvokeEvent+BooleanEvent | List of methods to call. Supports methods with one boolean parameter.
Integer Parameter | Fungus.IntegerData | Integer parameter to pass to the invoked methods.
Integer Event | Fungus.InvokeEvent+IntegerEvent | List of methods to call. Supports methods with one integer parameter.
Float Parameter | Fungus.FloatData | Float parameter to pass to the invoked methods.
Float Event | Fungus.InvokeEvent+FloatEvent | List of methods to call. Supports methods with one float parameter.
String Parameter | Fungus.StringDataMulti | String parameter to pass to the invoked methods.
String Event | Fungus.InvokeEvent+StringEvent | List of methods to call. Supports methods with one string parameter.

# Invoke Method # {#InvokeMethod}
Invokes a method of a component via reflection. Supports passing multiple parameters and storing returned values in a Fungus variable.

Defined in Fungus.InvokeMethod

Property | Type | Description
 --- | --- | ---
Description | System.String | A description of what this command does. Appears in the command summary.
Target Object | UnityEngine.GameObject | GameObject containing the component method to be invoked
Target Component Assembly Name | System.String | Name of assembly containing the target component
Target Component Fullname | System.String | Full name of the target component
Target Component Text | System.String | Display name of the target component
Target Method | System.String | Name of target method to invoke on the target component
Target Method Text | System.String | Display name of target method to invoke on the target component
Method Parameters | Fungus.InvokeMethodParameter[] | List of parameters to pass to the invoked method
Save Return Value | System.Boolean | If true, store the return value in a flowchart variable of the same type.
Return Value Variable Key | System.String | Name of Fungus variable to store the return value in
Return Value Type | System.String | The type of the return value
Show Inherited | System.Boolean | If true, list all inherited methods for the component
Call Mode | Fungus.CallMode | The coroutine call behavior for methods that return IEnumerator

# Open URL # {#OpenURL}
Opens the specified URL in the browser.

Defined in Fungus.OpenURL

Property | Type | Description
 --- | --- | ---
Url | Fungus.StringData | URL to open in the browser

# Set Active # {#SetActive}
Sets a game object in the scene to be active / inactive.

Defined in Fungus.SetActive

Property | Type | Description
 --- | --- | ---
_target Game Object | Fungus.GameObjectData | Reference to game object to enable / disable
Active State | Fungus.BooleanData | Set to true to enable the game object

# Spawn Object # {#SpawnObject}
Spawns a new object based on a reference to a scene or prefab game object.

Defined in Fungus.SpawnObject

Property | Type | Description
 --- | --- | ---
_source Object | Fungus.GameObjectData | Game object to copy when spawning. Can be a scene object or a prefab.
_parent Transform | Fungus.TransformData | Transform to use as parent during instantiate.
_spawn At Self | Fungus.BooleanData | If true, will use the Transfrom of this Flowchart for the position and rotation.
_spawn Position | Fungus.Vector3Data | Local position of newly spawned object.
_spawn Rotation | Fungus.Vector3Data | Local rotation of newly spawned object.
_newly Spawned Object | Fungus.GameObjectData | Optional variable to store the GameObject that was just created.


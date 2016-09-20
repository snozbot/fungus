# Flow commands {#flow_commands}

## Break
Force a loop to terminate immediately.
## Call
Execute another block in the same Flowchart as the command, or in a different Flowchart.

Property | Type | Description
 --- | --- | ---
Target Flowchart | Fungus.Flowchart | Flowchart which contains the block to execute. If none is specified then the current Flowchart is used.
Target Block | Fungus.Block | Block to start executing
Start Index | System.Int32 | Command index to start executing
Call Mode | Fungus.Commands.CallMode | Select if the calling block should stop or continue executing commands, or wait until the called block finishes.

## Else
Marks the start of a command block to be executed when the preceding If statement is False.
## Else If
Marks the start of a command block to be executed when the preceding If statement is False and the test expression is true.

Property | Type | Description
 --- | --- | ---
Variable | Fungus.Variable | Variable to use in expression
Boolean Data | Fungus.Variables.BooleanData | Boolean value to compare against
Integer Data | Fungus.Variables.IntegerData | Integer value to compare against
Float Data | Fungus.Variables.FloatData | Float value to compare against
String Data | Fungus.Variables.StringDataMulti | String value to compare against
Compare Operator | Fungus.CompareOperator | The type of comparison to be performed

## End
Marks the end of a conditional block.
## If
If the test expression is true, execute the following command block.

Property | Type | Description
 --- | --- | ---
Variable | Fungus.Variable | Variable to use in expression
Boolean Data | Fungus.Variables.BooleanData | Boolean value to compare against
Integer Data | Fungus.Variables.IntegerData | Integer value to compare against
Float Data | Fungus.Variables.FloatData | Float value to compare against
String Data | Fungus.Variables.StringDataMulti | String value to compare against
Compare Operator | Fungus.CompareOperator | The type of comparison to be performed

## Jump
Move execution to a specific Label command in the same block

Property | Type | Description
 --- | --- | ---
_target Label | Fungus.Variables.StringData | Name of a label in this block to jump to

## Label
Marks a position in the command list for execution to jump to.

Property | Type | Description
 --- | --- | ---
Key | System.String | Display name for the label

## Load Scene
Loads a new Unity scene and displays an optional loading image. This is useful for splitting a large game across multiple scene files to reduce peak memory usage. Previously loaded assets will be released before loading the scene to free up memory.The scene to be loaded must be added to the scene list in Build Settings.

Property | Type | Description
 --- | --- | ---
_scene Name | Fungus.Variables.StringData | Name of the scene to load. The scene must also be added to the build settings.
Loading Image | UnityEngine.Texture2D | Image to display while loading the scene

## Quit
Quits the application. Does not work in Editor or Webplayer builds. Shouldn't generally be used on iOS.
## Send Message
Sends a message to either the owner Flowchart or all Flowcharts in the scene. Blocks can listen for this message using a Message Received event handler.

Property | Type | Description
 --- | --- | ---
Message Target | Fungus.Commands.MessageTarget | Target flowchart(s) to send the message to
_message | Fungus.Variables.StringData | Name of the message to send

## Stop
Stop executing the Block that contains this command.
## Stop Block
Stops executing the named Block

Property | Type | Description
 --- | --- | ---
Flowchart | Fungus.Flowchart | Flowchart containing the Block. If none is specified, the parent Flowchart is used.
Block Name | Fungus.Variables.StringData | Name of the Block to stop

## Stop Flowchart
Stops execution of all Blocks in a Flowchart

Property | Type | Description
 --- | --- | ---
Stop Parent Flowchart | System.Boolean | Stop all executing Blocks in the Flowchart that contains this command
Target Flowcharts | System.Collections.Generic.List`1[Fungus.Flowchart] | Stop all executing Blocks in a list of target Flowcharts

## Wait
Waits for period of time before executing the next command in the block.

Property | Type | Description
 --- | --- | ---
_duration | Fungus.Variables.FloatData | Duration to wait for

## WaitInput
Waits for a period of time or for player input before executing the next command in the block.

Property | Type | Description
 --- | --- | ---
Duration | System.Single | Duration to wait for. If negative will wait until player input occurs.

## While
Continuously loop through a block of commands while the condition is true. Use the Break command to force the loop to terminate immediately.

Property | Type | Description
 --- | --- | ---
Variable | Fungus.Variable | Variable to use in expression
Boolean Data | Fungus.Variables.BooleanData | Boolean value to compare against
Integer Data | Fungus.Variables.IntegerData | Integer value to compare against
Float Data | Fungus.Variables.FloatData | Float value to compare against
String Data | Fungus.Variables.StringDataMulti | String value to compare against
Compare Operator | Fungus.CompareOperator | The type of comparison to be performed


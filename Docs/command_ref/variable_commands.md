# Variable commands {#variable_commands}

## Delete Save Key
Deletes a saved value from permanent storage.

Property | Type | Description
 --- | --- | ---
Key | System.String | Name of the saved value. Supports variable substition e.g. "player_{$PlayerNumber}

## Load Variable
Loads a saved value and stores it in a Boolean, Integer, Float or String variable. If the key is not found then the variable is not modified.

Property | Type | Description
 --- | --- | ---
Key | System.String | Name of the saved value. Supports variable substition e.g. "player_{$PlayerNumber}"
Variable | Fungus.Variable | Variable to store the value in.

## Random Float
Sets an float variable to a random value in the defined range.

Property | Type | Description
 --- | --- | ---
Variable | Fungus.Variables.FloatVariable | The variable whos value will be set
Min Value | Fungus.Variables.FloatData | Minimum value for random range
Max Value | Fungus.Variables.FloatData | Maximum value for random range

## Random Integer
Sets an integer variable to a random value in the defined range.

Property | Type | Description
 --- | --- | ---
Variable | Fungus.Variables.IntegerVariable | The variable whos value will be set
Min Value | Fungus.Variables.IntegerData | Minimum value for random range
Max Value | Fungus.Variables.IntegerData | Maximum value for random range

## Read Text File
Reads in a text file and stores the contents in a string variable

Property | Type | Description
 --- | --- | ---
Text File | UnityEngine.TextAsset | Text file to read into the string variable
String Variable | Fungus.Variables.StringVariable | String variable to store the tex file contents in

## Reset
Resets the state of all commands and variables in the Flowchart.

Property | Type | Description
 --- | --- | ---
Reset Commands | System.Boolean | Reset state of all commands in the script
Reset Variables | System.Boolean | Reset variables back to their default values

## Save Variable
Save an Boolean, Integer, Float or String variable to persistent storage using a string key. The value can be loaded again later using the Load Variable command. You can also use the Set Save Profile command to manage separate save profiles for multiple players.

Property | Type | Description
 --- | --- | ---
Key | System.String | Name of the saved value. Supports variable substition e.g. "player_{$PlayerNumber}
Variable | Fungus.Variable | Variable to read the value from. Only Boolean, Integer, Float and String are supported.

## Set Save Profile
Sets the active profile that the Save Variable and Load Variable commands will use. This is useful to crete multiple player save games. Once set, the profile applies across all Flowcharts and will also persist across scene loads.

Property | Type | Description
 --- | --- | ---
Save Profile Name | System.String | Name of save profile to make active.

## Set Variable
Sets a Boolean, Integer, Float or String variable to a new value using a simple arithmetic operation. The value can be a constant or reference another variable of the same type.

Property | Type | Description
 --- | --- | ---
Variable | Fungus.Variable | The variable whos value will be set
Set Operator | Fungus.Commands.SetOperator | The type of math operation to be performed
Boolean Data | Fungus.Variables.BooleanData | Boolean value to set with
Integer Data | Fungus.Variables.IntegerData | Integer value to set with
Float Data | Fungus.Variables.FloatData | Float value to set with
String Data | Fungus.Variables.StringDataMulti | String value to set with


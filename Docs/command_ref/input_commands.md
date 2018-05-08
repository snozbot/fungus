# Input commands # {#input_commands}

[TOC]
# GetAxis # {#GetAxis}
Store Input.GetAxis in a variable

Defined in Fungus.GetAxis

Property | Type | Description
 --- | --- | ---
Axis Raw | System.Boolean | If true, calls GetAxisRaw instead of GetAxis
Out Value | Fungus.FloatData | Float to store the value of the GetAxis

# GetKey # {#GetKey}
Store Input.GetKey in a variable. Supports an optional Negative key input. A negative value will be overridden by a positive one, they do not add.

Defined in Fungus.GetKey

Property | Type | Description
 --- | --- | ---
Key Code Negative | UnityEngine.KeyCode | Optional, secondary or negative keycode. For booleans will also set to true, for int and float will set to -1.
Key Code Name | Fungus.StringData | Only used if KeyCode is KeyCode.None, expects a name of the key to use.
Key Code Name Negative | Fungus.StringData | Optional, secondary or negative keycode. For booleans will also set to true, for int and float will set to -1.Only used if KeyCode is KeyCode.None, expects a name of the key to use.
Key Query Type | Fungus.GetKey+InputKeyQueryType | Do we want an Input.GetKeyDown, GetKeyUp or GetKey
Out Value | Fungus.Variable | Will store true or false or 0 or 1 depending on type. Sets true or -1 for negative key values.


# Input Commands # {#input_commands}

Commands that interact with UnityEngine.Input

[TOC]
# Move # {#Move}
Moves a game object to a specified position over time. Can be either to or from a given target. Can be absolute or additive.

Defined in Fungus.MoveLean

Property | Type | Description
 --- | --- | ---
axisName | Fungus.StringData | Input Axis name, defined in [InputManager](https://docs.unity3d.com/Manual/class-InputManager.html)
axisRaw | System.Boolean | If true, calls GetAxisRaw instead of GetAxis
outValue | Fungus.FloatData | Float to store the value of the GetAxis.
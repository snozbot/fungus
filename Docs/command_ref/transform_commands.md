# Transform commands # {#transform_commands}

Commands that interact with and impact the UnityEngine.Transform

[TOC]
# Property # {#Property}
Fades a sprite to a target color over a period of time.

Defined in Fungus.TransformProperty

Property | Type | Description
 --- | --- | ---
getOrSet | System.Enum | Get or set the property from the transform
property | System.Enum | Which property are you targeting (ChildCount,EulerAngles,Forward,HasChanged,HierarchyCapacity,HierarchyCount,LocalEulerAngles,LocalPosition,LocalScale,LossyScale,Parent,Position,Right,Root,Up)
transformData | Fungus.TransformData | Target transform.
inOutVar | Fungus.Variable | Variable that is being used to pull data from or push the data to. Can be a bool, int, float or Transform. See (Unity Transform Docs for detials)[https://docs.unity3d.com/ScriptReference/Transform.html]

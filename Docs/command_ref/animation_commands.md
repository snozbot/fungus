# Animation commands # {#animation_commands}

[TOC]
# Play Anim State # {#PlayAnimState}
Plays a state of an animator according to the state name

Defined in Fungus.PlayAnimState

Property | Type | Description
 --- | --- | ---
Animator | Fungus.AnimatorData | Reference to an Animator component in a game object
State Name | Fungus.StringData | Name of the state you want to play
Layer | Fungus.IntegerData | Layer to play animation on
Time | Fungus.FloatData | Start time of animation

# Reset Anim Trigger # {#ResetAnimTrigger}
Resets a trigger parameter on an Animator component.

Defined in Fungus.ResetAnimTrigger

Property | Type | Description
 --- | --- | ---
_animator | Fungus.AnimatorData | Reference to an Animator component in a game object
_parameter Name | Fungus.StringData | Name of the trigger Animator parameter that will be reset

# Set Anim Bool # {#SetAnimBool}
Sets a boolean parameter on an Animator component to control a Unity animation

Defined in Fungus.SetAnimBool

Property | Type | Description
 --- | --- | ---
_animator | Fungus.AnimatorData | Reference to an Animator component in a game object
_parameter Name | Fungus.StringData | Name of the boolean Animator parameter that will have its value changed
Value | Fungus.BooleanData | The boolean value to set the parameter to

# Set Anim Float # {#SetAnimFloat}
Sets a float parameter on an Animator component to control a Unity animation

Defined in Fungus.SetAnimFloat

Property | Type | Description
 --- | --- | ---
_animator | Fungus.AnimatorData | Reference to an Animator component in a game object
_parameter Name | Fungus.StringData | Name of the float Animator parameter that will have its value changed
Value | Fungus.FloatData | The float value to set the parameter to

# Set Anim Integer # {#SetAnimInteger}
Sets an integer parameter on an Animator component to control a Unity animation

Defined in Fungus.SetAnimInteger

Property | Type | Description
 --- | --- | ---
_animator | Fungus.AnimatorData | Reference to an Animator component in a game object
_parameter Name | Fungus.StringData | Name of the integer Animator parameter that will have its value changed
Value | Fungus.IntegerData | The integer value to set the parameter to

# Set Anim Trigger # {#SetAnimTrigger}
Sets a trigger parameter on an Animator component to control a Unity animation

Defined in Fungus.SetAnimTrigger

Property | Type | Description
 --- | --- | ---
_animator | Fungus.AnimatorData | Reference to an Animator component in a game object
_parameter Name | Fungus.StringData | Name of the trigger Animator parameter that will have its value changed


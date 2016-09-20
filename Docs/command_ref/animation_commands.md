# Animation commands {#animation_commands}

## Play Anim State
Plays a state of an animator according to the state name

Property | Type | Description
 --- | --- | ---
Animator | Fungus.Variables.AnimatorData | Reference to an Animator component in a game object
State Name | Fungus.Variables.StringData | Name of the state you want to play
Layer | Fungus.Variables.IntegerData | Layer to play animation on
Time | Fungus.Variables.FloatData | Start time of animation

## Reset Anim Trigger
Resets a trigger parameter on an Animator component.

Property | Type | Description
 --- | --- | ---
_animator | Fungus.Variables.AnimatorData | Reference to an Animator component in a game object
_parameter Name | Fungus.Variables.StringData | Name of the trigger Animator parameter that will be reset

## Set Anim Bool
Sets a boolean parameter on an Animator component to control a Unity animation

Property | Type | Description
 --- | --- | ---
_animator | Fungus.Variables.AnimatorData | Reference to an Animator component in a game object
_parameter Name | Fungus.Variables.StringData | Name of the boolean Animator parameter that will have its value changed
Value | Fungus.Variables.BooleanData | The boolean value to set the parameter to

## Set Anim Float
Sets a float parameter on an Animator component to control a Unity animation

Property | Type | Description
 --- | --- | ---
_animator | Fungus.Variables.AnimatorData | Reference to an Animator component in a game object
_parameter Name | Fungus.Variables.StringData | Name of the float Animator parameter that will have its value changed
Value | Fungus.Variables.FloatData | The float value to set the parameter to

## Set Anim Integer
Sets an integer parameter on an Animator component to control a Unity animation

Property | Type | Description
 --- | --- | ---
_animator | Fungus.Variables.AnimatorData | Reference to an Animator component in a game object
_parameter Name | Fungus.Variables.StringData | Name of the integer Animator parameter that will have its value changed
Value | Fungus.Variables.IntegerData | The integer value to set the parameter to

## Set Anim Trigger
Sets a trigger parameter on an Animator component to control a Unity animation

Property | Type | Description
 --- | --- | ---
_animator | Fungus.Variables.AnimatorData | Reference to an Animator component in a game object
_parameter Name | Fungus.Variables.StringData | Name of the trigger Animator parameter that will have its value changed


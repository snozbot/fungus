# LeanTween commands # {#leantween_commands}

[TOC]
# Move # {#Move}
Moves a game object to a specified position over time. The position can be defined by a transform in another object (using To Transform) or by setting an absolute position (using To Position, if To Transform is set to None).

Defined in Fungus.MoveLean

Property | Type | Description
 --- | --- | ---
_to Transform | Fungus.TransformData | Target transform that the GameObject will move to
_to Position | Fungus.Vector3Data | Target world position that the GameObject will move to, if no From Transform is set
Is Local | System.Boolean | Whether to animate in world space or relative to the parent. False by default.
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
_to From | Fungus.BaseLeanTweenCommand+ToFrom | Does the tween act from current TO destination or is it reversed and act FROM destination to its current
_abs Add | Fungus.BaseLeanTweenCommand+AbsAdd | Does the tween use the value as a target or as a delta to be added to current.
Ease Type | LeanTweenType | The shape of the easing curve applied to the animation
Loop Type | LeanTweenType | The type of loop to apply once the animation has completed
Repeats | System.Int32 | Number of times to repeat the tween, -1 is infinite.
Stop Previous Tweens | System.Boolean | Stop any previously LeanTweens on this object before adding this one. Warning; expensive.
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Rotate # {#Rotate}
Rotates a game object to the specified angles over time.

Defined in Fungus.RotateLean

Property | Type | Description
 --- | --- | ---
_to Transform | Fungus.TransformData | Target transform that the GameObject will rotate to
_to Rotation | Fungus.Vector3Data | Target rotation that the GameObject will rotate to, if no To Transform is set
Is Local | System.Boolean | Whether to animate in world space or relative to the parent. False by default.
Rotate Mode | Fungus.RotateLean+RotateMode | Whether to use the provided Transform or Vector as a target to look at rather than a euler to match.
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
_to From | Fungus.BaseLeanTweenCommand+ToFrom | Does the tween act from current TO destination or is it reversed and act FROM destination to its current
_abs Add | Fungus.BaseLeanTweenCommand+AbsAdd | Does the tween use the value as a target or as a delta to be added to current.
Ease Type | LeanTweenType | The shape of the easing curve applied to the animation
Loop Type | LeanTweenType | The type of loop to apply once the animation has completed
Repeats | System.Int32 | Number of times to repeat the tween, -1 is infinite.
Stop Previous Tweens | System.Boolean | Stop any previously LeanTweens on this object before adding this one. Warning; expensive.
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Scale # {#Scale}
Changes a game object's scale to a specified value over time.

Defined in Fungus.ScaleLean

Property | Type | Description
 --- | --- | ---
_to Transform | Fungus.TransformData | Target transform that the GameObject will scale to
_to Scale | Fungus.Vector3Data | Target scale that the GameObject will scale to, if no To Transform is set
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
_to From | Fungus.BaseLeanTweenCommand+ToFrom | Does the tween act from current TO destination or is it reversed and act FROM destination to its current
_abs Add | Fungus.BaseLeanTweenCommand+AbsAdd | Does the tween use the value as a target or as a delta to be added to current.
Ease Type | LeanTweenType | The shape of the easing curve applied to the animation
Loop Type | LeanTweenType | The type of loop to apply once the animation has completed
Repeats | System.Int32 | Number of times to repeat the tween, -1 is infinite.
Stop Previous Tweens | System.Boolean | Stop any previously LeanTweens on this object before adding this one. Warning; expensive.
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# StopTweens # {#StopTweens}
Stops the LeanTweens on a target GameObject

Defined in Fungus.StopTweensLean

Property | Type | Description
 --- | --- | ---
_target Object | Fungus.GameObjectData | Target game object stop LeanTweens on


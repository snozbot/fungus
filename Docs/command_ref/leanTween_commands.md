# LeanTween commands # {#leanTween_commands}

Fungus Commands that apply [LeanTweens](http://dentedpixel.com/LeanTweenDocumentation/classes/LeanTween.html) to a GameObject.

[TOC]
# Move # {#Move}
Moves a game object to a specified position over time. Can be either to or from a given target. Can be absolute or additive.

Defined in Fungus.MoveLean

Property | Type | Description
 --- | --- | ---
_targetObject | Fungus.GameObjectData | GameObject to tween
_duration | Fungus.FloatData | Time in seconds for the tween to complete
_toFrom | System.Enum | 'To' or 'From'. To means it tweens from its current to the target. From, will jump to the final and tween back towards the current.
_absAdd | System.Enum | 'Absolute' or 'Additive'. Absolute treats the destination as a final. Additive calculates the final as the current plus the value within the destination.
easeType | LeanTweenType | Forumla used to animate from start to end value. E.g. easeInOutQuad
loopType | LeanTweenType | If the tween is to loop (play it's duration more than once) how should it do that, clamp or pingping etc.
repeats | System.Int32 | Number of times to repeat the tween, -1 is infinite.
stopPreviousTweens | System.Boolean | If true stop any previously LeanTweens on this object before adding this one. Warning; expensive.
waitUntilFinished | System.Boolean | If true, this command will halt the block until the tween is finished.
_toTransform | Fungus.TransformData | Target transform that the GameObject will move to. Often easier to configure tweens with the use of a target gameobject than a hard coded world position. (if this is not set then the _toPosition vector3 will be used)
_toPosition | Fungus.Vector3Data | Target world position that the GameObject will move to, if no From Transform is set. This is only used if the _toTransform is null
isLocal | System.Boolean | Whether to animate in world space or relative to the parent. False by default.


# Scale # {#Scale}
Scales a game object to a specified scale over time. Can be either to or from a given target. Can be absolute or additive.

Defined in Fungus.ScaleLean

Property | Type | Description
 --- | --- | ---
_targetObject | Fungus.GameObjectData | GameObject to tween
_duration | Fungus.FloatData | Time in seconds for the tween to complete
_toFrom | System.Enum | 'To' or 'From'. To means it tweens from its current to the target. From, will jump to the final and tween back towards the current.
_absAdd | System.Enum | 'Absolute' or 'Additive'. Absolute treats the destination as a final. Additive calculates the final as the current plus the value within the destination.
easeType | LeanTweenType | Forumla used to animate from start to end value. E.g. easeInOutQuad
loopType | LeanTweenType | If the tween is to loop (play it's duration more than once) how should it do that, clamp or pingping etc.
repeats | System.Int32 | Number of times to repeat the tween, -1 is infinite.
stopPreviousTweens | System.Boolean | If true stop any previously LeanTweens on this object before adding this one. Warning; expensive.
waitUntilFinished | System.Boolean | If true, this command will halt the block until the tween is finished.
_toTransform | Fungus.TransformData | Target scale for the tween to use. If null then the _toScale vector3 is used instead.
_toScale | Fungus.Vector3Data | Target scale that the GameObject will scale to. Only used if _toTransform is null. Default is 1,1,1

# Rotate # {#Rotate}
Rotate a game object to a specified rotation (matching another transform or eulerAngle) over time. Can be either to or from a given target. Can be absolute or additive.

Defined in Fungus.RotateLean

Property | Type | Description
 --- | --- | ---
_targetObject | Fungus.GameObjectData | GameObject to tween
_duration | Fungus.FloatData | Time in seconds for the tween to complete
_toFrom | System.Enum | 'To' or 'From'. To means it tweens from its current to the target. From, will jump to the final and tween back towards the current.
_absAdd | System.Enum | 'Absolute' or 'Additive'. Absolute treats the destination as a final. Additive calculates the final as the current plus the value within the destination.
easeType | LeanTweenType | Forumla used to animate from start to end value. E.g. easeInOutQuad
loopType | LeanTweenType | If the tween is to loop (play it's duration more than once) how should it do that, clamp or pingping etc.
repeats | System.Int32 | Number of times to repeat the tween, -1 is infinite.
stopPreviousTweens | System.Boolean | If true stop any previously LeanTweens on this object before adding this one. Warning; expensive.
waitUntilFinished | System.Boolean | If true, this command will halt the block until the tween is finished.
_toTransform | Fungus.TransformData | Rotation for the tween to match. If not set _toRotation will be used.
_ToRotation | Fungus.FloatData | Target rotation that the GameObject will rotate to, if no To Transform is set
isLocal | System.Boolean | Whether to animate in world space or relative to the parent. False by default.
rotateMode | System.Enum | 'PureRotate', 'LookAt2D' or 'LookAt3D'. Determines how to use the supplied rotation information. PureRotate is a simply euler/quaternion match. LookAt2D matches the euler z only. LookAt3D rotations the objects forward to point at the given direction/target.

# Stop Tweens # {#StopTweens}
Stops the all active LeanTweens, made by fungus or otherwise, on the target GameObject

Defined in Fungus.StopTweensLean

Property | Type | Description
 --- | --- | ---
_target Object | Fungus.GameObjectData | Target game object stop LeanTweens on

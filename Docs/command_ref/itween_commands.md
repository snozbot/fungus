# iTween commands # {#itween_commands}

[TOC]
# Look From # {#LookFrom}
Instantly rotates a GameObject to look at the supplied Vector3 then returns it to it's starting rotation over time.

Defined in Fungus.LookFrom

Property | Type | Description
 --- | --- | ---
_from Transform | Fungus.TransformData | Target transform that the GameObject will look at
_from Position | Fungus.Vector3Data | Target world position that the GameObject will look at, if no From Transform is set
Axis | Fungus.iTweenAxis | Restricts rotation to the supplied axis only
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Look To # {#LookTo}
Rotates a GameObject to look at a supplied Transform or Vector3 over time.

Defined in Fungus.LookTo

Property | Type | Description
 --- | --- | ---
_to Transform | Fungus.TransformData | Target transform that the GameObject will look at
_to Position | Fungus.Vector3Data | Target world position that the GameObject will look at, if no From Transform is set
Axis | Fungus.iTweenAxis | Restricts rotation to the supplied axis only
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Move Add # {#MoveAdd}
Moves a game object by a specified offset over time.

Defined in Fungus.MoveAdd

Property | Type | Description
 --- | --- | ---
_offset | Fungus.Vector3Data | A translation offset in space the GameObject will animate to
Space | UnityEngine.Space | Apply the transformation in either the world coordinate or local cordinate system
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Move From # {#MoveFrom}
Moves a game object from a specified position back to its starting position over time. The position can be defined by a transform in another object (using To Transform) or by setting an absolute position (using To Position, if To Transform is set to None).

Defined in Fungus.MoveFrom

Property | Type | Description
 --- | --- | ---
_from Transform | Fungus.TransformData | Target transform that the GameObject will move from
_from Position | Fungus.Vector3Data | Target world position that the GameObject will move from, if no From Transform is set
Is Local | System.Boolean | Whether to animate in world space or relative to the parent. False by default.
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Move To # {#MoveTo}
Moves a game object to a specified position over time. The position can be defined by a transform in another object (using To Transform) or by setting an absolute position (using To Position, if To Transform is set to None).

Defined in Fungus.MoveTo

Property | Type | Description
 --- | --- | ---
_to Transform | Fungus.TransformData | Target transform that the GameObject will move to
_to Position | Fungus.Vector3Data | Target world position that the GameObject will move to, if no From Transform is set
Is Local | System.Boolean | Whether to animate in world space or relative to the parent. False by default.
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Punch Position # {#PunchPosition}
Applies a jolt of force to a GameObject's position and wobbles it back to its initial position.

Defined in Fungus.PunchPosition

Property | Type | Description
 --- | --- | ---
_amount | Fungus.Vector3Data | A translation offset in space the GameObject will animate to
Space | UnityEngine.Space | Apply the transformation in either the world coordinate or local cordinate system
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Punch Rotation # {#PunchRotation}
Applies a jolt of force to a GameObject's rotation and wobbles it back to its initial rotation.

Defined in Fungus.PunchRotation

Property | Type | Description
 --- | --- | ---
_amount | Fungus.Vector3Data | A rotation offset in space the GameObject will animate to
Space | UnityEngine.Space | Apply the transformation in either the world coordinate or local cordinate system
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Punch Scale # {#PunchScale}
Applies a jolt of force to a GameObject's scale and wobbles it back to its initial scale.

Defined in Fungus.PunchScale

Property | Type | Description
 --- | --- | ---
_amount | Fungus.Vector3Data | A scale offset in space the GameObject will animate to
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Rotate Add # {#RotateAdd}
Rotates a game object by the specified angles over time.

Defined in Fungus.RotateAdd

Property | Type | Description
 --- | --- | ---
_offset | Fungus.Vector3Data | A rotation offset in space the GameObject will animate to
Space | UnityEngine.Space | Apply the transformation in either the world coordinate or local cordinate system
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Rotate From # {#RotateFrom}
Rotates a game object from the specified angles back to its starting orientation over time.

Defined in Fungus.RotateFrom

Property | Type | Description
 --- | --- | ---
_from Transform | Fungus.TransformData | Target transform that the GameObject will rotate from
_from Rotation | Fungus.Vector3Data | Target rotation that the GameObject will rotate from, if no From Transform is set
Is Local | System.Boolean | Whether to animate in world space or relative to the parent. False by default.
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Rotate To # {#RotateTo}
Rotates a game object to the specified angles over time.

Defined in Fungus.RotateTo

Property | Type | Description
 --- | --- | ---
_to Transform | Fungus.TransformData | Target transform that the GameObject will rotate to
_to Rotation | Fungus.Vector3Data | Target rotation that the GameObject will rotate to, if no To Transform is set
Is Local | System.Boolean | Whether to animate in world space or relative to the parent. False by default.
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Scale Add # {#ScaleAdd}
Changes a game object's scale by a specified offset over time.

Defined in Fungus.ScaleAdd

Property | Type | Description
 --- | --- | ---
_offset | Fungus.Vector3Data | A scale offset in space the GameObject will animate to
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Scale From # {#ScaleFrom}
Changes a game object's scale to the specified value and back to its original scale over time.

Defined in Fungus.ScaleFrom

Property | Type | Description
 --- | --- | ---
_from Transform | Fungus.TransformData | Target transform that the GameObject will scale from
_from Scale | Fungus.Vector3Data | Target scale that the GameObject will scale from, if no From Transform is set
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Scale To # {#ScaleTo}
Changes a game object's scale to a specified value over time.

Defined in Fungus.ScaleTo

Property | Type | Description
 --- | --- | ---
_to Transform | Fungus.TransformData | Target transform that the GameObject will scale to
_to Scale | Fungus.Vector3Data | Target scale that the GameObject will scale to, if no To Transform is set
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Shake Position # {#ShakePosition}
Randomly shakes a GameObject's position by a diminishing amount over time.

Defined in Fungus.ShakePosition

Property | Type | Description
 --- | --- | ---
_amount | Fungus.Vector3Data | A translation offset in space the GameObject will animate to
Is Local | System.Boolean | Whether to animate in world space or relative to the parent. False by default.
Axis | Fungus.iTweenAxis | Restricts rotation to the supplied axis only
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Shake Rotation # {#ShakeRotation}
Randomly shakes a GameObject's rotation by a diminishing amount over time.

Defined in Fungus.ShakeRotation

Property | Type | Description
 --- | --- | ---
_amount | Fungus.Vector3Data | A rotation offset in space the GameObject will animate to
Space | UnityEngine.Space | Apply the transformation in either the world coordinate or local cordinate system
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Shake Scale # {#ShakeScale}
Randomly shakes a GameObject's rotation by a diminishing amount over time.

Defined in Fungus.ShakeScale

Property | Type | Description
 --- | --- | ---
_amount | Fungus.Vector3Data | A scale offset in space the GameObject will animate to
_target Object | Fungus.GameObjectData | Target game object to apply the Tween to
_tween Name | Fungus.StringData | An individual name useful for stopping iTweens by name
_duration | Fungus.FloatData | The time in seconds the animation will take to complete
Ease Type | Fungus.iTween+EaseType | The shape of the easing curve applied to the animation
Loop Type | Fungus.iTween+LoopType | The type of loop to apply once the animation has completed
Stop Previous Tweens | System.Boolean | Stop any previously added iTweens on this object before adding this iTween
Wait Until Finished | System.Boolean | Wait until the tween has finished before executing the next command

# Stop Tween # {#StopTween}
Stops an active iTween by name.

Defined in Fungus.StopTween

Property | Type | Description
 --- | --- | ---
_tween Name | Fungus.StringData | Stop and destroy any Tweens in current scene with the supplied name

# Stop Tweens # {#StopTweens}
Stop all active iTweens in the current scene.

Defined in Fungus.StopTweens

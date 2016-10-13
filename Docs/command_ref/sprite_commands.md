# Sprite commands # {#sprite_commands}

[TOC]
# Fade Sprite # {#FadeSprite}
Fades a sprite to a target color over a period of time.

Defined in Fungus.FadeSprite

Property | Type | Description
 --- | --- | ---
Sprite Renderer | UnityEngine.SpriteRenderer | Sprite object to be faded
_duration | Fungus.FloatData | Length of time to perform the fade
_target Color | Fungus.ColorData | Target color to fade to. To only fade transparency level, set the color to white and set the alpha to required transparency.
Wait Until Finished | System.Boolean | Wait until the fade has finished before executing the next command

# Set Clickable 2D # {#SetClickable2D}
Sets a Clickable2D component to be clickable / non-clickable.

Defined in Fungus.SetClickable2D

Property | Type | Description
 --- | --- | ---
Target Clickable2 D | Fungus.Clickable2D | Reference to Clickable2D component on a gameobject
Active State | Fungus.BooleanData | Set to true to enable the component

# Set Collider # {#SetCollider}
Sets all collider (2d or 3d) components on the target objects to be active / inactive

Defined in Fungus.SetCollider

Property | Type | Description
 --- | --- | ---
Target Objects | System.Collections.Generic.List`1[UnityEngine.GameObject] | A list of gameobjects containing collider components to be set active / inactive
Target Tag | System.String | All objects with this tag will have their collider set active / inactive
Active State | Fungus.BooleanData | Set to true to enable the collider components

# Set Draggable 2D # {#SetDraggable2D}
Sets a Draggable2D component to be draggable / non-draggable.

Defined in Fungus.SetDraggable2D

Property | Type | Description
 --- | --- | ---
Target Draggable2 D | Fungus.Draggable2D | Reference to Draggable2D component on a gameobject
Active State | Fungus.BooleanData | Set to true to enable the component

# Set Mouse Cursor # {#SetMouseCursor}
Sets the mouse cursor sprite.

Defined in Fungus.SetMouseCursor

Property | Type | Description
 --- | --- | ---
Cursor Texture | UnityEngine.Texture2D | Texture to use for cursor. Will use default mouse cursor if no sprite is specified
Hot Spot | UnityEngine.Vector2 | The offset from the top left of the texture to use as the target point

# Set Sorting Layer # {#SetSortingLayer}
Sets the Renderer sorting layer of every child of a game object. Applies to all Renderers (including mesh, skinned mesh, and sprite).

Defined in Fungus.SetSortingLayer

Property | Type | Description
 --- | --- | ---
Target Object | UnityEngine.GameObject | Root Object that will have the Sorting Layer set. Any children will also be affected
Sorting Layer | System.String | The New Layer Name to apply

# Set Sprite Order # {#SetSpriteOrder}
Controls the render order of sprites by setting the Order In Layer property of a list of sprites.

Defined in Fungus.SetSpriteOrder

Property | Type | Description
 --- | --- | ---
Target Sprites | System.Collections.Generic.List`1[UnityEngine.SpriteRenderer] | List of sprites to set the order in layer property on
Order In Layer | Fungus.IntegerData | The order in layer value to set on the target sprites

# Show Sprite # {#ShowSprite}
Makes a sprite visible / invisible by setting the color alpha.

Defined in Fungus.ShowSprite

Property | Type | Description
 --- | --- | ---
Sprite Renderer | UnityEngine.SpriteRenderer | Sprite object to be made visible / invisible
_visible | Fungus.BooleanData | Make the sprite visible or invisible
Affect Children | System.Boolean | Affect the visibility of child sprites


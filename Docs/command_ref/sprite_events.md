# Sprite event handlers # {#sprite_events}

[TOC]
# Drag Cancelled # {#DragCancelled}
The block will execute when the player drags an object and releases it without dropping it on a target object.

Defined in Fungus.DragCancelled

Property | Type | Description
 --- | --- | ---
Draggable Object | Fungus.Draggable2D | Draggable object to listen for drag events on

# Drag Completed # {#DragCompleted}
The block will execute when the player drags an object and successfully drops it on a target object.

Defined in Fungus.DragCompleted

Property | Type | Description
 --- | --- | ---
Draggable Object | Fungus.Draggable2D | Draggable object to listen for drag events on
Target Object | UnityEngine.Collider2D | Drag target object to listen for drag events on

# Drag Entered # {#DragEntered}
The block will execute when the player is dragging an object which starts touching the target object.

Defined in Fungus.DragEntered

Property | Type | Description
 --- | --- | ---
Draggable Object | Fungus.Draggable2D | Draggable object to listen for drag events on
Target Object | UnityEngine.Collider2D | Drag target object to listen for drag events on

# Drag Exited # {#DragExited}
The block will execute when the player is dragging an object which stops touching the target object.

Defined in Fungus.DragExited

Property | Type | Description
 --- | --- | ---
Draggable Object | Fungus.Draggable2D | Draggable object to listen for drag events on
Target Object | UnityEngine.Collider2D | Drag target object to listen for drag events on

# Drag Started # {#DragStarted}
The block will execute when the player starts dragging an object.

Defined in Fungus.DragStarted
# Object Clicked # {#ObjectClicked}
The block will execute when the user clicks or taps on the clickable object.

Defined in Fungus.ObjectClicked

Property | Type | Description
 --- | --- | ---
Clickable Object | Fungus.Clickable2D | Object that the user can click or tap on
Wait Frames | System.Int32 | Wait for a number of frames before executing the block.


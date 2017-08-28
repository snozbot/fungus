# MonoBehaviour Events # {#monobeh_events}

See [Unity MonoBehaviour Messages](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html) for more context.

[TOC]
# Animator # {#Animator}
The block will execute on the selected OnAnimator messages from Unity.

Defined in Fungus.AnimatorState

Property | Type | Description
 --- | --- | ---
FireOn | System.Enum | OnAnimatorIK, OnAnimatorMove. Flags to determine which of the Unity messages causes this event to fire.
IKLayer | System.Int32 | IK layer to trigger on. Negative is all.

# Application # {#Application}
The block will execute on the selected OnApplication messages from Unity.

Defined in Fungus.ApplicationState

Property | Type | Description
 --- | --- | ---
FireOn | System.Enum | OnApplicationGetFocus, OnApplicationLoseFocus, OnApplicationPause, OnApplicationResume, OnApplicationQuit. Flags to determine which of the Unity messages causes this event to fire.

# CharacterCollider # {#CharacterCollider}
The block will execute on the OnControllerColliderHit messages from Unity & tags pass tests.

Defined in Fungus.CharacterControllerCollide

Property | Type | Description
 --- | --- | ---
tagFilter | System.String\[\] | Array of strings, if this is empty then tag comparing is ignored. Otherwise as long as 1 of the tags within matches the incoming tag to test it will pass. Think of it like a big chain of 'or's.

# Collision # {#Collision}
The block will execute on the OnCollision related messages from Unity & tags pass tests. Used for the 3D physics system, see the [collision detection occurs ](https://docs.unity3d.com/Manual/CollidersOverview.html) section for more info.

Defined in Fungus.Collision

Property | Type | Description
 --- | --- | ---
tagFilter | System.String\[\] | Array of strings, if this is empty then tag comparing is ignored. Otherwise as long as 1 of the tags within matches the incoming tag to test it will pass. Think of it like a big chain of 'or's.
FireOn | System.Enum | Enter, Stay, Exit. Flags to determine which of the Unity messages causes this event to fire. 

# Collision2D # {#Collision2D}
The block will execute on the OnCollision related messages from Unity & tags pass tests. Used for the 2D physics system, see the [collision detection occurs ](https://docs.unity3d.com/Manual/CollidersOverview.html) section for more info.

Defined in Fungus.Collision2D

Property | Type | Description
 --- | --- | ---
tagFilter | System.String\[\] | Array of strings, if this is empty then tag comparing is ignored. Otherwise as long as 1 of the tags within matches the incoming tag to test it will pass. Think of it like a big chain of 'or's.
FireOn | System.Enum | Enter, Stay, Exit. Flags to determine which of the Unity messages causes this event to fire. 

# Mouse # {#Mouse}
The block will execute on the selected OnMouse messages from Unity.

Defined in Fungus.Mouse

Property | Type | Description
 --- | --- | ---
FireOn | System.Enum | OnMouseDown, OnMouseDrag, OnMouseEnter, OnMouseExit, OnMouseOver, OnMouseUp, OnMouseUpAsButton. Flags to determine which of the Unity messages causes this event to fire.


# Particle # {#Particle}
The block will execute on the OnParticle related messages from Unity & tags pass tests.

Defined in Fungus.Particle

Property | Type | Description
 --- | --- | ---
tagFilter | System.String\[\] | Array of strings, if this is empty then tag comparing is ignored. Otherwise as long as 1 of the tags within matches the incoming tag to test it will pass. Think of it like a big chain of 'or's.
FireOn | System.Enum | OnParticleCollision, OnParticleTrigger. Flags to determine which of the Unity messages causes this event to fire. OnParticleCollision uses the tag filter, OnParticleTrigger has no parameters provided by Unity.

# Render # {#Render}
The block will execute on the selected On*Render messages from Unity.

Defined in Fungus.Render

Property | Type | Description
 --- | --- | ---
FireOn | System.Enum | OnPostRender, OnPreCull, OnPreRender, OnRenderObject, OnWillRenderObject, OnBecameInvisible, OnBecameVisible. Flags to determine which of the Unity messages causes this event to fire.

# Transform # {#Transform}
The block will execute on the selected transform changed messages from Unity.

Defined in Fungus.TransformChanged

Property | Type | Description
 --- | --- | ---
FireOn | System.Enum | OnTransformChildrenChanged, OnTransformParentChanged. Flags to determine which of the Unity messages causes this event to fire.

# Trigger # {#Trigger}
The block will execute on the OnTrigger related messages from Unity & tags pass tests. Used for the 3D physics system, see the [trigger messages sent upon](https://docs.unity3d.com/Manual/CollidersOverview.html) section for more info.

Defined in Fungus.Trigger

Property | Type | Description
 --- | --- | ---
tagFilter | System.String\[\] | Array of strings, if this is empty then tag comparing is ignored. Otherwise as long as 1 of the tags within matches the incoming tag to test it will pass. Think of it like a big chain of 'or's.
FireOn | System.Enum | Enter, Stay, Exit. Flags to determine which of the Unity messages causes this event to fire. 

# Trigger2D # {#Trigger2D}
The block will execute on the OnTrigger*2D related messages from Unity & tags pass tests. Used for the 2D physics system, see the [trigger messages sent upon](https://docs.unity3d.com/Manual/CollidersOverview.html) section for more info.

Defined in Fungus.Trigger2D

Property | Type | Description
 --- | --- | ---
tagFilter | System.String\[\] | Array of strings, if this is empty then tag comparing is ignored. Otherwise as long as 1 of the tags within matches the incoming tag to test it will pass. Think of it like a big chain of 'or's.
FireOn | System.Enum | Enter, Stay, Exit. Flags to determine which of the Unity messages causes this event to fire. 

# Update # {#Update}
The block will execute on the selected update messages from Unity.

Defined in Fungus.UpdateTick

Property | Type | Description
 --- | --- | ---
FireOn | System.Enum | Update, FixedUpdate, LateUpdate. Flags to determine which of the Unity messages causes this event to fire.

# MonoBehaviour event handlers # {#monobehaviour_events}

[TOC]
# Animator # {#Animator}
The block will execute when the desired OnAnimator* message for the monobehaviour is received.

Defined in Fungus.AnimatorState

Property | Type | Description
 --- | --- | ---
Fire On | Fungus.AnimatorState+AnimatorMessageFlags | Which of the OnAnimator messages to trigger on.
I K Layer | System.Int32 | IK layer to trigger on. Negative is all.

# Application # {#Application}
The block will execute when the desired OnApplication message for the monobehaviour is received.

Defined in Fungus.ApplicationState

Property | Type | Description
 --- | --- | ---
Fire On | Fungus.ApplicationState+ApplicationMessageFlags | Which of the Application messages to trigger on.

# CharacterCollider # {#CharacterCollider}
The block will execute when tag filtered OnCharacterColliderHit is received

Defined in Fungus.CharacterControllerCollide

Property | Type | Description
 --- | --- | ---
Tag Filter | System.String[] | Only fire the event if one of the tags match. Empty means any will fire.

# Collision # {#Collision}
The block will execute when a 3d physics collision matching some basic conditions is met.

Defined in Fungus.Collision

Property | Type | Description
 --- | --- | ---
Fire On | Fungus.BasePhysicsEventHandler+PhysicsMessageType | Which of the 3d physics messages to we trigger on.
Tag Filter | System.String[] | Only fire the event if one of the tags match. Empty means any will fire.

# Collision2D # {#Collision2D}
The block will execute when a 2d physics collision matching some basic conditions is met.

Defined in Fungus.Collision2D

Property | Type | Description
 --- | --- | ---
Fire On | Fungus.BasePhysicsEventHandler+PhysicsMessageType | Which of the 3d physics messages to we trigger on.
Tag Filter | System.String[] | Only fire the event if one of the tags match. Empty means any will fire.

# Mouse # {#Mouse}
The block will execute when the desired OnMouse* message for the monobehaviour is received

Defined in Fungus.Mouse

Property | Type | Description
 --- | --- | ---
Fire On | Fungus.Mouse+MouseMessageFlags | Which of the Mouse messages to trigger on.

# Particle # {#Particle}
The block will execute when the desired OnParticle message for the monobehaviour is received.

Defined in Fungus.Particle

Property | Type | Description
 --- | --- | ---
Fire On | Fungus.Particle+ParticleMessageFlags | Which of the Rendering messages to trigger on.
Tag Filter | System.String[] | Only fire the event if one of the tags match. Empty means any will fire.

# Render # {#Render}
The block will execute when the desired Rendering related message for the monobehaviour is received.

Defined in Fungus.Render

Property | Type | Description
 --- | --- | ---
Fire On | Fungus.Render+RenderMessageFlags | Which of the Rendering messages to trigger on.

# Transform # {#Transform}
The block will execute when the desired OnTransform related message for the monobehaviour is received.

Defined in Fungus.TransformChanged

Property | Type | Description
 --- | --- | ---
Fire On | Fungus.TransformChanged+TransformMessageFlags | Which of the OnTransformChanged messages to trigger on.

# Trigger # {#Trigger}
The block will execute when a 3d physics trigger matching some basic conditions is met.

Defined in Fungus.Trigger

Property | Type | Description
 --- | --- | ---
Fire On | Fungus.BasePhysicsEventHandler+PhysicsMessageType | Which of the 3d physics messages to we trigger on.
Tag Filter | System.String[] | Only fire the event if one of the tags match. Empty means any will fire.

# Trigger2D # {#Trigger2D}
The block will execute when a 2d physics trigger matching some basic conditions is met.

Defined in Fungus.Trigger2D

Property | Type | Description
 --- | --- | ---
Fire On | Fungus.BasePhysicsEventHandler+PhysicsMessageType | Which of the 3d physics messages to we trigger on.
Tag Filter | System.String[] | Only fire the event if one of the tags match. Empty means any will fire.

# Update # {#Update}
The block will execute every chosen Update, or FixedUpdate or LateUpdate.

Defined in Fungus.UpdateTick

Property | Type | Description
 --- | --- | ---
Fire On | Fungus.UpdateTick+UpdateMessageFlags | Which of the Update messages to trigger on.


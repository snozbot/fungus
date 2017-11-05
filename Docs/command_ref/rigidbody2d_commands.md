# Rigidbody2D Commands # {#rigidbody2d_commands}

Commands that interact with [UnityEngine.Rigidbody2D](https://docs.unity3d.com/ScriptReference/Rigidbody2D.html) & Fungus.Rigidbody2DVariable

[TOC]
# AddTorque2D # {#AddTorque2D}
Add Torque to a Rigidbody2D.

Defined in Fungus.AddTorque2D

Property | Type | Description
 --- | --- | ---
rb | Fungus.Rigidbody2DData | Targeted rigidbody2d
forceMode | Unity.ForceMode2D | Parameter given to AddTorque
force | Fungus.FloatData | Amount of torque to be added

# AddForce2D # {#AddForce2D}
Add Force to a Rigidbody2D.

Defined in Fungus.AddForce2D

Property | Type | Description
 --- | --- | ---
rb | Fungus.Rigidbody2DData | Targeted rigidbody2d
forceMode | Unity.ForceMode2D | Parameter given to AddForce
forceFunction | System.Enum | Which variant of AddForce to use (AddForce,AddForceAtPosition,AddRelativeForce)
force | Fungus.Vector2DData | Amount of torque to be added
forceScaleFactor | Fungus.FloatData | Scale factor to be applied to force as it is used. Default 1.
atPosition | Fungus.Vector2DData | World position the force is being applied from. Used only in AddForceAtPosition

# StopMotion2D # {#StopMotion2D}
Stop motion or angularmotion or both of a rigidbody2d.

Defined in Fungus.StopMotion2D

Property | Type | Description
 --- | --- | ---
rb | Fungus.Rigidbody2DData | Targeted rigidbody2d
motionToStop | System.Enum | Which motion to stop (Velocity,AngularVelocity,AngularAndLinearVelocity)
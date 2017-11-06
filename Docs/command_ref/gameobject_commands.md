# GameObject commands # {#gameobject_commands}

[TOC]
# Instantiate # {#Instantiate}
Instantiate a game object

Defined in Fungus.SpawnObject

Property | Type | Description
 --- | --- | ---
_source Object | Fungus.GameObjectData | Game object to copy when spawning. Can be a scene object or a prefab.
_parent Transform | Fungus.TransformData | Transform to use as parent during instantiate.
_spawn At Self | Fungus.BooleanData | If true, will use the Transfrom of this Flowchart for the position and rotation.
_spawn Position | Fungus.Vector3Data | Local position of newly spawned object.
_spawn Rotation | Fungus.Vector3Data | Local rotation of newly spawned object.
_newly Spawned Object | Fungus.GameObjectData | Optional variable to store the GameObject that was just created.


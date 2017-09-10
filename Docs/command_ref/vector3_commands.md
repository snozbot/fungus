# Vector3 Commands # {#vector3_commands}

Commands that interact with and manipulate Fungus.Vector3Variable

[TOC]
# Arithmetic # {#Arithmetic}
Vector3 add, sub, mul, div arithmetic

Defined in Fungus.Vector3Arithmetic

Property | Type | Description
 --- | --- | ---
lhs | Fungus.Vector3Data | Left hand side of the operation
rhs | Fungus.Vector3Data | Right hand side of the operation
output | Fungus.Vector3Data | Push result of operation into this variable
operation | System.Enum | Operation to perform (Add,Sub,Mul,Div)

# Fields # {#Fields}
Get or Set the x,y,z fields of a vector3 via floatvars

Defined in Fungus.Vector3Fields

Property | Type | Description
 --- | --- | ---
getOrSet | System.Enum | Get or Set the fields of the Vector3.
vec3 | Fungus.Vector3Data | Target Vector3.
x | Fungus.FloatData | x field.
y | Fungus.FloatData | y field.
z | Fungus.FloatData | z field.

# Normalise # {#Normalise}
Normalise a vector3, output can be the same as the input.

Defined in Fungus.Vector3Normalise

Property | Type | Description
 --- | --- | ---
vec3In | Fungus.Vector3Data | Vector3 data to be normalised.
vec3Out | Fungus.Vector3Data | Vector3 to store result of normalisation.

# ToVector2 # {#ToVector2}
Convert Fungus Vector3 to Fungus Vector2.

Defined in Fungus.Vector3ToVector2

Property | Type | Description
 --- | --- | ---
vec3 | Fungus.Vector3Data | Vector3 data to be normalised.
vec2 | Fungus.Vector3Data | Vector2 to store result of normalisation.
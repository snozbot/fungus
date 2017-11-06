# Math commands # {#math_commands}

[TOC]
# Abs # {#Abs}
Command to execute and store the result of a Abs

Defined in Fungus.Abs

Property | Type | Description
 --- | --- | ---
In Value | Fungus.FloatData | Value to be passed in to the function.
Out Value | Fungus.FloatData | Where the result of the function is stored.

# Clamp # {#Clamp}
Command to contain a value between a lower and upper bound, with optional wrapping modes

Defined in Fungus.Clamp

Property | Type | Description
 --- | --- | ---
Out Value | Fungus.FloatData | Result put here, if using pingpong don't use the same var for value as outValue.

# Curve # {#Curve}
Pass a value through an AnimationCurve

Defined in Fungus.Curve

Property | Type | Description
 --- | --- | ---
In Value | Fungus.FloatData | Value to be passed in to the function.
Out Value | Fungus.FloatData | Where the result of the function is stored.

# Exp # {#Exp}
Command to execute and store the result of a Exp

Defined in Fungus.Exp

Property | Type | Description
 --- | --- | ---
In Value | Fungus.FloatData | Value to be passed in to the function.
Out Value | Fungus.FloatData | Where the result of the function is stored.

# Inverse # {#Inverse}
Multiplicative Inverse of a float (1/f)

Defined in Fungus.Inv

Property | Type | Description
 --- | --- | ---
In Value | Fungus.FloatData | Value to be passed in to the function.
Out Value | Fungus.FloatData | Where the result of the function is stored.

# InvLerp # {#InvLerp}
Calculates the inverse lerp, the percentage a value is between two others.

Defined in Fungus.InvLerp

Property | Type | Description
 --- | --- | ---
Clamp Result | System.Boolean | Clamp percentage to 0-1?

# Lerp # {#Lerp}
Linearly Interpolate from A to B

Defined in Fungus.Lerp
# Log # {#Log}
Command to execute and store the result of a Log

Defined in Fungus.Log

Property | Type | Description
 --- | --- | ---
Mode | Fungus.Log+Mode | Which log to use, natural or base 10
In Value | Fungus.FloatData | Value to be passed in to the function.
Out Value | Fungus.FloatData | Where the result of the function is stored.

# Map # {#Map}
Map a value that exists in 1 range of numbers to another.

Defined in Fungus.Map
# MinMax # {#MinMax}
Command to store the min or max of 2 values

Defined in Fungus.MinMax

Property | Type | Description
 --- | --- | ---
Function | Fungus.MinMax+Function | Min Or Max

# Negate # {#Negate}
Negate a float

Defined in Fungus.Neg

Property | Type | Description
 --- | --- | ---
In Value | Fungus.FloatData | Value to be passed in to the function.
Out Value | Fungus.FloatData | Where the result of the function is stored.

# Pow # {#Pow}
Raise a value to the power of another.

Defined in Fungus.Pow

Property | Type | Description
 --- | --- | ---
Out Value | Fungus.FloatData | Where the result of the function is stored.

# Round # {#Round}
Command to execute and store the result of a Round

Defined in Fungus.Round

Property | Type | Description
 --- | --- | ---
Function | Fungus.Round+Mode | Mode; Round (closest), floor(smaller) or ceil(bigger).
In Value | Fungus.FloatData | Value to be passed in to the function.
Out Value | Fungus.FloatData | Where the result of the function is stored.

# Sign # {#Sign}
Command to execute and store the result of a Sign

Defined in Fungus.Sign

Property | Type | Description
 --- | --- | ---
In Value | Fungus.FloatData | Value to be passed in to the function.
Out Value | Fungus.FloatData | Where the result of the function is stored.

# Sqrt # {#Sqrt}
Command to execute and store the result of a Sqrt

Defined in Fungus.Sqrt

Property | Type | Description
 --- | --- | ---
In Value | Fungus.FloatData | Value to be passed in to the function.
Out Value | Fungus.FloatData | Where the result of the function is stored.

# ToInt # {#ToInt}
Command to execute and store the result of a float to int conversion

Defined in Fungus.ToInt

Property | Type | Description
 --- | --- | ---
Function | Fungus.ToInt+Mode | To integer mode; round, floor or ceil.
In Value | Fungus.FloatData | Value to be passed in to the function.
Out Value | Fungus.IntegerData | Where the result of the function is stored.

# Trig # {#Trig}
Command to execute and store the result of basic trigonometry

Defined in Fungus.Trig

Property | Type | Description
 --- | --- | ---
Function | Fungus.Trig+Function | Trigonometric function to run.
In Value | Fungus.FloatData | Value to be passed in to the function.
Out Value | Fungus.FloatData | Where the result of the function is stored.


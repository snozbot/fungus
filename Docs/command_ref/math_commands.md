# Math commands # {#math_commands}

Provides a way for Fungus.FloatData to be used in conjunction with most of [Mathf](https://docs.unity3d.com/ScriptReference/Mathf.html) and more.

[TOC]
# Abs # {#Abs}
Sets the outValue to the be Absolute value of the inValue.

Defined in Fungus.Abs

Property | Type | Description
 --- | --- | ---
inValue | Fungus.FloatData | Value passed into the function (the right hand side).
outValue | Fungus.FloatData | Value the result of the function is saved to (the left hand side). This can be the same Fungus.FloatData as the inValue.

# Clamp # {#Clamp}
Sets the outValue to the result of a clamp of value, between lower and upper.

Defined in Fungus.Clamp

Property | Type | Description
 --- | --- | ---
mode | System.Enum | Clamp or Repeat or Pingpong. See [Repeat and Pingpong](https://docs.unity3d.com/ScriptReference/Mathf.html) for more details.
lower | Fungus.FloatData | The lower bound of the clamp
upper | Fungus.FloatData | The upper bound of the clamp
value | Fungus.FloatData | The value to be clamped
outValue | Fungus.FloatData | Value the result of the function is saved to (the left hand side).

# Curve # {#Curve}
Sets the outValue to the evaluation at inValue of the supplied animation curve. Useful for non linearly remapping values.

Defined in Fungus.Curve

Property | Type | Description
 --- | --- | ---
inValue | Fungus.FloatData | Value passed into the function (the right hand side).
outValue | Fungus.FloatData | Value the result of the function is saved to (the left hand side). This can be the same Fungus.FloatData as the inValue.
curve | Unity.AnimationCurve | See [AnimationCurve](https://docs.unity3d.com/ScriptReference/AnimationCurve.html) for more info. Defaults to a Linear 0,0, to 1,1 AnimationCurve.

# Exp # {#Exp}
Sets the outValue to the be Exp (e^) value of the inValue.

Defined in Fungus.Exp

Property | Type | Description
 --- | --- | ---
inValue | Fungus.FloatData | Value passed into the function (the right hand side).
outValue | Fungus.FloatData | Value the result of the function is saved to (the left hand side). This can be the same Fungus.FloatData as the inValue.

# Inv # {#Inv}
Sets the outValue to the be mutliplicative inverse of the inValue, 1 / inValue.

Defined in Fungus.Inv

Property | Type | Description
 --- | --- | ---
inValue | Fungus.FloatData | Value passed into the function (the right hand side).
outValue | Fungus.FloatData | Value the result of the function is saved to (the left hand side). This can be the same Fungus.FloatData as the inValue.

# InvLerp # {#InvLerp}
Sets the outValue to the Calculates the inverse lerp, the percentage a value is between two others.

Defined in Fungus.InvLerp

Property | Type | Description
 --- | --- | ---
clampResult | System.Boolean | Clamp percentage to 0-1?
a | Fungus.FloatData | Min of the range
b | Fungus.FloatData | Max of the range
value | Fungus.FloatData | Value to determine precentage between a and b.
outValue | Fungus.FloatData | Value the result of the function is saved to (the left hand side).

# Lerp # {#Lerp}
Sets the outValue to the linear interpolation of a percentage between two other values.

Defined in Fungus.Lerp

Property | Type | Description
 --- | --- | ---
mode | System.Enum | Lerp or LerpUnclamped or LerpAngle. See [Lerp functions in Mathf](https://docs.unity3d.com/ScriptReference/Mathf.html) for more details.
a | Fungus.FloatData | Min of the range, default 0.
b | Fungus.FloatData | Max of the range, default 1.
percentage | Fungus.FloatData | percentage between a and b.
outValue | Fungus.FloatData | Value the result of the function is saved to (the left hand side).

# Log # {#Log}
Sets the outValue to the be Log or Ln of the inValue.

Defined in Fungus.Log

Property | Type | Description
 --- | --- | ---
inValue | Fungus.FloatData | Value passed into the function (the right hand side).
outValue | Fungus.FloatData | Value the result of the function is saved to (the left hand side). This can be the same Fungus.FloatData as the inValue.
mode | System.Enum | Base10 or Natural. Base10 is the standard Log, Natural log is often seen as Ln.

# Map # {#Map}
Sets the outValue mapping of a value that currently exists between a set of numbers to another set of numbers. 
E.g. a value of 5 between 0 and 10, mapped to 0-20 would result in 10.

Does not clamp between ranges, use a Fungus.Clamp before or after this command for that if is desired.

Defined in Fungus.Map

Property | Type | Description
 --- | --- | ---
initialRangeLower | Fungus.FloatData | Min of the initial range, default 0.
initialRangeupper | Fungus.FloatData | Max of the initial range, default 1.
value | Fungus.FloatData | Value to be mapped from initial to new range.
newRangeLower | Fungus.FloatData | Min of the new target range, default 0.
newRangeUpper | Fungus.FloatData | Max of the new target range, default 1.
outValue | Fungus.FloatData | Value the result of the function is saved to (the left hand side).

# MinMax # {#MinMax}
Sets the outValue to minimum or the maximum of 2 given values.

Defined in Fungus.MinMax

Property | Type | Description
 --- | --- | ---
function | System.Enum | Min or Max.
inLHSValue | Fungus.FloatData | lhs given to min or max function.
inRHSValue | Fungus.FloatData | rhs given to min or max function.
outValue | Fungus.FloatData | Value the result of the function min or max.

# Neg # {#Neg}
Sets the outValue to the be addative inverse of the inValue, becomes -inValue.

Defined in Fungus.Neg

Property | Type | Description
 --- | --- | ---
inValue | Fungus.FloatData | Value passed into the function (the right hand side).
outValue | Fungus.FloatData | Value the result of the function is saved to (the left hand side). This can be the same Fungus.FloatData as the inValue.

# Pow # {#Pow}
Sets the outValue to result of a base value rasied to an exponent.
E.g. 2^5
2 is the base
5 is the exponent.

Defined in Fungus.Pow

Property | Type | Description
 --- | --- | ---
baseValue | Fungus.FloatData | Base value.
exponentValue | Fungus.FloatData | Exponent value
outValue | Fungus.FloatData | Value the result of the pow function.

# Round # {#Round}
Sets the outValue to the Rounded result of inValue.

Defined in Fungus.Round

Property | Type | Description
 --- | --- | ---
inValue | Fungus.FloatData | Value passed into the function (the right hand side).
outValue | Fungus.FloatData | Value the result of the function is saved to (the left hand side). This can be the same Fungus.FloatData as the inValue.
mode | System.Enum | Round or Floor or Ceil. Round is closest whole number, Floor is the smaller whole number, Ceil is the larger whole number.

# Sign # {#Sign}
Sets the outValue to the be mutliplicative sign of the inValue. -1 for negative number otherwise it is 1.

Defined in Fungus.Sign

Property | Type | Description
 --- | --- | ---
inValue | Fungus.FloatData | Value passed into the function (the right hand side).
outValue | Fungus.FloatData | Value the result of the function is saved to (the left hand side). This can be the same Fungus.FloatData as the inValue.

# Sqrt # {#Sqrt}
Sets the outValue to the be square root of the inValue.

Defined in Fungus.Sqrt

Property | Type | Description
 --- | --- | ---
inValue | Fungus.FloatData | Value passed into the function (the right hand side).
outValue | Fungus.FloatData | Value the result of the function is saved to (the left hand side). This can be the same Fungus.FloatData as the inValue.

# ToInt # {#ToInt}
Sets the outValue to the Rounded to Int result of inValue.

Defined in Fungus.ToInt

Property | Type | Description
 --- | --- | ---
inValue | Fungus.FloatData | Value passed into the function (the right hand side).
outValue | Fungus.IntData | Value the result of the function is saved to (the left hand side). This can be the same Fungus.FloatData as the inValue.
mode | System.Enum | RoundToInt or FloorToInt or CeilToInt. Round is closest integer, Floor is the smaller integer, Ceil is the larger integer.

# Trig # {#Trig}
Sets the outValue to the of a trigonmetric function performed on inValue.

Defined in Fungus.Trig

Property | Type | Description
 --- | --- | ---
inValue | Fungus.FloatData | Value passed into the function (the right hand side).
outValue | Fungus.IntData | Value the result of the function is saved to (the left hand side). This can be the same Fungus.FloatData as the inValue.
function | System.Enum | Rad2Deg, Deg2Rad, ACos, ASin, ATan, Cos, Sin, Tan. Default is Sin.
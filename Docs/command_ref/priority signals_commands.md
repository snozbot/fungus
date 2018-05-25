# Priority Signals commands # {#priority signals_commands}

[TOC]
# Get Priority Count # {#GetPriorityCount}
Copy the value of the Priority Count to a local IntegerVariable, intended primarily to assist with debugging use of Priority.

Defined in Fungus.FungusPriorityCount
# Priority Down # {#PriorityDown}
Decrease the FungusPriority count, causing the related FungusPrioritySignals to fire. Intended to be used to notify external systems that fungus is doing something important and they should perhaps resume.

Defined in Fungus.FungusPriorityDecrease
# Priority Reset # {#PriorityReset}
Resets the FungusPriority count to zero. Useful if you are among logic that is hard to have matching increase and decreases.

Defined in Fungus.FungusPriorityReset
# Priority Up # {#PriorityUp}
Increases the FungusPriority count, causing the related FungusPrioritySignals to fire. Intended to be used to notify external systems that fungus is doing something important and they should perhaps pause.

Defined in Fungus.FungusPriorityIncrease

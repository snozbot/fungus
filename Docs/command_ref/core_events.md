# Core event handlers # {#core_events}

[TOC]
# Flowchart Enabled # {#FlowchartEnabled}
The block will execute when the Flowchart game object is enabled.

Defined in Fungus.FlowchartEnabled
# Game Started # {#GameStarted}
The block will execute when the game starts playing.

Defined in Fungus.GameStarted

Property | Type | Description
 --- | --- | ---
Wait For Frames | System.Int32 | Wait for a number of frames after startup before executing the Block. Can help fix startup order issues.

# Message Received # {#MessageReceived}
The block will execute when the specified message is received from a Send Message command.

Defined in Fungus.MessageReceived

Property | Type | Description
 --- | --- | ---
Message | System.String | Fungus message to listen for


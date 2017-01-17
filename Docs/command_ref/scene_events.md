# Scene event handlers # {#scene_events}

[TOC]
# Flowchart Enabled # {#FlowchartEnabled}
The block will execute when the Flowchart game object is enabled.

Defined in Fungus.FlowchartEnabled
# Message Received # {#MessageReceived}
The block will execute when the specified message is received from a Send Message command.

Defined in Fungus.MessageReceived

Property | Type | Description
 --- | --- | ---
Message | System.String | Fungus message to listen for

# Save Point Loaded # {#SavePointLoaded}
Execute this block when a saved point is loaded. Use the 'new_game' key to handle game start.

Defined in Fungus.SavePointLoaded

Property | Type | Description
 --- | --- | ---
Save Point Keys | System.Collections.Generic.List`1[System.String] | Block will execute if the Save Key of the loaded save point matches this save key.


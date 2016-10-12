# Coding Tips # {#coding_tips}
[TOC]

This is a collection of quick tips for scripting %Fungus from code.

# Executing Blocks # {#executing_blocks}

You first need to get a reference to your flowchart object.  Add a public Flowchart property to your component and set it to reference your flowchart in the inspector.
```
using UnityEngine;
using Fungus;

class MyComponent : public MonoBehaviour
{
	public Flowchart flowchart;
}
```

To execute a named Block in the Flowchart:
```
flowchart.ExecuteBlock("BlockName");
```

To start execution at a specific command index:
```
flowchart.ExecuteBlock("BlockName", 3);
```

To tell if a Flowchart has any executing Blocks:
```
if (flowchart.HasExecutingBlocks())
{
	// Do something
}
```

# Block Signals # {#block_signals}

You can use the BlockSignals class to listen for events from the Block execution system.

```
using Fungus;

public MyComponent : MonoBehaviour
{
    void OnEnable() 
    {
    	// Register as listener for Block events
        BlockSignals.OnBlockStart += OnBlockStart;
    }

    void OnDisable()
    {
    	// Unregister as listener for Block events
        BlockSignals.OnBlockStart -= OnBlockStart;
    }

    void OnBlockStart(Block block)
    {
    	Debug.Log("Block started " + block.BlockName);
    }
}
```

# Writer Signals # {#writer_signals}

You can use the WriterSignals class to listen for a variety of events from the text writing system.

```
using Fungus;

public MyComponent : MonoBehaviour
{
    void OnEnable() 
    {
    	// Register as listener for Writer state change events
        WriterSignals.OnWriterState += OnWriterState;
    }

    void OnDisable()
    {
    	// Unregister as listener for Writer state change events
        WriterSignals.OnWriterState -= OnWriterState;
    }

    void OnWriterState(Writer writer, WriterState writerState)
    {
    	if (writerState == WriterState.Start)
    	{
    		Debug.Log("Writing started");
    	}
    }
}
```


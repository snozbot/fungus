# Narrative Parser Tags # {#narrative_tags}

The following tokens can be used within Story Text to do things such as change the styling of text or clear the contents of a dialog area on input and much more. You can see this list in the editor by pressing the Tag Help button in the Say command.

* {b} Bold Text {/b}
* {i} Italic Text {/i}
* {color=red} Color Text (color){/color}
* {size=30} Text size {/size}
* {s}, {s=60} Writing speed (chars per sec){/s}
* {w}, {w=0.5} Wait (seconds)
* {wi} Wait for input
* {wc} Wait for input and clear
* {wp}, {wp=0.5} Wait on punctuation (seconds){/wp}
* {c} Clear
* {x} Exit, advance to the next command without waiting for input
* {vpunch=10,0.5} Vertically punch screen (intensity,time)
* {hpunch=10,0.5} Horizontally punch screen (intensity,time)
* {punch=10,0.5} Punch screen (intensity,time)
* {flash=0.5} Flash screen (duration)
* {audio=AudioObjectName} Play Audio Once
* {audioloop=AudioObjectName} Play Audio Loop
* {audiopause=AudioObjectName} Pause Audio
* {audiostop=AudioObjectName} Stop Audio
* {m=MessageName} Broadcast message
* {$VarName} Substitute variable

Examples:

```
This is a line of dialog.
{wc}
This is another line of dialog with some {b}bold{/b} styling.
This is another line of dialog with some {color=blue}blue{/b} text.
```

# Audio commands {#audio_commands}

## Control Audio
Plays, loops, or stops an audiosource. Any AudioSources with the same tag as the target Audio Source will automatically be stoped.

Property | Type | Description
 --- | --- | ---
Control | Fungus.Commands.ControlAudioType | What to do to audio
_audio Source | Fungus.Variables.AudioSourceData | Audio clip to play
Start Volume | System.Single | Start audio at this volume
End Volume | System.Single | End audio at this volume
Fade Duration | System.Single | Time to fade between current volume level and target volume level.
Wait Until Finished | System.Boolean | Wait until this command has finished before executing the next command.

## Play Music
Plays looping game music. If any game music is already playing, it is stopped. Game music will continue playing across scene loads.

Property | Type | Description
 --- | --- | ---
Music Clip | UnityEngine.AudioClip | Music sound clip to play
At Time | System.Single | Time to begin playing in seconds. If the audio file is compressed, the time index may be inaccurate.
Loop | System.Boolean | The music will start playing again at end.
Fade Duration | System.Single | Length of time to fade out previous playing music.

## Play Sound
Plays a once-off sound effect. Multiple sound effects can be played at the same time.

Property | Type | Description
 --- | --- | ---
Sound Clip | UnityEngine.AudioClip | Sound effect clip to play
Volume | System.Single | Volume level of the sound effect
Wait Until Finished | System.Boolean | Wait until the sound has finished playing before continuing execution.

## Play Usfxr Sound
Plays a usfxr synth sound. Use the usfxr editor [Tools > Fungus > Utilities > Generate usfxr Sound Effects] to create the SettingsString. Set a ParentTransform if using positional sound. See https://github.com/zeh/usfxr for more information about usfxr.

Property | Type | Description
 --- | --- | ---
Parent Transform | UnityEngine.Transform | Transform to use for positional audio
_ Settings String | Fungus.Variables.StringDataMulti | Settings string which describes the audio
Wait Duration | System.Single | Time to wait before executing the next command

## Set Audio Pitch
Sets the global pitch level for audio played with Play Music and Play Sound commands.

Property | Type | Description
 --- | --- | ---
Pitch | System.Single | Global pitch level for audio played using the Play Music and Play Sound commands
Fade Duration | System.Single | Time to fade between current pitch level and target pitch level.
Wait Until Finished | System.Boolean | Wait until the pitch change has finished before executing next command

## Set Audio Volume
Sets the global volume level for audio played with Play Music and Play Sound commands.

Property | Type | Description
 --- | --- | ---
Volume | System.Single | Global volume level for audio played using Play Music and Play Sound
Fade Duration | System.Single | Time to fade between current volume level and target volume level.
Wait Until Finished | System.Boolean | Wait until the volume fade has completed before continuing.

## Stop Music
Stops the currently playing game music.

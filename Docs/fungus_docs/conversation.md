# The Conversation system {#conversation}

The Say and Portrait commands are a powerful way to build character dialogue, but it can be tedious to add large amounts of dialogue this way.

The Conversation system provides a simplified format that allows you to quickly control:

- Which character is speaking
- Which portrait image to use (either on the Say Dialog or on the Stage portrait)
- Which stage position to move to the character to (if using the Stage)
- When to hide a character

To create a conversation, add the Conversation command (Narrative > Conversation) to a Block and enter the conversation text. 

# Example

```text
john bored left: Oh, so that's how you use the Conversation command.
sherlock eyeroll right: Yes, well done John.
You catch on quickly don't you?
hide john "offscreen left": I sure do.

-- This is a comment, it doesn't appear in the conversation

john angry middle: Wait, what!
left: There's no need to be rude Sherlock!
bored: Not like that would stop you.

sherlock excited: AHA! So that's how you do a conversation from Lua!
Fascinating.
john: Yes, riveting.

john hide:
sherlock hide:
```

# Format

The format for conversation text is:
```text
[character] [portrait] [position] [hide] : [Dialogue text]
```

- character: The gameobject name or Name Text value of the speaking character.
- portrait: The name of a sprite in the character's Portraits list.
- position: The name of a position object in the Stage (e.g. Left, Middle, Right, Offscreen Left, Offscreen Right)
- hide: Hides the character

Parameters go on the left of the colon and the dialogue text goes on the right. You can omit any parameter and specify them in any order. Parameters are separated by spaces. If you need to use a name which contains spaces, wrap it in quotation marks e.g. "John Watson". Parameters are case insensitive. Blank lines and comment lines starting with -- are ignored. A line of dialogue text on its own will be spoken by the most recent character. You can omit dialogue text, but remember you still need to add the : character at the end of the line.

# String substitution

You can use the normal string subsitution syntax {$VarName} anywhere in the conversation text. For example if you have a string Flowchart variable called PlayerName you can embed this in a conversation like this:

```text
john: Hi there {$PlayerName}.
```

# Localization

You can use the string substitution feature above with a @ref lua_string_table "Lua string table" to localize the conversation for multiple languages.

The Conversation system does not work with the Localization component in %Fungus because the syntax makes it difficult to localize that way. 

# Lua

The conversation system can also be used @ref lua_controlling_fungus "from Lua".

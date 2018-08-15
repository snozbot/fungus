# Text Variation System # {#text_variation_system}

A system for selecting a sub section of a larger string. Allows for simple changes to occur within the same block of text, instead of having to create multiple commands. The Text Variation System is inspired by [Ink]'s [Variable Text system](https://github.com/inkle/ink/blob/master/Documentation/WritingWithInk.md#6-variable-text). Fungus' Writer and Menu classes currently use this system, as such Say, Menu and Conversation commands can make use of it. Fungus Lua actions that result in Lua passing strings to the Fungus Writer or Menu will also work.

Handles replacing vary text segments. Keeps history of previous replacements to allow for ordered sequence of variation.
* [] mark the bounds of the vary section
* | divide elements within the variation

Default behaviour is to show one element after another and hold the final element. Such that [1|2|3] will show
1 the first time it is parsed, 2 the second and every subsequent time it will show 3.

Empty sections are allowed, such that [a||c], on second showing it will have 0 characters.

Supports nested sections, that are only evaluated if their parent element is chosen. 

This behaviour can be modified with certain characters at the start of the [], e.g. [&a|b|c];
- & does not hold the final element it wraps back around to the beginning in a looping fashion
- ! does not hold the final element, it instead returns empty for the varying section
- ~ chooses a random element every time it is encountered 

##Example Usage##

In a simple case you may want to have a line read differently the first time the user encounters it. Perhaps a shop keeper, first time they say 
```
Hail and well met, stranger. What can I help you with?
```
but when the same block is run again you want it to be more friendly 
```
Welcome back, friend. What can I help you with?
```
Instead of creating diverging blocks of commands we could use text variation of.
```
[Hail and well met, stranger|Welcome back, friend]. What can I help you with?
```



Another common usage is for commonly repeated blocks where players return for menus or story branching points. You may want there to be some variation so it feels more natural. Perhaps by varying the greeting used to be randomised, among;
* Mornin.
* G'day.
* How's it hanging?
* Oi oi.
* How are we?
* Let's do this.

We could do a variation of
```
[~Mornin.|G'day.|How's it hanging?|Oi oi.|How are we?|Let's do this.]
```

For a more complete and thorough example, see example scene at  FungusExamples\VariationText\TextVariation.unity

##Future Work##

* Bundling of state tracking of variations so they can be saved along with other fungus data in save files
* Loading and Unbundling of state tracking for variations so that variations can continue across instances of the applications life.
* Conditional vary sections: sections that are only valid if a fungus variable is in a certain state.

[Ink]: https://github.com/inkle/ink
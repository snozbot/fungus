# Coding Standard # {#coding_standard}

\brief The %Fungus coding standard is designed to make the source code simple to understand for users and easy to maintain for contributors.

This document is focussed on decisions that we've made for the %Fungus project. For general Unity coding tips, try [50 Tips and Best Practices for Unity](http://www.gamasutra.com/blogs/HermanTulleken/20160812/279100/50_Tips_and_Best_Practices_for_Unity_2016_Edition.php).

# Code layout # {#code_layout}

This is the typical layout of a class in %Fungus:

```
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// Description of the component.
    /// </summary>
    public class MyComponent : MonoBehaviour
    {
        [Tooltip("Tooltip comment displayed in the editor.")]
        [SerializeField] protected float myProperty = 10f;

        protected virtual void MyMethod()
        {
            if (myProperty > 5f)
            {
                Debug.Log("A message");
            }
        }

        #region Public members

        /// <summary>
        /// Documentation comment for public property.
        /// </summary>
        public virtual float MyProperty { get; set; }

        /// <summary>
        /// Aspect ratio of the secondary view rectangle. e.g. a 2:1 aspect ratio = 2/1 = 2.0.
        /// </summary>
        public virtual void DoSomething()
        {
        }

        #endregion
    }
}
```

Things to note:

- using declarations all go together at the top of the file. 
- Remove any unused using declarations (can spot these easily with static code analysis - see below).
- Runtime code goes in the Fungus namespace. 
- Editor code goes in the Fungus.EditorUtils namespace.
- All public classes, structs, enums and class members should be documented using xml comments.
- You can document private and protected members if you want to, but ALL public members must have at least a summary comment.
- Parameter & return descriptions are optional, add them if you feel the parameters require a non-trivial explanation.
- Serialized fields should NEVER be public. Use a public accessor property to access the field if external access is required.
- All serialized fields should have a Tooltip attribute. This doubles as code documentation for the field.
- All methods should be declared virtual and use protected instead of private. This allows for easy inheritance and extension (at the cost of some performance).
- All public members of a class (including public static & delegate types) should be placed inside a 'Public members' region for easy access.
- Braces go on a newline and use spaces exclusively instead of tabs.

# Coding best practices # {#coding_best_practices}

These are some general best practices when writing code for %Fungus. Where these go against the usual recommended coding practice (e.g. Assert) it's because of an issue in Unity with doing it 'the right way'.

- Use the static code analyser in MonoDevelop. http://tinyurl.com/h7xqpwg
- Use the c# xml comment style. https://msdn.microsoft.com/en-us/library/b2s063f7.aspx
- Declare all public enums at namespace scope, not inside a class. (Consistency, easier sharing of enums between classes).
- Use var instead of declaring variable types when possible. (More readable).
- Use for instead of foreach when possible. (Avoids allocating an iterator & GC problems).
- Use string.Format or StringBuilder instead of concatenating strings. (Avoids allocations & GC problems).
- Don't use LINQ. (Avoids allocations & GC problems.)
- Don't use Assert. (We support back to Unity 5.0, before Assert was introduced).
- Use Mathf.Approximately when comparing float variables to constants.
- Treat compiler warnings as errors. There should be zero warnings at build or runtime in normal operation.
- Add global constants to FungusConstants.cs

# Backwards compatibility # {#backwards_compatibility}

We aim to maintain backwards compatibility with each new release (to a reasonable extent).

- Projects should work correctly after upgrading to a newer %Fungus version. Minor behavior changes are acceptable.
- Custom code which uses the %Fungus API should compile without error after upgrading. Minor compile errors that are trivial to fix are sometimes acceptable.
- There are loads of %Fungus tutorial videos and articles on the Internet, so avoid changing the UI too dramatically. Small UI tweaks and adding new controls is acceptable.
- We support Unity 5.0+ so beware of API differences in newer versions. If in doubt, install Unity 5.0 and test your changes.

# Contributing # {#contributing}

We welcome pull requests from everyone. By contributing to this project, you agree to abide by the @ref code_of_conduct. You also agree that by submitting a pull request for this project, your contribution will be licensed under the [Open Source license] for this project.

- Fork and clone the %Fungus repo.
- Make sure the tests pass locally (see the project readme for instructions).
- Make your change. Add tests for your change. Make the tests pass locally.
- Push to your fork and submit a pull request.

Your pull request will have a better chance of being accepted if you do the following: 

- Send one pull request for each new feature / bug fix. It's time consuming to review multi-feature changes and we won't merge a change unless we know exactly what it does.
- Write tests for each change / new feature (not always possible)
- Follow our coding standard (see above)
- Write a [good commit message][commit].

[commit]: http://chris.beams.io/posts/git-commit/
[fork a repo]: https://help.github.com/articles/fork-a-repo/
[Open Source license]: https://github.com/snozbot/Fungus/blob/master/LICENSE

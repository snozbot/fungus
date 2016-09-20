# Unity Test Tools

If you are using the [Unity Test Tools](http://u3d.as/65h), FungusLua is a powerful and fast way to create integration tests using Lua scripting.

# Example

1. Create a new test in the scene.
2. Add a Lua object (Tools > Fungus > Create > Lua) as a child of the test object.
4. In the LuaScript component, use the check() function to assert whatever conditions you need for the test. At the end, call pass().

Example test script:
```lua
-- Check a condition, and output a reason if it fails
check( myvar < 40, "My var is too big")

-- Just check a condition
check( myvar > 20 )

-- Test will exit successfully
pass()
```

If any of the checks fail, then the test fails immediately.

# Lua Functions

```lua
-- Checks if a condition is true
-- Lua has a built in assert function, so we called this check to avoid conflicting.
check(c, reason)

-- Pass an integration test
pass()

-- Fail an integration test
-- reason: Optional string explaining why the test failed.
fail(reason)
```



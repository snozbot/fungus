namespace Fungus
{
    /// <summary>
    /// A simple struct wrapping a reference to a Fungus Variable. Allows for VariableReferenceDrawer. 
    /// This is the a way to directly reference a fungus variable in external c# scripts, it will 
    /// give you an inspector field that gives a drop down of all the variables on the targeted
    /// flowchart, in a similar way to what you would expect from selecting a variable on a command.
    /// </summary>
    [System.Serializable]
    public struct VariableReference
    {
        public Variable variable;

        public T Get<T>()
        {
            T retval = default(T);

            var asType = variable as VariableBase<T>;

            if (asType != null)
                return asType.Value;

            return retval;
        }

        public void Set<T>(T val)
        {
            var asType = variable as VariableBase<T>;

            if (asType != null)
                asType.Value = val;
        }
    }
}
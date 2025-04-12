namespace IdelPog.Engine.Structures.Types
{
    /// <summary>
    /// Contains two readonly strings that describes an objects readable name and readable description
    /// </summary>
    /// <seealso cref="Name"/>
    /// <seealso cref="Description"/>
    public readonly struct Information(string name, string description)
    {
        public readonly string Name = name;
        public readonly string Description = description;
    }
}
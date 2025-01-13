namespace IdelPog.Structures
{
    /// <summary>
    /// Contains two readonly strings that describes an objects readable name and readable description
    /// </summary>
    /// <seealso cref="Name"/>
    /// <seealso cref="Description"/>
    public struct Information
    {
        public readonly string Name;
        public readonly string Description;

        public Information(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
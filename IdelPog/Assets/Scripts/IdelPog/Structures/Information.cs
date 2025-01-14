namespace IdelPog.Structures
{
    /// <summary>
    /// Contains two readonly strings that describes an objects readable name and readable description
    /// </summary>
    /// <seealso cref="Name"/>
    /// <seealso cref="Description"/>
    /// <seealso cref="Create"/>
    public struct Information
    {
        public readonly string Name;
        public readonly string Description;

        private Information(string name, string description)
        {
            Name = name;
            Description = description;
        }

        /// <summary>
        /// Create and return a new <see cref="Information"/> object
        /// </summary>
        /// <param name="name">The name string</param>
        /// <param name="description">The description string</param>
        /// <returns>A newly created <see cref="Information"/> object</returns>
        public static Information Create(string name, string description)
        {
            return new Information(name, description);
        }
    }
}
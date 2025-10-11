namespace IdelPog.Loot.Policy
{
    /// <summary>
    /// Always returns false
    /// </summary>
    public readonly record struct SkipPolicy : IGrantPolicy
    {
        public bool ShouldGrant()
        {
            return false;
        }
    }
}
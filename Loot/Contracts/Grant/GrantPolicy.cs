namespace IdelPog.Loot.Contracts.Grant
{
    /// <summary>
    /// Always returns true
    /// </summary>
    public readonly record struct GrantPolicy : IGrantPolicy
    {
        public bool ShouldGrant()
        {
            return true;
        }
    }
}
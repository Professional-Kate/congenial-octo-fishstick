namespace IdelPog.Core.Contracts
{
    public readonly record struct GrantPolicyEntry
    {
        public required int GrantWeight { get; init; }
        public required int SkipWeight { get; init; }
    }
}
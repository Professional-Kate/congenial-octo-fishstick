namespace IdelPog.Core.Contracts.Error
{
    public readonly record struct SkillUpdateError
    { 
        public required BaseError BaseError { get; init; }
    }
}
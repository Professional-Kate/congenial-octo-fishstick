using IdelPog.Core.Contracts.Command;

namespace IdelPog.Core.Contracts.Error
{
    public readonly record struct HarvestNodeRequirementsCreationError
    {
        public required HarvestNodeRequirementsCreation[] HarvestNodeRequirementsCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}
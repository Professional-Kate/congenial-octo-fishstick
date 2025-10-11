using IdelPog.Core.Contracts;
using IdelPog.HarvestNode.Contracts.Command;

namespace IdelPog.HarvestNode.Contracts.Error
{
    public readonly record struct HarvestNodeRequirementsCreationError
    {
        public required HarvestNodeRequirementsCreation[] HarvestNodeRequirementsCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}
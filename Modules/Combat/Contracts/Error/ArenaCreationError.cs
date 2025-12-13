using IdelPog.Combat.Contracts.Command;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Error
{
    public readonly record struct ArenaCreationError
    {
        public required ArenaCreation[] ArenaCreations { get; init; }
        public required BaseError BaseError { get; init; }
    }
}
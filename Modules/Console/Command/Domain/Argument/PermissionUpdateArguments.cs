using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Console.Command.Domain.Argument
{
    public readonly record struct PermissionUpdateArguments
    {
        public required Types.Domain Domain { get; init; }
        public required ActionType ActionType { get; init; }
    }
}
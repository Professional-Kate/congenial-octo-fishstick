using IdelPog.Common.Enums;

namespace Console.Types
{
    public readonly record struct PermissionUpdateArguments
    {
        public required Domain Domain { get; init; }
        public required ActionType ActionType { get; init; }
    }
}
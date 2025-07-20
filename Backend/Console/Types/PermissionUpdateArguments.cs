using IdelPog.Common.Enums;

namespace Console.Types
{
    public readonly record struct PermissionUpdateArguments
    {
        public required CommandDomain CommandDomain { get; init; }
        public required ActionType ActionType { get; init; }
    }
}
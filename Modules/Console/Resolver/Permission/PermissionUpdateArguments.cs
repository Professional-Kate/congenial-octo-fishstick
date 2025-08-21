using IdelPog.Console.Types;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Console.Resolver.Permission
{
    public readonly record struct PermissionUpdateArguments
    {
        public required Domain Domain { get; init; }
        public required ActionType ActionType { get; init; }
    }
}
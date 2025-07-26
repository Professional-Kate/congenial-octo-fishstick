using Console.Types;
using IdelPog.Common.Enums;

namespace Console.Commands.Domains.Arguments
{
    public readonly record struct PermissionUpdateArguments
    {
        public required Domain Domain { get; init; }
        public required ActionType ActionType { get; init; }
    }
}
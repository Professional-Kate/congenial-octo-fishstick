using Console.Types;
using IdelPog.ECS.Component;

namespace Console.Runtime
{
    public readonly record struct CommandComponent : IComponent
    {
        public required CommandID CommandID { get; init; }
    }
}
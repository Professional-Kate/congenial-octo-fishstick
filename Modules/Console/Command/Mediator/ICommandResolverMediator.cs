namespace IdelPog.Console.Command.Mediator
{
    public interface ICommandResolverMediator
    {
        public void ResolveCommand(Types.Domain domain, ReadOnlySpan<string> arguments);
    }
}
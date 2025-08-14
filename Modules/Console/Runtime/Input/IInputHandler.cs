namespace IdelPog.Console.Runtime.Input
{
    public interface IInputHandler
    {
        public void Input(ReadOnlySpan<string> args);
    }
}
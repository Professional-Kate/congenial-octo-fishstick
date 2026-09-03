namespace IdelPog.Combat.Exceptions
{
    public sealed class MaxIterationsException : Exception
    {
        private const string MESSAGE = "Oops!! That combat was too epic! Too many iterations cause the poor CPU to cry... Max Iterations: {0}";
        
        public readonly uint MaxIterations;
        
        public MaxIterationsException(uint maxIterations) : base(string.Format(MESSAGE, maxIterations))
        {
            MaxIterations = maxIterations;
        }
    }
}
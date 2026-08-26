namespace IdelPog.Combat.Service.Interface
{
    public interface IPrioritySorter
    {
        public IReadOnlyList<T> Sort<T>(IReadOnlyList<T> values, Func<T, byte> prioritySelector);
    }
}
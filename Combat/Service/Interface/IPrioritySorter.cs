using System.Collections.Immutable;

namespace IdelPog.Combat.Service.Interface
{
    public interface IPrioritySorter
    {
        public ImmutableArray<T> Sort<T>(IReadOnlyList<T> values, Func<T, byte> prioritySelector);
    }
}
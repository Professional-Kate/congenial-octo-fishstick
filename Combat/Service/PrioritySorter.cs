using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class PrioritySorter : IPrioritySorter
    {
        public IReadOnlyList<T> Sort<T>(IReadOnlyList<T> values, Func<T, byte> prioritySelector)
        {
            // Insertion sort
            T[] sortedValues = new T[values.Count];
            for (int i = 0; i < values.Count; i++)
            {
                sortedValues[i] = values[i];
            }

            for (int i = 1; i < sortedValues.Length; i++)
            {
                T value = sortedValues[i];
                byte priority = prioritySelector(value);

                int comparisonIndex = i - 1;
                while (comparisonIndex >= 0 && prioritySelector(sortedValues[comparisonIndex]) > priority)
                { 
                    sortedValues[comparisonIndex + 1] = sortedValues[comparisonIndex];
                    comparisonIndex--;
                }
                
                sortedValues[comparisonIndex + 1] = value;
            }
            
            return sortedValues;
        }
    }
}
using IdelPog.Core.Contracts;

namespace IdelPog.Core.Tests.Repository
{
    internal class CloneableTestObject(string value) : ICloneable<CloneableTestObject>
    {
        public string GetValue()
        {
            return value;
        }

        public CloneableTestObject DeepClone()
        {
            return new CloneableTestObject(value);
        }
    }
}
using IdelPog.Common.Structures;

namespace IdelPog.Common.Tests.Repository
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
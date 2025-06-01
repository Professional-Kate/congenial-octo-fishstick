using IdelPog.Infrastructure.Structures;

namespace IdelPog.Infrastructure.Tests.Repository
{
    internal class CloneableTestObject(string value) : ICloneable<CloneableTestObject>
    {
        public string GetValue() => value;
        
        public CloneableTestObject Clone()
        {
            return new CloneableTestObject(value);
        }
    }
}
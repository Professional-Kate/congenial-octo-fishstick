using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Validation.Assertions
{
    public class ObjectNullAssertion : BaseAssertion, IObjectNullAssertion
    {
        public ObjectNullAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertNotNull<T>(T? value, string paramName)
        {
            Assert<ArgumentNullException>(() => { ArgumentNullException.ThrowIfNull(value, paramName); });
        }
    }
}
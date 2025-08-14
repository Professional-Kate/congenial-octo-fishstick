using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Validation.Assertion
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
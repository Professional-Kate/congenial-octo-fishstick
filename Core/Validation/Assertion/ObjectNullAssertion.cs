using IdelPog.Core.Validation.Assertion.Interface;

namespace IdelPog.Core.Validation.Assertion
{
    public sealed class ObjectNullAssertion : IObjectNullAssertion
    {
        public void AssertNotNull<T>(T? value, string paramName)
        {
            ArgumentNullException.ThrowIfNull(value, paramName);
        }
    }
}
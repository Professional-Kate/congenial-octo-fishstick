using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Exceptions;
using IdelPog.Console.Types;

namespace IdelPog.Console.Assertion
{
    public sealed class DomainPermissionAssertion : IDomainPermissionAssertion
    {
        public void AssertHasPermission(bool hasPermission, Domain domainContext)
        {
            if (hasPermission == false)
            {
                throw new DomainPermissionDeniedException(domainContext);
            }
        }
    }
}
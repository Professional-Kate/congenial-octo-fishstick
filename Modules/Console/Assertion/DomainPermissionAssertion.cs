using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Exceptions;
using IdelPog.Console.Types;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Console.Assertion
{
    public class DomainPermissionAssertion : BaseAssertion, IDomainPermissionAssertion
    {
        public DomainPermissionAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertHasPermission(bool hasPermission, Domain domainContext)
        {
            Assert<DomainPermissionDeniedException>(() =>
            {
                if (hasPermission == false)
                {
                    throw new DomainPermissionDeniedException(domainContext);
                }
            });
        }
    }
}
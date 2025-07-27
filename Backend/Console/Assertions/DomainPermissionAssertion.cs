using Console.Exceptions;
using Console.Types;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace Console.Assertions
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
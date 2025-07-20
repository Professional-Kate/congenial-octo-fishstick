using Console.Exceptions;
using Console.Types;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace Console.Assertions
{
    public class AssertHasPermission(IHandler handler) : BaseAssertion<DomainPermissionDeniedException>(handler), IAssertHasPermission
    {
        public void Handle(bool hasPermission, Domain domainContext)
        {
            Assert(() =>
            {
                if (hasPermission == false)
                {
                    throw new DomainPermissionDeniedException(domainContext);
                }
            });
        }
    }
}
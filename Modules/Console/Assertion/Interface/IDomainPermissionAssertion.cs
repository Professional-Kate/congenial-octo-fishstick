using IdelPog.Console.Types;

namespace IdelPog.Console.Assertion.Interface
{
    public interface IDomainPermissionAssertion
    {
        public void AssertHasPermission(bool hasPermission, Domain domainContext);
    }
}
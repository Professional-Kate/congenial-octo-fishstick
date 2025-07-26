using Console.Types;

namespace Console.Assertions
{
    public interface IDomainPermissionAssertion
    {
        public void AssertHasPermission(bool hasPermission, Domain domainContext);
    }
}
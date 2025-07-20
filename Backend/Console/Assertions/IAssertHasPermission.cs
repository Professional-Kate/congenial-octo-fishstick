using Console.Types;

namespace Console.Assertions
{
    public interface IAssertHasPermission
    {
        public void Handle(bool hasPermission, CommandDomain commandDomainContext);
    }
}
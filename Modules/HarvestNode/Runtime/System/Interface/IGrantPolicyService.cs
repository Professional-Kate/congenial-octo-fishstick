using IdelPog.Core.Contracts;

namespace IdelPog.HarvestNode.Runtime.System.Interface
{
    public interface IGrantPolicyService<in TID>
    {
        public void CreateGrantPolicy(GrantPolicyEntry grantPolicyEntry, TID id);
    }
}
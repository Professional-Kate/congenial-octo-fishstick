using IdelPog.Core.Contracts.Enum;
using IdelPog.HarvestNode.Contracts;
using IdelPog.HarvestNode.Contracts.Response;
using IdelPog.Progression.Runtime;

namespace IdelPog.HarvestNode.Runtime.Factory.Interface
{
    public interface IUnlockRequirementsEntityFactory
    {
        public UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse> Create(SkillID skillID, HarvestNodeRequirement[] harvestNodeRequirements);
    }
}
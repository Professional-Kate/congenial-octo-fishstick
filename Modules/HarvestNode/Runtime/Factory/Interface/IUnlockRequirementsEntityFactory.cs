using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Progression.Runtime;

namespace IdelPog.HarvestNode.Runtime.Factory.Interface
{
    public interface IUnlockRequirementsEntityFactory
    {
        public UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse> Create(SkillID skillID, HarvestNodeRequirement[] harvestNodeRequirements);
    }
}
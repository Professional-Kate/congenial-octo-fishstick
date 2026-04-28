using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.Asserter;
using IdelPog.HarvestNode.Contracts;
using IdelPog.HarvestNode.Runtime.ECS;
using IdelPog.HarvestNode.Runtime.Factory.Interface;

namespace IdelPog.HarvestNode.Runtime.Factory
{
    public sealed class SkillNodeEntityFactory : ISkillNodeEntityFactory
    {
        private readonly IRepositoryAsserter _repositoryAsserter;

        public SkillNodeEntityFactory(IRepositoryAsserter repositoryAsserter)
        {
            _repositoryAsserter = repositoryAsserter;
        }

        public SkillNodeEntity Create(SkillID skillID, ReadOnlyHarvestNode[] readOnlyHarvestNodes)
        {
            HarvestTargetComponent[] resourceComponents = new HarvestTargetComponent[readOnlyHarvestNodes.Length];
            
            for (int i = 0; i < readOnlyHarvestNodes.Length; i++)
            {
                resourceComponents[i] = new HarvestTargetComponent { HarvestTarget = readOnlyHarvestNodes[i].ResourceID };
            }
            
            return new SkillNodeEntity(_repositoryAsserter, resourceComponents) { SkillID = skillID };
        }
    }
}
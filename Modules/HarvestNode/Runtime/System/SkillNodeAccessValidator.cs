using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.HarvestNode.Runtime.ECS;
using IdelPog.HarvestNode.Runtime.System.Interface;

namespace IdelPog.HarvestNode.Runtime.System
{
    public class SkillNodeAccessValidator : ISkillNodeAccessValidator 
    {
        private readonly IAssetRepository<SkillID, SkillNodeEntity> _skillNodeEntityRepository;
        private readonly IFoundAssertion _foundAssertion;

        public SkillNodeAccessValidator(IAssetRepository<SkillID, SkillNodeEntity> skillNodeEntityRepository, IFoundAssertion foundAssertion)
        {
            _skillNodeEntityRepository = skillNodeEntityRepository;
            _foundAssertion = foundAssertion;
        }

        public void AssertSkillAllows(SkillID skillID, ItemID itemID)
        {
            _foundAssertion.AssertFound(skillID, _skillNodeEntityRepository.Contains(skillID));
            SkillNodeEntity skillNodeEntity = _skillNodeEntityRepository.Get(skillID);
            _foundAssertion.AssertFound(itemID, skillNodeEntity.Allows(itemID));
        }
    }
}
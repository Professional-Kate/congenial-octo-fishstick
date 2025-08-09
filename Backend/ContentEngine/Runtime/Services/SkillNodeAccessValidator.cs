using ContentEngine.Runtime.ECS;
using IdelPog.Common.Enums;
using IdelPog.Common.Repository;
using IdelPog.Validation.Assertions;

namespace ContentEngine.Runtime.Services
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

        public void AssertSkillAllows(SkillID skillID, ResourceID resourceID)
        {
            _foundAssertion.AssertFound(skillID, _skillNodeEntityRepository.Contains(skillID));
            SkillNodeEntity skillNodeEntity = _skillNodeEntityRepository.Get(skillID);
            _foundAssertion.AssertFound(resourceID, skillNodeEntity.Allows(resourceID));
        }
    }
}
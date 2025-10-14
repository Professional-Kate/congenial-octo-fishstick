using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Repository.Asserter;
using IdelPog.HarvestNode.Contracts;
using IdelPog.HarvestNode.Contracts.Response;
using IdelPog.HarvestNode.Runtime.Factory.Interface;
using IdelPog.Progression.Runtime;
using IdelPog.Progression.Runtime.Component;

namespace IdelPog.HarvestNode.Runtime.Factory
{
    public sealed class UnlockRequirementsEntityFactory : IUnlockRequirementsEntityFactory
    {
        private readonly IRepositoryAsserter _repositoryAsserter;

        public UnlockRequirementsEntityFactory(IRepositoryAsserter repositoryAsserter)
        {
            _repositoryAsserter = repositoryAsserter;
        }

        public UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse> Create(SkillID skillID, HarvestNodeRequirement[] harvestNodeRequirements)
        {
            LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse>[] requirementComponents = new LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse>[harvestNodeRequirements.Length];

            for (int i = 0; i < harvestNodeRequirements.Length; i++)
            {
                HarvestNodeRequirement requirement = harvestNodeRequirements[i];

                LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse> component = new()
                {
                    ID = skillID,
                    Level = requirement.RequiredLevel,
                    OnUnlockCommand = requirement.OnUnlockCommand
                };
                
                requirementComponents[i] = component;
            }

            return new UnlockRequirementsEntity<SkillID, HarvestNodeUnlockResponse>(_repositoryAsserter, requirementComponents);
        }
    }
}
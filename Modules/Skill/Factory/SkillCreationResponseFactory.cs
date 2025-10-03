using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Response;
using IdelPog.Skill.Factory.Interface;

namespace IdelPog.Skill.Factory
{
    public sealed class SkillCreationResponseFactory : ISkillCreationResponseFactory
    {
        public SkillCreationResponse[] Create(SkillCreation[] skillCreations)
        {
            SkillCreationResponse[] responses = new SkillCreationResponse[skillCreations.Length];
            for (int i = 0; i < skillCreations.Length; i++)
            {
                SkillCreation skillCreation = skillCreations[i];
                responses[i] = new SkillCreationResponse
                {
                    SkillID = skillCreation.SkillID,
                    ReadOnlyLevelable = skillCreation.ReadOnlyLevelable, 
                    Information =  skillCreation.Information
                };
            }

            return responses;
        }
    }
}
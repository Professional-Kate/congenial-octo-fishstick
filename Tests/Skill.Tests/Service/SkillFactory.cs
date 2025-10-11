using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Progression;

namespace IdelPog.Skills.Tests.Service
{
    internal static class SkillFactory
    {
        internal static Skill.Contracts.Skill CreateMining()
        {
            Levelable levelable = new(1, 0, 10, 0);

            return new Skill.Contracts.Skill
            {
                Information = new Information { Description = "Created", Name = "pog" },
                Levelable = levelable,
                SkillID = SkillID.MINING
            };
        }
    }
}
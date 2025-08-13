using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Progression;
using IdelPog.Skill.Contracts;

namespace Skills.Tests
{
    internal static class SkillFactory
    {
        internal static Skill CreateMining()
        {
            Levelable levelable = new(1, 0, 10, 0);

            return new Skill
            {
                Information = new Information { Description = "Created", Name = "pog" },
                Levelable = levelable,
                SkillID = SkillID.MINING
            };
        }
    }
}
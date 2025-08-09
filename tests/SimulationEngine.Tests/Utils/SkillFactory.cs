using IdelPog.Common.Enums;
using IdelPog.Common.Structures;
using IdelPog.SimulationEngine.Models;

namespace IdelPogTests.Utils
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
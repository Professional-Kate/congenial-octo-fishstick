using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Structures.Types;

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
using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Constants;
using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Skill;

namespace IdelPogTests.Utils
{
    internal static class SkillFactory
    {
        internal static Skill CreateMining()
        {
            ILevelable levelable = new Levelable(1, 0, 10, 0);
            
            return new Skill(levelable, SkillID.MINING, SkillConstants.MINING_INFO);
        }
    }
}
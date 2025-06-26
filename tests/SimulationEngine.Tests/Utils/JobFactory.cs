using IdelPog.SimulationEngine.Constants;
using IdelPog.SimulationEngine.Flows.Skill;
using IdelPog.SimulationEngine.Models;

namespace IdelPogTests.Utils
{
    internal static class JobFactory
    {
        internal static Skill CreateMining()
        {
            ILevelable levelable = new Levelable(1, 0, 10, 0);
            
            return new Skill(levelable, SkillID.MINING, JobConstants.MINING_INFO);
        }
    }
}
using System;
using IdelPog.Constants;
using IdelPog.Exceptions;
using IdelPog.Structures.Models.Levelable;

namespace IdelPog.Service.Level
{
    public class LevelService : ILevelService   
    {
        public void LevelUpJob(ILevelable levelable)
        {
            if (levelable == null)
            {
                throw new ArgumentNullException(nameof(levelable));
            }

            if (levelable.Level == JobConstants.MAX_JOB_LEVEL)
            {
                throw new MaxLevelException($"Error! Passed Job {levelable} is at max level. No level up possible!");
            }

            int total = 0;
            for (int i = 1; i < levelable.Level; i++)
            {
                total += Convert.ToInt32(Math.Floor(i + 83 * Math.Pow(2, i / 7.0)));
            }

            levelable.LevelUp();
            levelable.SetNextLevelExperience(total);
        }
    }
}
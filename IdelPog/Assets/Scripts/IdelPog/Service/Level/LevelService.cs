using System;
using IdelPog.Constants;
using IdelPog.Exceptions;
using IdelPog.Model;

namespace IdelPog.Service
{
    public class LevelService : ILevelService   
    {
        public void LevelUpJob(Job job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            if (job.Level == JobConstants.MAX_JOB_LEVEL)
            {
                throw new MaxLevelException($"Error! Passed Job {job} is at max level. No level up possible!");
            }

            int total = 0;
            for (int i = 1; i < job.Level; i++)
            {
                total += Convert.ToInt32(Math.Floor(i + 83 * Math.Pow(2, i / 7.0)));
            }

            job.LevelUp(total);
        }
    }
}
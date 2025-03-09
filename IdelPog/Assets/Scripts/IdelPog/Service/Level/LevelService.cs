using System;
using IdelPog.Model;
using IdelPog.Validation.Pipelines.Interfaces;

namespace IdelPog.Service
{
    public class LevelService : ILevelService   
    {
        private readonly ILevelableAsserter _levelableAsserter;
        
        public LevelService(ILevelableAsserter levelableAsserter)
        {
            _levelableAsserter = levelableAsserter;
        }
        
        public void LevelUpJob(Job job)
        {
            _levelableAsserter.AssertLevelable(job);

            int total = 0;
            for (int i = 1; i < job.Level; i++)
            {
                total += Convert.ToInt32(Math.Floor(i + 83 * Math.Pow(2, i / 7.0)));
            }

            job.LevelUp(total);
        }
    }
}
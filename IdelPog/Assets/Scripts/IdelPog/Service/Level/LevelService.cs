using System;
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
            
            // TODO: decide on a better XP formula.
            int newExperienceToLevelUp = 10 * job.Level;
            job.LevelUp(newExperienceToLevelUp);
        }
    }
}
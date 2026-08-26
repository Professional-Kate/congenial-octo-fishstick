using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Service
{
    public sealed class CastingCalculator : ICastingCalculator
    {
        public double GetCastDuration(uint combatantSpeed, uint abilityCastTime)
        {
            const double speedScalingFactor = 0.01;
            double nextTick = abilityCastTime * speedScalingFactor / combatantSpeed;
            
            return nextTick;
        }
    }
}
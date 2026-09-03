using IdelPog.Combat.Ability.Service.Interface;

namespace IdelPog.Combat.Ability.Service
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
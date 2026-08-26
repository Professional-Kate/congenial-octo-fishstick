namespace IdelPog.Combat.Service.Interface
{
    public interface ICastingCalculator
    {
        public double GetCastDuration(uint combatantSpeed, uint abilityCastTime);
    }
}
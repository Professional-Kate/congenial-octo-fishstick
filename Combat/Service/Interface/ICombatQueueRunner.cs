using IdelPog.Combat.Contracts.Command;

namespace IdelPog.Combat.Service.Interface
{
    public interface ICombatQueueRunner
    {
        public void RunDeck(BasicEncounterDeck basicEncounterDeck);
    }
}
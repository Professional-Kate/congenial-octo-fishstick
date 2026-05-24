using IdelPog.Combat.Contracts.Command;

namespace IdelPog.Combat.Service.Queue.Interface
{
    public interface ICombatQueueRunner
    {
        public void RunDeck(BasicEncounterDeck basicEncounterDeck);
    }
}
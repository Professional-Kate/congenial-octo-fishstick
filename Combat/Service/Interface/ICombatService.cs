using IdelPog.Combat.Contracts.Deck;
using IdelPog.Combat.Contracts.Response;

namespace IdelPog.Combat.Service.Interface
{
    public interface ICombatService
    { 
        public EncounterResponse RunEncounter(BasicEncounterDeck basicEncounterDeck);
    }
}
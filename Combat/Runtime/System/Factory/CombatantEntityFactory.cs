using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.Combat.Runtime.System.Factory.Interface;
using IdelPog.Core.Repository.Asserter;

namespace IdelPog.Combat.Runtime.System.Factory
{
    public sealed class CombatantEntityFactory : ICombatantEntityFactory
    {
        private readonly IRepositoryAsserter _repositoryAsserter;

        public CombatantEntityFactory(IRepositoryAsserter repositoryAsserter)
        {
            _repositoryAsserter = repositoryAsserter;
        }

        public CombatantEntity CreateEntity(CombatantCreation combatantCreation, byte combatantID)
        {
            return new CombatantEntity(_repositoryAsserter, combatantCreation.StatCard)
            {
                CombatantID = combatantID,
                CombatantType =  combatantCreation.CombatantType,
                CombatantInformation = combatantCreation.Information
            };
        }
    }
}
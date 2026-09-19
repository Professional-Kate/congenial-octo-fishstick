using IdelPog.Combat.Ability.Runtime.Entities;
using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.Combat.Core.Logging.Contracts;
using IdelPog.Combat.Core.Logging.Interface;

namespace IdelPog.Combat.Core.Logging
{
    public sealed class InitialStateLogger : IInitialStateLogger
    {
        private readonly IReadOnlyCombatantFactory _readOnlyCombatantFactory;
        private readonly IReadOnlyAbilityFactory _readOnlyAbilityFactory;

        public InitialStateLogger(IReadOnlyCombatantFactory readOnlyCombatantFactory, IReadOnlyAbilityFactory readOnlyAbilityFactory)
        {
            _readOnlyCombatantFactory = readOnlyCombatantFactory;
            _readOnlyAbilityFactory = readOnlyAbilityFactory;
        }

        public InitialEntities LogInitialState(CombatantEntity[] friendlyCombatants, CombatantEntity[] enemyCombatants, AbilityEntity[] abilityEntities)
        {
            InitialEntities initialEntities = new()
            {
                FriendlyCombatants = _readOnlyCombatantFactory.Create(friendlyCombatants),
                EnemyCombatants = _readOnlyCombatantFactory.Create(enemyCombatants),
                Abilities = _readOnlyAbilityFactory.Create(abilityEntities)
            };
            
            return initialEntities;
        }
    }
}
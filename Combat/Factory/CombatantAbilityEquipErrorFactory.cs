using IdelPog.Combat.Contracts.Command;
using IdelPog.Combat.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.Combat.Factory
{
    public sealed class CombatantAbilityEquipErrorFactory : IErrorFactory<CombatantAbilityEquipError, IReadOnlyList<CombatantAbilityEquip>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public CombatantAbilityEquipErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public CombatantAbilityEquipError Create<TException>(TException exception, IReadOnlyList<CombatantAbilityEquip> context) where TException : Exception
        {
            return new CombatantAbilityEquipError
            {
                CombatantAbilityEquips = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
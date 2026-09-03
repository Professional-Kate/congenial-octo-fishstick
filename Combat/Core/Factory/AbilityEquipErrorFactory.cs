using IdelPog.Combat.Combatant.Contracts.Command;
using IdelPog.Combat.Combatant.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.Combat.Core.Factory
{
    public sealed class AbilityEquipErrorFactory : IErrorFactory<AbilityEquipError, IReadOnlyList<AbilityEquip>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public AbilityEquipErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public AbilityEquipError Create<TException>(TException exception, IReadOnlyList<AbilityEquip> context) where TException : Exception
        {
            return new AbilityEquipError
            {
                AbilityEquips = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
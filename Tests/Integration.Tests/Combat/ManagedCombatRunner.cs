using IdelPog.Combat.Stat.Contracts.Command;
using IdelPog.Combat.Stat.Contracts.Enum;
using IdelPog.Integration.Tests.Combat.Tools;

namespace IdelPog.Integration.Tests.Combat
{
    public abstract class ManagedCombatRunner : ManagedTestBuffer
    {
        [SetUp]
        protected override void BaseSetUp()
        {
            base.BaseSetUp();

            StatType[] statTypes = Enum.GetValues<StatType>();
            StatCreation[] statCreations = new StatCreation[statTypes.Length];
            for (int i = 0; i < statTypes.Length; i++)
            { 
                statCreations[i] = new StatCreation { InitialValue = 0 };
            }

            DispatchMessage(statCreations);
            DispatchMessage(StaticCombatCommands.StatConfiguration);
        }
    }
}
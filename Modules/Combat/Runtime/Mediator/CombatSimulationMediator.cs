using IdelPog.Combat.Contracts.Command;
using IdelPog.Core.Messaging.Listener.Buffer;

namespace IdelPog.Combat.Runtime.Mediator
{
    public sealed class CombatSimulationMediator : IBatchMediator<CombatSimulation>
    {
        public void HandleMessages(IReadOnlyList<CombatSimulation> messages)
        {
            throw new NotImplementedException();
        }
    }
}
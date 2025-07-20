using IdelPog.Common.Commands;
using IdelPog.Messaging.Listeners;
using IdelPog.SimulationEngine.Skill;

namespace Integration.Tests.SkillCommands.Switch
{
    public class SkillChangeDTOListener : ISingleListener<SkillChange>
    {
        public Type ListenerType { get; } = typeof(SkillChange);
        public SkillChange SkillChangeDTO { get; private set; }
        public bool WasCalled { get; private set; }

        public void Handle(SkillChange item)
        {
            SkillChangeDTO = item;
            WasCalled = true;
        }
    }
}
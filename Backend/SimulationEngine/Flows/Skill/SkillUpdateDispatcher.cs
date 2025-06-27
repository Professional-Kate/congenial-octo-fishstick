using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Orchestration;

namespace IdelPog.SimulationEngine.Flows.Skill
{
    public class SkillUpdateDispatcher(IBufferManager bufferManager) : ISkillUpdateDispatcher
    {
        public void Dispatch(SkillUpdateDTO skillUpdateDTO)
        {
            IBuffer<SkillUpdateDTO> buffer = bufferManager.RequestBuffer<SkillUpdateDTO>(new BufferRequest(1));
            buffer.Assign([skillUpdateDTO]);
            buffer.MarkReady();
        }
    }
}
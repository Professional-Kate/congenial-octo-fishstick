using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Orchestration;

namespace IdelPog.SimulationEngine.Skill
{
    public class SkillChangeDispatcher : ISkillChangeDispatcher
    {
        private readonly IBufferManager _bufferManager;

        public SkillChangeDispatcher(IBufferManager bufferManager)
        {
            _bufferManager = bufferManager;
        }
        
        public void Dispatch(SkillChangeDTO skillChangeDTO)
        {
            IBuffer<SkillChangeDTO> buffer = _bufferManager.RequestBuffer<SkillChangeDTO>(new BufferRequest(1));
            buffer.Assign([skillChangeDTO]);
            buffer.MarkReady();
        }
    }
}
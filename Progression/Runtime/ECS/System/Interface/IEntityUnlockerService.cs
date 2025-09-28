namespace IdelPog.Progression.Runtime.ECS.System.Interface
{
    public interface IEntityUnlockerService<in TID, out TResponse>
    {
        public bool CanUnlock(TID id, byte skillLevel);
        
        public TResponse Unlock(TID id, byte skillLevel);
    }
}
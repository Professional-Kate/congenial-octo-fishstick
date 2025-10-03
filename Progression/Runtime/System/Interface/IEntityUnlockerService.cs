namespace IdelPog.Progression.Runtime.System.Interface
{
    public interface IEntityUnlockerService<in TID, out TResponse>
    {
        public bool CanUnlock(TID id, byte skillLevel);
        
        public TResponse Unlock(TID id, byte skillLevel);

        public IEnumerable<TResponse> UnlockAllAvailable(TID id, byte skillLevel);
    }
}
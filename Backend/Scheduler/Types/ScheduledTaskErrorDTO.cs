using IdelPog.Common.DTO.Error;

namespace Scheduler.Types
{
    public readonly record struct ScheduledTaskErrorDTO
    {
        public required ErrorDTO ErrorDTO { get; init; }
        public required Type TaskType { get; init; }
    }
}
using IdelPog.Common.DTO;

namespace Scheduler.Types
{
    public readonly record struct ScheduledTaskErrorDTO
    {
        public required ErrorDTO ErrorDTO { get; init; }
        public required Type TaskType { get; init; }
    }
}
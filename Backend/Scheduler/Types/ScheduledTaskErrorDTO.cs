using IdelPog.Common.Errors;

namespace Scheduler.Types
{
    public readonly record struct ScheduledTaskErrorDTO
    {
        public required BaseError BaseError { get; init; }
        public required Type TaskType { get; init; }
    }
}
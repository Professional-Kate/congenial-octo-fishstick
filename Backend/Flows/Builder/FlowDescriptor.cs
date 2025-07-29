namespace IdelPog.Flows.Builder
{
    public readonly record struct FlowDescriptor
    {
        public required Type CommandType { get; init; }
        public required Type ControllerType { get; init; }
        public required Type MediatorType { get; init; }
        public required Type SuccessResultType { get; init; }
        public required Type ErrorResultType { get; init; }
    }
}
namespace SharedKernel.Meta;

public static class MetadataConstant
{
    public static class MetadataKey
    {
        public const string Timestamp = nameof(Timestamp);
        public const string MemberType = nameof(MemberType);
        public const string CorrelationId = nameof(CorrelationId);
        public const string RequestId = nameof(RequestId);
        public const string CausationId = nameof(CausationId);
        public const string EventId = nameof(EventId);
        public const string AggregateVersion = nameof(AggregateVersion);
    }
}

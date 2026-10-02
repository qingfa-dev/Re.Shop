namespace SharedKernel.Structures.Meta;

public static class MetadataConstant
{
    public static class MetadataKey
    {
        // ── Generic / status ────────────────────────────────
        public const string Code          = "code";
        public const string Message       = "message";
        public const string Status        = "status";
        public const string Type          = "type";
        public const string Target        = "target";
        public const string Priority      = "priority";
        public const string ContentType   = "contentType";
        public const string SchemaVersion = "schemaVersion";
        public const string MemberType    = "memberType";

        // ── Time ────────────────────────────────────────────
        public const string Timestamp         = "timestamp";
        public const string OccurredOn        = "occurredOn";
        public const string ScheduledAt       = "scheduledAt";
        public const string CommitTimestamp   = "commitTimestamp";
        public const string SnapshotTakenAt   = "snapshotTakenAt";

        // ── Identity / actor ────────────────────────────────
        public const string EventId  = "eventId";
        public const string UserId   = "userId";
        public const string TenantId = "tenantId";
        public const string Source   = "source";

        // ── Correlation / tracing ───────────────────────────
        public const string CorrelationId  = "correlationId";
        public const string CausationId    = "causationId";
        public const string RequestId      = "requestId";
        public const string TraceId        = "traceId";
        public const string SpanId         = "spanId";
        public const string ParentId       = "parentId";
        public const string IdempotencyKey = "idempotencyKey";

        // ── Domain / aggregate ──────────────────────────────
        public const string AggregateId      = "aggregateId";
        public const string AggregateType    = "aggregateType";
        public const string AggregateVersion = "aggregateVersion";

        // ── Event sourcing ──────────────────────────────────
        public const string StreamId        = "streamId";
        public const string StreamName      = "streamName";
        public const string StreamPosition  = "streamPosition";
        public const string GlobalPosition  = "globalPosition";
        public const string CommitId        = "commitId";
        public const string IsSnapshot      = "isSnapshot";
        public const string SnapshotVersion = "snapshotVersion";

        // ── Integration / broker ────────────────────────────
        public const string MessageId        = "messageId";
        public const string Destination      = "destination";
        public const string RoutingKey       = "routingKey";
        public const string PartitionKey     = "partitionKey";
        public const string ReplyTo          = "replyTo";
        public const string DeliveryAttempts = "deliveryAttempts";
        public const string SagaId           = "sagaId";
        public const string ProcessId        = "processId";
    }

    public static class Errors
    {
        public static class Key
        {
            public static class NullOrWhitespace
            {
                public const string Code    = "metadata.key.null_or_whitespace";
                public const string Message = "The metadata key cannot be null or whitespace.";
            }
        }

        public static class Value
        {
            public static class Null
            {
                public const string Code    = "metadata.value.null";
                public const string Message = "The metadata value cannot be null.";
            }
        }
    }
}

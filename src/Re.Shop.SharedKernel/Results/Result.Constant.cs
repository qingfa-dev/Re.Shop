namespace SharedKernel.Results;

public static class ResultConstant
{
    // Constraints:
    public static class Constraint
    {
        public static class Errors
        {
            public const int MaxCount = 10;
        }

        public static class Metadata
        {
            public const int MaxEntries = 50;
        }

        public static class Status
        {
            public const int Min = 100;
            public const int Max = 599;

            public static class Success
            {
                public const int Min = 200;
                public const int Max = 299;
            }

            public static class Failure
            {
                public const int Min = 400;
                public const int Max = 599;
            }
        }
    }

    // Defaults:
    public static class Default
    {
        public const int Status = 500;
    }

    // Status codes:
    public static class StatusCode
    {
        public static class Success
        {
            public const int Ok = 200;
            public const int Created = 201;
            public const int Accepted = 202;
            public const int NoContent = 204;
        }
    }

    // Failures:
    public static class Failure
    {
        public static class Result
        {
            public static class MixedStatusCodes
            {
                public const int Status = Default.Status;
                public const string Code = "result.errors.mixed_status_codes";
                public const string Message = "All errors in a result must have the same HTTP status code.";
            }

            public static class FailureWithoutErrors
            {
                public const int Status = Default.Status;
                public const string Code = "result.failure.without_errors";
                public const string Message = "A failed result must contain at least one error.";
            }
        }

        public static class Errors
        {
            public static class Empty
            {
                public const string Code = "result.errors.empty";
                public const string Message = "At least one error is required.";
            }

            public static class ExceedsMaxRange
            {
                public const string Code = "result.errors.exceeds_max_range";
                public const string Message = "A result cannot contain more than {0} errors.";
            }
        }

        public static class Metadata
        {
            public static class EmptyKey
            {
                public const string Code = "result.metadata.empty_key";
                public const string Message = "Metadata keys cannot be null, empty, or whitespace.";
            }

            public static class NullValue
            {
                public const string Code = "result.metadata.null_value";
                public const string Message = "Metadata value for key '{0}' cannot be null.";
            }
        }

        public static class Value
        {
            public static class Required
            {
                public const string Code = "result.value.required";
                public const string Message = "A success result must have a non-null value.";
            }

            public static class NotAllowed
            {
                public const string Code = "result.value.not_allowed";
                public const string Message = "A failure result cannot have a value.";
            }

            public static class ExceedsMaxMetadataEntries
            {
                public const string Code = "result.metadata.exceeds_max_entries";
                public const string Message = "Metadata cannot contain more than {0} entries.";
            }
        }

        public static class Status
        {
            public static class InvalidStatusCode
            {
                public const string Code = "result.status.invalid_status_code";
                public const string Message = "Status code must be between {0} and {1}, but was {2}.";
            }

            public static class InvalidSuccessStatus
            {
                public const string Code = "result.status.invalid_success_status";
                public const string Message = "Success result must have a status code between {0} and {1}, but was {2}.";
            }

            public static class InvalidFailureStatus
            {
                public const string Code = "result.status.invalid_failure_status";
                public const string Message = "Failure result must have a status code between {0} and {1}, but was {2}.";
            }
        }

        public static class ValueResult
        {
            public static class MixedStatusCodes
            {
                public const int Status = Default.Status;
                public const string Code = "result.errors.mixed_status_codes";
                public const string Message = "All errors in a result must have the same HTTP status code.";
            }

            public static class FailureWithoutErrors
            {
                public const int Status = Default.Status;
                public const string Code = "result.failure.without_errors";
                public const string Message = "A failed result must contain at least one error.";
            }

            public static class EmptyErrors
            {
                public const int Status = Default.Status;
                public const string Code = "result.errors.empty";
                public const string Message = "At least one error is required.";
            }

            public static class ExceedsMaxErrors
            {
                public const int Status = Default.Status;
                public const string Code = "result.errors.exceeds_max_range";
                public const string Message = "A result cannot contain more than {0} errors.";
            }

            public static class MetadataEmptyKey
            {
                public const int Status = Default.Status;
                public const string Code = "result.metadata.empty_key";
                public const string Message = "Metadata keys cannot be null, empty, or whitespace.";
            }

            public static class MetadataNullValue
            {
                public const int Status = Default.Status;
                public const string Code = "result.metadata.null_value";
                public const string Message = "Metadata value for key '{0}' cannot be null.";
            }
        }
    }
}

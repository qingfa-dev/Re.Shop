using BuildingBlock.Kernel.Errors;

namespace BuildingBlock.Kernel.Results;

public static class ResultConstant
{
    #region Constraints

    public static class Constraint
    {
        public static class Errors
        {
            public const int MaxCount = 50;
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

    #endregion

    #region Defaults

    public static class Default
    {
        public const int Status = 500;
        public static readonly IReadOnlyList<IError> EmptyErrors = Array.Empty<IError>();
    }

    #endregion

    #region Status Codes

    public static class StatusCode
    {
        #region Success

        // ── 2xx ─────────────────────────────────────────────────────────
        public const int Ok = 200;
        public const int Created = 201;
        public const int Accepted = 202;
        public const int NoContent = 204;

        // ── 4xx ─────────────────────────────────────────────────────────
        public const int BadRequest = ErrorConstant.StatusCode.BadRequest;
        public const int Unauthorized = ErrorConstant.StatusCode.Unauthorized;
        public const int Forbidden = ErrorConstant.StatusCode.Forbidden;
        public const int NotFound = ErrorConstant.StatusCode.NotFound;
        public const int Conflict = ErrorConstant.StatusCode.Conflict;
        public const int UnprocessableEntity = ErrorConstant.StatusCode.UnprocessableEntity;

        // ── 5xx ─────────────────────────────────────────────────────────
        public const int InternalServerError = ErrorConstant.StatusCode.InternalServerError;
        public const int ServiceUnavailable = ErrorConstant.StatusCode.ServiceUnavailable;

        #endregion
    }

    #endregion

    #region Result Failures

    public static class Failure
    {
        #region Result

        public static class Result
        {
            public static class MixedStatusCodes
            {
                public const string Code =
                    "result.errors.mixed_status_codes";

                public const string Message =
                    "All errors in a result must have the same HTTP status code.";
            }

            public static class FailureWithoutErrors
            {
                public const string Code =
                    "result.failure.without_errors";

                public const string Message =
                    "A failed result must contain at least one error.";
            }
        }

        #endregion

        #region Errors

        public static class Errors
        {
            public static class Empty
            {
                public const string Code =
                    "result.errors.empty";

                public const string Message =
                    "At least one error is required.";
            }

            public static class ExceedsMaxCount
            {
                public const string Code =
                    "result.errors.exceeds_max_count";

                public const string Message =
                    "A result cannot contain more than {0} errors.";
            }
        }

        #endregion

        #region Metadata

        public static class Metadata
        {
            public static class EmptyKey
            {
                public const string Code =
                    "result.metadata.empty_key";

                public const string Message =
                    "Metadata keys cannot be null, empty, or whitespace.";
            }

            public static class NullValue
            {
                public const string Code =
                    "result.metadata.null_value";

                public const string Message =
                    "Metadata value for key '{0}' cannot be null.";
            }

            public static class ExceedsMaxEntries
            {
                public const string Code =
                    "result.metadata.exceeds_max_entries";

                public const string Message =
                    "Metadata cannot contain more than {0} entries.";
            }
        }

        #endregion

        #region Value

        public static class Value
        {
            public static class Required
            {
                public const string Code =
                    "result.value.required";

                public const string Message =
                    "A successful result must have a value.";
            }

            public static class NotAllowed
            {
                public const string Code =
                    "result.value.not_allowed";

                public const string Message =
                    "A failed result cannot have a value.";
            }
        }

        #endregion

        #region Status

        public static class Status
        {
            public static class InvalidStatusCode
            {
                public const string Code =
                    "result.status.invalid_status_code";

                public const string Message =
                    "Status code must be between {0} and {1}, but was {2}.";
            }

            public static class InvalidSuccessStatus
            {
                public const string Code =
                    "result.status.invalid_success_status";

                public const string Message =
                    "A successful result must have a status code between {0} and {1}, but was {2}.";
            }

            public static class InvalidFailureStatus
            {
                public const string Code =
                    "result.status.invalid_failure_status";

                public const string Message =
                    "A failed result must have a status code between {0} and {1}, but was {2}.";
            }
        }

        #endregion

        #region Flow

        public static class Flow
        {
            public static class CallbackMissing
            {
                public const string Code = "result.flow.callback_missing";
                public const string Message = "The required callback for '{0}' was not provided.";
            }

            public static class ErrorMissing
            {
                public const string Code = "result.flow.error_missing";
                public const string Message = "The error callback for '{0}' returned null.";
            }

            public static class ResultMissing
            {
                public const string Code = "result.flow.result_missing";
                public const string Message = "The result callback for '{0}' returned null.";
            }
        }

        #endregion
    }

    #endregion
}

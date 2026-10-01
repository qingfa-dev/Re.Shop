using SharedKernel.Meta;

namespace SharedKernel.Errors;

public partial record struct Error
{
    #region Factory

    public static Error Create(string code, string message, int status)
    {
        ErrorGuard.ValidateStatus(status);
        ErrorGuard.ValidateCode(code);
        ErrorGuard.ValidateMessage(message);
        return new Error(code, message, status);
    }

    #endregion

    #region Server Errors

    public static Error Unexpected(
        string code = ErrorConstant.ErrorTypes.ServerError.Code,
        string message = ErrorConstant.ErrorTypes.ServerError.Message)
    {
        return Error.Create(code, message, ErrorConstant.ErrorTypes.ServerError.Status)
            .WithType(ErrorConstant.ErrorTypes.ServerError.Type);
    }

    public static Error ServerError(
        string code = ErrorConstant.ErrorTypes.ServerError.Code,
        string message = ErrorConstant.ErrorTypes.ServerError.Message)
    {
        return Error.Create(code, message, ErrorConstant.ErrorTypes.ServerError.Status)
            .WithType(ErrorConstant.ErrorTypes.ServerError.Type);
    }

    #endregion

    #region Client Errors

    public static Error Failure(
        string code = ErrorConstant.ErrorTypes.Failure.Code,
        string message = ErrorConstant.ErrorTypes.Failure.Message)
    {
        return Error.Create(code, message, ErrorConstant.ErrorTypes.Failure.Status)
            .WithType(ErrorConstant.ErrorTypes.Failure.Type);
    }

    public static Error BadRequest(
        string code = ErrorConstant.ErrorTypes.BadRequest.Code,
        string message = ErrorConstant.ErrorTypes.BadRequest.Message)
    {
        return Error.Create(code, message, ErrorConstant.ErrorTypes.BadRequest.Status)
            .WithType(ErrorConstant.ErrorTypes.BadRequest.Type);
    }

    public static Error Unauthorized(
        string code = ErrorConstant.ErrorTypes.Unauthorized.Code,
        string message = ErrorConstant.ErrorTypes.Unauthorized.Message)
    {
        return Error.Create(code, message, ErrorConstant.ErrorTypes.Unauthorized.Status)
            .WithType(ErrorConstant.ErrorTypes.Unauthorized.Type);
    }

    public static Error Forbidden(
        string code = ErrorConstant.ErrorTypes.Forbidden.Code,
        string message = ErrorConstant.ErrorTypes.Forbidden.Message)
    {
        return Error.Create(code, message, ErrorConstant.ErrorTypes.Forbidden.Status)
            .WithType(ErrorConstant.ErrorTypes.Forbidden.Type);
    }

    public static Error NotFound(
        string code = ErrorConstant.ErrorTypes.NotFound.Code,
        string message = ErrorConstant.ErrorTypes.NotFound.Message)
    {
        return Error.Create(code, message, ErrorConstant.ErrorTypes.NotFound.Status)
            .WithType(ErrorConstant.ErrorTypes.NotFound.Type);
    }

    public static Error AlreadyExists(
        string code = ErrorConstant.ErrorTypes.AlreadyExists.Code,
        string message = ErrorConstant.ErrorTypes.AlreadyExists.Message)
    {
        return Error.Create(code, message, ErrorConstant.ErrorTypes.AlreadyExists.Status)
            .WithType(ErrorConstant.ErrorTypes.AlreadyExists.Type);
    }

    public static Error Validation(
        string code = ErrorConstant.ErrorTypes.Validation.Code,
        string message = ErrorConstant.ErrorTypes.Validation.Message)
    {
        return Error.Create(code, message, ErrorConstant.ErrorTypes.Validation.Status)
            .WithType(ErrorConstant.ErrorTypes.Validation.Type);
    }

    public static Error BusinessRuleViolation(
        string code = ErrorConstant.ErrorTypes.BusinessRuleViolation.Code,
        string message = ErrorConstant.ErrorTypes.BusinessRuleViolation.Message)
    {
        return Error.Create(code, message, ErrorConstant.ErrorTypes.BusinessRuleViolation.Status)
            .WithType(ErrorConstant.ErrorTypes.BusinessRuleViolation.Type);
    }

    #endregion

    #region Request Validation Errors

    public static Error MissingBodyProperty(string property)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(property);

        return Error.Create(
                ErrorConstant.ErrorTypes.MissingBodyProperty.Code,
                string.Format(ErrorConstant.ErrorTypes.MissingBodyProperty.Message, property),
                ErrorConstant.ErrorTypes.MissingBodyProperty.Status)
            .WithType(ErrorConstant.ErrorTypes.MissingBodyProperty.Type);
    }

    public static Error MissingRequestHeader(string header)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(header);

        return Error.Create(
                ErrorConstant.ErrorTypes.MissingRequestHeader.Code,
                string.Format(ErrorConstant.ErrorTypes.MissingRequestHeader.Message, header),
                ErrorConstant.ErrorTypes.MissingRequestHeader.Status)
            .WithType(ErrorConstant.ErrorTypes.MissingRequestHeader.Type);
    }

    public static Error MissingRequestParameter(string parameter)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameter);

        return Error.Create(
                ErrorConstant.ErrorTypes.MissingRequestParameter.Code,
                string.Format(ErrorConstant.ErrorTypes.MissingRequestParameter.Message, parameter),
                ErrorConstant.ErrorTypes.MissingRequestParameter.Status)
            .WithType(ErrorConstant.ErrorTypes.MissingRequestParameter.Type);
    }

    public static Error InvalidBodyPropertyFormat(string property)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(property);

        return Error.Create(
                ErrorConstant.ErrorTypes.InvalidBodyPropertyFormat.Code,
                string.Format(ErrorConstant.ErrorTypes.InvalidBodyPropertyFormat.Message, property),
                ErrorConstant.ErrorTypes.InvalidBodyPropertyFormat.Status)
            .WithType(ErrorConstant.ErrorTypes.InvalidBodyPropertyFormat.Type);
    }

    public static Error InvalidBodyPropertyValue(string property)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(property);

        return Error.Create(
                ErrorConstant.ErrorTypes.InvalidBodyPropertyValue.Code,
                string.Format(ErrorConstant.ErrorTypes.InvalidBodyPropertyValue.Message, property),
                ErrorConstant.ErrorTypes.InvalidBodyPropertyValue.Status)
            .WithType(ErrorConstant.ErrorTypes.InvalidBodyPropertyValue.Type);
    }

    public static Error InvalidRequestParameterFormat(string parameter)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameter);

        return Error.Create(
                ErrorConstant.ErrorTypes.InvalidRequestParameterFormat.Code,
                string.Format(ErrorConstant.ErrorTypes.InvalidRequestParameterFormat.Message, parameter),
                ErrorConstant.ErrorTypes.InvalidRequestParameterFormat.Status)
            .WithType(ErrorConstant.ErrorTypes.InvalidRequestParameterFormat.Type);
    }

    public static Error InvalidRequestParameterValue(string parameter)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(parameter);

        return Error.Create(
                ErrorConstant.ErrorTypes.InvalidRequestParameterValue.Code,
                string.Format(ErrorConstant.ErrorTypes.InvalidRequestParameterValue.Message, parameter),
                ErrorConstant.ErrorTypes.InvalidRequestParameterValue.Status)
            .WithType(ErrorConstant.ErrorTypes.InvalidRequestParameterValue.Type);
    }

    public static Error InvalidRequestHeaderFormat(string header)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(header);

        return Error.Create(
                ErrorConstant.ErrorTypes.InvalidRequestHeaderFormat.Code,
                string.Format(ErrorConstant.ErrorTypes.InvalidRequestHeaderFormat.Message, header),
                ErrorConstant.ErrorTypes.InvalidRequestHeaderFormat.Status)
            .WithType(ErrorConstant.ErrorTypes.InvalidRequestHeaderFormat.Type);
    }

    #endregion

    #region Service Errors

    public static Error InvalidParameters(
        string message = ErrorConstant.ErrorTypes.InvalidParameters.Message,
        string code = ErrorConstant.ErrorTypes.InvalidParameters.Code)
    {
        return Error.Create(code, message, ErrorConstant.ErrorTypes.InvalidParameters.Status)
            .WithType(ErrorConstant.ErrorTypes.InvalidParameters.Type);
    }

    public static Error ServiceUnavailable(
        string code = ErrorConstant.ErrorTypes.ServiceUnavailable.Code,
        string message = ErrorConstant.ErrorTypes.ServiceUnavailable.Message)
    {
        return Error.Create(code, message, ErrorConstant.ErrorTypes.ServiceUnavailable.Status)
            .WithType(ErrorConstant.ErrorTypes.ServiceUnavailable.Type);
    }

    public static Error LicenseExpired(
        string code = ErrorConstant.ErrorTypes.LicenseExpired.Code,
        string message = ErrorConstant.ErrorTypes.LicenseExpired.Message)
    {
        return Error.Create(code, message, ErrorConstant.ErrorTypes.LicenseExpired.Status)
            .WithType(ErrorConstant.ErrorTypes.LicenseExpired.Type);
    }

    public static Error LicenseCancelled(
        string code = ErrorConstant.ErrorTypes.LicenseCancelled.Code,
        string message = ErrorConstant.ErrorTypes.LicenseCancelled.Message)
    {
        return Error.Create(code, message, ErrorConstant.ErrorTypes.LicenseCancelled.Status)
            .WithType(ErrorConstant.ErrorTypes.LicenseCancelled.Type);
    }

    #endregion
}

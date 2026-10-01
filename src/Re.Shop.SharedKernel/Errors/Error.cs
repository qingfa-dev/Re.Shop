namespace SharedKernel.Errors;

public partial record struct Error : IError
{
    #region Properties
    public string Code { get; init; } = ErrorConstant.Default.Code;
    public string Message { get; init; } = ErrorConstant.Default.Message;
    public int Status { get; init; } = ErrorConstant.Default.Status;
    public Dictionary<string, object?>? Metadata { get; set; }
    #endregion

    #region Constructors
    public Error(
        string code,
        string message,
        int status)
    {
        ErrorGuard.ValidateCode(code);
        ErrorGuard.ValidateMessage(message);
        ErrorGuard.ValidateStatus(status);

        Code = code;
        Message = message;
        Status = status;

    }
    #endregion
}

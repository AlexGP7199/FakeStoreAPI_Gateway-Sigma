using FluentValidation.Results;
using FakeStore.Gateway.Application.Commons.Enums;

namespace FakeStore.Gateway.Application.Commons.Bases;

public class BaseResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public IEnumerable<ValidationFailure>? Errores { get; set; }
    public ErrorCode ErrorCode { get; set; }

    public BaseResponse()
    {
        IsSuccess = true;
        ErrorCode = ErrorCode.None;
    }

    public BaseResponse(string message)
    {
        IsSuccess = true;
        Message = message;
        ErrorCode = ErrorCode.None;
    }
}

using FluentValidation.Results;

namespace FakeStore.Gateway.Application.Commons.Bases;

public class BaseResponseList<T> : BaseResponse<IEnumerable<T>>
{
    public int TotalRecords { get; set; }

    public BaseResponseList()
    {
        IsSuccess = true;
        Data = Enumerable.Empty<T>();
    }

    public BaseResponseList(string message) : base(message)
    {
        Data = Enumerable.Empty<T>();
    }
}

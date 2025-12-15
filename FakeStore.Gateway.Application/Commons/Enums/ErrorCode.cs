namespace FakeStore.Gateway.Application.Commons.Enums;

public enum ErrorCode
{
    None = 0,
    ValidationError = 400,
    NotFound = 404,
    Conflict = 409,
    InternalServerError = 500,
    ServiceUnavailable = 503,
    GatewayTimeout = 504
}

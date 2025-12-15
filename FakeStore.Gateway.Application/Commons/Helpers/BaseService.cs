using FakeStore.Gateway.Application.Commons.Bases;
using FakeStore.Gateway.Application.Commons.Enums;

namespace FakeStore.Gateway.Application.Commons.Helpers;

public abstract class BaseService
{
    protected async Task<T> ExecuteAsync<T>(T response, Func<Task> action) where T : class
    {
        try
        {
            await action();
        }
        catch (HttpRequestException ex)
        {
            SetErrorResponse(response, ErrorCode.ServiceUnavailable, $"Error de conexión con el servicio externo: {ex.Message}");
        }
        catch (TaskCanceledException ex)
        {
            SetErrorResponse(response, ErrorCode.GatewayTimeout, "Tiempo de espera agotado al conectar con el servicio externo");
        }
        catch (Exception ex)
        {
            SetErrorResponse(response, ErrorCode.InternalServerError, $"Error inesperado: {ex.Message}");
        }

        return response;
    }

    private void SetErrorResponse(object response, ErrorCode errorCode, string message)
    {
        var responseType = response.GetType();
        var isSuccessProperty = responseType.GetProperty("IsSuccess");
        var errorCodeProperty = responseType.GetProperty("ErrorCode");
        var messageProperty = responseType.GetProperty("Message");

        isSuccessProperty?.SetValue(response, false);
        errorCodeProperty?.SetValue(response, errorCode);
        messageProperty?.SetValue(response, message);
    }
}

namespace FakeStore.Gateway.Infrastructure.Configuration;

public class FakeStoreApiSettings
{
    public const string SectionName = "FakeStoreApi";
    
    public string BaseUrl { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
    public int CircuitBreakerThreshold { get; set; } = 5;
    public int CircuitBreakerDurationSeconds { get; set; } = 30;
}

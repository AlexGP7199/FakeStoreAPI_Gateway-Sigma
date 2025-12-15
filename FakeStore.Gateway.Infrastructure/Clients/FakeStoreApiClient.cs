using System.Net;
using System.Text;
using System.Text.Json;
using FakeStore.Gateway.Domain.Entities;

namespace FakeStore.Gateway.Infrastructure.Clients;

public class FakeStoreApiClient : IFakeStoreApiClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public FakeStoreApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task<IEnumerable<Products>> GetProductsAsync()
    {
        using var response = await _httpClient.GetAsync("products");
        response.EnsureSuccessStatusCode();
        
        await using var stream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<IEnumerable<Products>>(stream, _jsonOptions);
        
        return result ?? Enumerable.Empty<Products>();
    }

    public async Task<Products?> GetProductByIdAsync(int id)
    {
        using var response = await _httpClient.GetAsync($"products/{id}");
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        
        await using var stream = await response.Content.ReadAsStreamAsync();
        return await JsonSerializer.DeserializeAsync<Products?>(stream, _jsonOptions);
    }

    public async Task<Products> CreateProductAsync(Products product)
    {
        var json = JsonSerializer.Serialize(product, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _httpClient.PostAsync("products", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Products>(responseJson, _jsonOptions);

        return result ?? throw new InvalidOperationException("No se pudo crear el producto");
    }

    public async Task<Products> UpdateProductAsync(int id, Products product)
    {
        var json = JsonSerializer.Serialize(product, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _httpClient.PutAsync($"products/{id}", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Products>(responseJson, _jsonOptions);

        return result ?? throw new InvalidOperationException("No se pudo actualizar el producto");
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        using var response = await _httpClient.DeleteAsync($"products/{id}");
        
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return false;
        }

        response.EnsureSuccessStatusCode();
        return true;
    }
}

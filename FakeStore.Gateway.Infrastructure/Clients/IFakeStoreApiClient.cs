using FakeStore.Gateway.Domain.Entities;

namespace FakeStore.Gateway.Infrastructure.Clients;

public interface IFakeStoreApiClient
{
    Task<IEnumerable<Products>> GetProductsAsync();
    Task<Products?> GetProductByIdAsync(int id);
    Task<Products> CreateProductAsync(Products product);
    Task<Products> UpdateProductAsync(int id, Products product);
    Task<bool> DeleteProductAsync(int id);
}

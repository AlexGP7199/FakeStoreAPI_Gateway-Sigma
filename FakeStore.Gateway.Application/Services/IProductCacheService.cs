using FakeStore.Gateway.Application.DTOs;

namespace FakeStore.Gateway.Application.Services;

public interface IProductCacheService
{
    IEnumerable<ProductDto> GetCachedProducts();
    void AddProduct(ProductDto product);
    void UpdateProduct(ProductDto product);
    void RemoveProduct(int id);
    ProductDto? GetProductById(int id);
    bool ExistsInCache(int id);
}

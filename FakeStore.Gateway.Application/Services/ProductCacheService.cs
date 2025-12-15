using System.Collections.Concurrent;
using FakeStore.Gateway.Application.DTOs;

namespace FakeStore.Gateway.Application.Services;

public class ProductCacheService : IProductCacheService
{
    private readonly ConcurrentDictionary<int, ProductDto> _createdProducts = new();
    private readonly ConcurrentDictionary<int, ProductDto> _updatedProducts = new();
    private readonly HashSet<int> _deletedProductIds = new();
    private readonly object _lock = new();

    public IEnumerable<ProductDto> GetCachedProducts()
    {
        lock (_lock)
        {
            return _createdProducts.Values.ToList();
        }
    }

    public void AddProduct(ProductDto product)
    {
        _createdProducts.TryAdd(product.Id, product);
    }

    public void UpdateProduct(ProductDto product)
    {
        // Si el producto está en _createdProducts, actualizarlo ahí también
        if (_createdProducts.ContainsKey(product.Id))
        {
            _createdProducts[product.Id] = product;
        }
        
        // Siempre guardar en _updatedProducts
        _updatedProducts.AddOrUpdate(product.Id, product, (key, existing) => product);
    }

    public void RemoveProduct(int id)
    {
        lock (_lock)
        {
            _deletedProductIds.Add(id);
            _createdProducts.TryRemove(id, out _);
            _updatedProducts.TryRemove(id, out _);
        }
    }

    public ProductDto? GetProductById(int id)
    {
        // Primero buscar en productos ACTUALIZADOS (prioridad)
        if (_updatedProducts.TryGetValue(id, out var updated))
        {
            return updated;
        }

        // Luego buscar en productos CREADOS
        if (_createdProducts.TryGetValue(id, out var created))
        {
            return created;
        }

        return null;
    }

    public bool ExistsInCache(int id)
    {
        return _createdProducts.ContainsKey(id) || _updatedProducts.ContainsKey(id);
    }

    public bool IsDeleted(int id)
    {
        lock (_lock)
        {
            return _deletedProductIds.Contains(id);
        }
    }

    public ProductDto? GetUpdatedVersion(int id)
    {
        return _updatedProducts.TryGetValue(id, out var product) ? product : null;
    }
}

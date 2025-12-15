using FakeStore.Gateway.Application.Commons.Bases;
using FakeStore.Gateway.Application.Commons.Enums;
using FakeStore.Gateway.Application.Commons.Helpers;
using FakeStore.Gateway.Application.DTOs;
using FakeStore.Gateway.Application.Interfaces;
using FakeStore.Gateway.Application.Mappers;
using FakeStore.Gateway.Infrastructure.Clients;
using FluentValidation;

namespace FakeStore.Gateway.Application.Services;

public class ProductsService : BaseService, IProductsService
{
    private readonly IFakeStoreApiClient _client;
    private readonly IProductCacheService _cache;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;

    public ProductsService(
        IFakeStoreApiClient client, 
        IProductCacheService cache,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
        _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
    }

    public async Task<BaseResponseList<ProductDto>> GetAllAsync()
    {
        var response = new BaseResponseList<ProductDto>();

        return await ExecuteAsync(response, async () =>
        {
            var productsFromApi = await _client.GetProductsAsync();
            var dtos = productsFromApi.ToDto().ToList();

            var filteredProducts = new List<ProductDto>();
            foreach (var product in dtos)
            {
                var cacheService = _cache as ProductCacheService;
                
                if (cacheService?.IsDeleted(product.Id) == true)
                {
                    continue;
                }

                var updatedVersion = cacheService?.GetUpdatedVersion(product.Id);
                filteredProducts.Add(updatedVersion ?? product);
            }

            var cachedProducts = _cache.GetCachedProducts();
            filteredProducts.AddRange(cachedProducts);

            response.IsSuccess = true;
            response.Data = filteredProducts;
            response.TotalRecords = filteredProducts.Count;
            response.Message = "Productos obtenidos exitosamente";
        });
    }

    public async Task<BaseResponse<ProductDto>> GetByIdAsync(int id)
    {
        var response = new BaseResponse<ProductDto>();

        return await ExecuteAsync(response, async () =>
        {
            var cacheService = _cache as ProductCacheService;
            
            if (cacheService?.IsDeleted(id) == true)
            {
                response.IsSuccess = false;
                response.ErrorCode = ErrorCode.NotFound;
                response.Message = $"El producto con ID {id} no existe";
                return;
            }

            var cachedProduct = _cache.GetProductById(id);
            if (cachedProduct != null)
            {
                response.IsSuccess = true;
                response.Data = cachedProduct;
                response.Message = "Producto obtenido exitosamente";
                return;
            }

            var product = await _client.GetProductByIdAsync(id);
            
            if (product == null)
            {
                response.IsSuccess = false;
                response.ErrorCode = ErrorCode.NotFound;
                response.Message = $"El producto con ID {id} no existe";
                return;
            }

            var updatedVersion = cacheService?.GetUpdatedVersion(id);
            response.IsSuccess = true;
            response.Data = updatedVersion ?? product.ToDto();
            response.Message = "Producto obtenido exitosamente";
        });
    }

    public async Task<BaseResponse<ProductDto>> CreateAsync(CreateProductDto createProductDto)
    {
        var response = new BaseResponse<ProductDto>();

        return await ExecuteAsync(response, async () =>
        {
            var validationResult = await _createValidator.ValidateAsync(createProductDto);

            if (!validationResult.IsValid)
            {
                response.IsSuccess = false;
                response.ErrorCode = ErrorCode.ValidationError;
                response.Message = "Errores de validación";
                response.Errores = validationResult.Errors;
                return;
            }

            var entity = createProductDto.ToEntity();
            var createdProduct = await _client.CreateProductAsync(entity);
            var dto = createdProduct.ToDto();
            
            _cache.AddProduct(dto);
            
            response.IsSuccess = true;
            response.Data = dto;
            response.Message = "Producto creado exitosamente";
        });
    }

    public async Task<BaseResponse<ProductDto>> UpdateAsync(int id, UpdateProductDto updateProductDto)
    {
        var response = new BaseResponse<ProductDto>();

        return await ExecuteAsync(response, async () =>
        {
            var validationResult = await _updateValidator.ValidateAsync(updateProductDto);

            if (!validationResult.IsValid)
            {
                response.IsSuccess = false;
                response.ErrorCode = ErrorCode.ValidationError;
                response.Message = "Errores de validación";
                response.Errores = validationResult.Errors;
                return;
            }

            var existingProduct = await GetByIdAsync(id);
            if (!existingProduct.IsSuccess || existingProduct.Data == null)
            {
                response.IsSuccess = false;
                response.ErrorCode = ErrorCode.NotFound;
                response.Message = $"El producto con ID {id} no existe";
                return;
            }

            var entity = updateProductDto.ToEntity(id);
            var updatedProduct = await _client.UpdateProductAsync(id, entity);
            var dto = updatedProduct.ToDto();
            
            _cache.UpdateProduct(dto);
            
            response.IsSuccess = true;
            response.Data = dto;
            response.Message = "Producto actualizado exitosamente";
        });
    }

    public async Task<BaseResponse<bool>> DeleteAsync(int id)
    {
        var response = new BaseResponse<bool>();

        return await ExecuteAsync(response, async () =>
        {
            var deleted = await _client.DeleteProductAsync(id);
            
            if (!deleted)
            {
                response.IsSuccess = false;
                response.ErrorCode = ErrorCode.NotFound;
                response.Data = false;
                response.Message = $"El producto con ID {id} no existe";
                return;
            }

            _cache.RemoveProduct(id);
            
            response.IsSuccess = true;
            response.Data = true;
            response.Message = "Producto eliminado exitosamente";
        });
    }
}

using FakeStore.Gateway.Application.Commons.Bases;
using FakeStore.Gateway.Application.DTOs;

namespace FakeStore.Gateway.Application.Interfaces;

public interface IProductsService
{
    Task<BaseResponseList<ProductDto>> GetAllAsync();
    Task<BaseResponse<ProductDto>> GetByIdAsync(int id);
    Task<BaseResponse<ProductDto>> CreateAsync(CreateProductDto createProductDto);
    Task<BaseResponse<ProductDto>> UpdateAsync(int id, UpdateProductDto updateProductDto);
    Task<BaseResponse<bool>> DeleteAsync(int id);
}

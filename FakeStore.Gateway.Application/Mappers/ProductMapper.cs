using FakeStore.Gateway.Application.DTOs;
using FakeStore.Gateway.Domain.Entities;

namespace FakeStore.Gateway.Application.Mappers;

public static class ProductMapper
{
    public static ProductDto ToDto(this Products entity)
    {
        return new ProductDto
        {
            Id = entity.Id,
            Title = entity.Title,
            Price = entity.Price,
            Description = entity.Description,
            Category = entity.Category,
            Image = entity.Image
        };
    }

    public static IEnumerable<ProductDto> ToDto(this IEnumerable<Products> entities)
    {
        return entities.Select(e => e.ToDto());
    }

    public static Products ToEntity(this ProductDto dto)
    {
        return new Products
        {
            Id = dto.Id,
            Title = dto.Title,
            Price = dto.Price,
            Description = dto.Description,
            Category = dto.Category,
            Image = dto.Image
        };
    }

    public static Products ToEntity(this CreateProductDto dto)
    {
        return new Products
        {
            Title = dto.Title,
            Price = dto.Price,
            Description = dto.Description,
            Category = dto.Category,
            Image = dto.Image
        };
    }

    public static Products ToEntity(this UpdateProductDto dto, int id)
    {
        return new Products
        {
            Id = id,
            Title = dto.Title,
            Price = dto.Price,
            Description = dto.Description,
            Category = dto.Category,
            Image = dto.Image
        };
    }
}

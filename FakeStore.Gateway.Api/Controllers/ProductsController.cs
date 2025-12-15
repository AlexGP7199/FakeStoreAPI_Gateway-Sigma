using FakeStore.Gateway.Application.Commons.Enums;
using FakeStore.Gateway.Application.DTOs;
using FakeStore.Gateway.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FakeStore.Gateway.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductsService _productsService;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductsService productsService, ILogger<ProductsController> logger)
    {
        _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Obtiene todos los productos
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var response = await _productsService.GetAllAsync();
            
            if (!response.IsSuccess)
            {
                return StatusCode(GetHttpStatusCode(response.ErrorCode), response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener todos los productos");
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            { 
                isSuccess = false, 
                message = "Error interno del servidor",
                errorCode = ErrorCode.InternalServerError
            });
        }
    }

    /// <summary>
    /// Obtiene un producto por su ID
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var response = await _productsService.GetByIdAsync(id);
            
            if (!response.IsSuccess)
            {
                return StatusCode(GetHttpStatusCode(response.ErrorCode), response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al obtener el producto {ProductId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            { 
                isSuccess = false, 
                message = "Error interno del servidor",
                errorCode = ErrorCode.InternalServerError
            });
        }
    }

    /// <summary>
    /// Crea un nuevo producto
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
    public async Task<IActionResult> Create([FromBody] CreateProductDto createProductDto)
    {
        try
        {
            var response = await _productsService.CreateAsync(createProductDto);
            
            if (!response.IsSuccess)
            {
                return StatusCode(GetHttpStatusCode(response.ErrorCode), response);
            }

            return CreatedAtAction(
                nameof(GetById), 
                new { id = response.Data!.Id }, 
                response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al crear el producto");
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            { 
                isSuccess = false, 
                message = "Error interno del servidor",
                errorCode = ErrorCode.InternalServerError
            });
        }
    }

    /// <summary>
    /// Actualiza un producto existente
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto updateProductDto)
    {
        try
        {
            var response = await _productsService.UpdateAsync(id, updateProductDto);
            
            if (!response.IsSuccess)
            {
                return StatusCode(GetHttpStatusCode(response.ErrorCode), response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al actualizar el producto {ProductId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            { 
                isSuccess = false, 
                message = "Error interno del servidor",
                errorCode = ErrorCode.InternalServerError
            });
        }
    }

    /// <summary>
    /// Elimina un producto por su ID
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var response = await _productsService.DeleteAsync(id);
            
            if (!response.IsSuccess)
            {
                return StatusCode(GetHttpStatusCode(response.ErrorCode), response);
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado al eliminar el producto {ProductId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new 
            { 
                isSuccess = false, 
                message = "Error interno del servidor",
                errorCode = ErrorCode.InternalServerError
            });
        }
    }

    private int GetHttpStatusCode(ErrorCode errorCode)
    {
        return errorCode switch
        {
            ErrorCode.None => StatusCodes.Status200OK,
            ErrorCode.ValidationError => StatusCodes.Status400BadRequest,
            ErrorCode.NotFound => StatusCodes.Status404NotFound,
            ErrorCode.Conflict => StatusCodes.Status409Conflict,
            ErrorCode.InternalServerError => StatusCodes.Status500InternalServerError,
            ErrorCode.ServiceUnavailable => StatusCodes.Status503ServiceUnavailable,
            ErrorCode.GatewayTimeout => StatusCodes.Status504GatewayTimeout,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}

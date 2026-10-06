namespace AspNetEmptyApp.Feature.Inventory.Controller.v1;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Asp.Versioning;
using AspNetEmptyApp.Feature.Auth.Authorization;
using AspNetEmptyApp.Feature.Inventory.Command;
using AspNetEmptyApp.Feature.Inventory.DTOs;
using AspNetEmptyApp.Feature.Inventory.Handler;
using AspNetEmptyApp.Feature.Inventory.Query;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/inventory/productions")]
public class ProductionController : ControllerBase
{
    private readonly CreateProductionHandler _createProductionHandler;
    private readonly GetProductionsHandler _getProductionsHandler;
    private readonly IValidator<CreateProductionDto> _validator;

    public ProductionController(
        CreateProductionHandler createProductionHandler,
        GetProductionsHandler getProductionsHandler,
        IValidator<CreateProductionDto> validator)
    {
        _createProductionHandler = createProductionHandler;
        _getProductionsHandler = getProductionsHandler;
        _validator = validator;
    }

    [HttpPost]
    [HasPermission("product.create")]
    public async Task<IActionResult> Create([FromBody] CreateProductionDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(new { message = "Invalid user credentials" });
        }

        var command = new CreateProductionCommand(
            dto.Name,
            dto.Description,
            dto.Price,
            dto.Quantity,
            dto.CategoryId,
            userId
        );

        var result = await _createProductionHandler.HandleAsync(command);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [HasPermission("product.read")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _getProductionsHandler.HandleAsync(new GetProductionsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("product.read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _getProductionsHandler.HandleByIdAsync(new GetProductionByIdQuery(id));
        if (result == null)
        {
            return NotFound(new { message = "Product not found" });
        }
        return Ok(result);
    }
}

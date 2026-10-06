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
[Route("api/v{version:apiVersion}/inventory/categories")]
public class CategoryController : ControllerBase
{
    private readonly CreateCategoryHandler _createCategoryHandler;
    private readonly GetCategoriesHandler _getCategoriesHandler;
    private readonly IValidator<CreateCategoryDto> _validator;

    public CategoryController(
        CreateCategoryHandler createCategoryHandler,
        GetCategoriesHandler getCategoriesHandler,
        IValidator<CreateCategoryDto> validator)
    {
        _createCategoryHandler = createCategoryHandler;
        _getCategoriesHandler = getCategoriesHandler;
        _validator = validator;
    }

    [HttpPost]
    [HasPermission("category.create")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
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

        var command = new CreateCategoryCommand(dto.Name, dto.Description, userId);
        var result = await _createCategoryHandler.HandleAsync(command);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    [HasPermission("category.read")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _getCategoriesHandler.HandleAsync(new GetCategoriesQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [HasPermission("category.read")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _getCategoriesHandler.HandleByIdAsync(new GetCategoryByIdQuery(id));
        if (result == null)
        {
            return NotFound(new { message = "Category not found" });
        }
        return Ok(result);
    }
}

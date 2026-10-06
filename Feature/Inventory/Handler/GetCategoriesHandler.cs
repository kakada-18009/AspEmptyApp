namespace AspNetEmptyApp.Feature.Inventory.Handler;

using AspNetEmptyApp.Data;
using AspNetEmptyApp.Feature.Inventory.DTOs;
using AspNetEmptyApp.Feature.Inventory.Query;
using Dapper;

public class GetCategoriesHandler
{
    private readonly IDapperContext _dapper;

    public GetCategoriesHandler(IDapperContext dapper)
    {
        _dapper = dapper;
    }

    public async Task<IEnumerable<CategoryResponseDto>> HandleAsync(GetCategoriesQuery query)
    {
        using var connection = _dapper.CreateConnection();
        const string sql = """
            SELECT 
                Id, 
                Name, 
                Description, 
                CreatedBy, 
                CreatedAt 
            FROM Categories 
            ORDER BY Name ASC
            """;

        return await connection.QueryAsync<CategoryResponseDto>(sql);
    }

    public async Task<CategoryResponseDto?> HandleByIdAsync(GetCategoryByIdQuery query)
    {
        using var connection = _dapper.CreateConnection();
        const string sql = """
            SELECT 
                Id, 
                Name, 
                Description, 
                CreatedBy, 
                CreatedAt 
            FROM Categories 
            WHERE Id = @Id COLLATE NOCASE
            """;

        return await connection.QuerySingleOrDefaultAsync<CategoryResponseDto>(sql, new { Id = query.Id.ToString() });
    }
}

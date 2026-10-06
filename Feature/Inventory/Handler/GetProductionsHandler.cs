namespace AspNetEmptyApp.Feature.Inventory.Handler;

using AspNetEmptyApp.Data;
using AspNetEmptyApp.Feature.Inventory.DTOs;
using AspNetEmptyApp.Feature.Inventory.Query;
using Dapper;

public class GetProductionsHandler
{
    private readonly IDapperContext _dapper;

    public GetProductionsHandler(IDapperContext dapper)
    {
        _dapper = dapper;
    }

    public async Task<IEnumerable<ProductionResponseDto>> HandleAsync(GetProductionsQuery query)
    {
        using var connection = _dapper.CreateConnection();
        const string sql = """
            SELECT 
                Id, 
                Name, 
                Description, 
                Price, 
                Quantity, 
                CategoryId, 
                CreatedBy, 
                CreatedAt 
            FROM Productions 
            ORDER BY CreatedAt DESC
            """;

        return await connection.QueryAsync<ProductionResponseDto>(sql);
    }

    public async Task<ProductionResponseDto?> HandleByIdAsync(GetProductionByIdQuery query)
    {
        using var connection = _dapper.CreateConnection();
        const string sql = """
            SELECT 
                Id, 
                Name, 
                Description, 
                Price, 
                Quantity, 
                CategoryId, 
                CreatedBy, 
                CreatedAt 
            FROM Productions 
            WHERE Id = @Id COLLATE NOCASE
            """;

        return await connection.QuerySingleOrDefaultAsync<ProductionResponseDto>(sql, new { Id = query.Id.ToString() });
    }
}

namespace AspNetEmptyApp.Feature.Auth.Handler;

using AspNetEmptyApp.Data;
using AspNetEmptyApp.Feature.Auth.DTOs;
using AspNetEmptyApp.Feature.Auth.Query;
using Dapper;

public class GetUserProfileHandler
{
    private readonly IDapperContext _dapper;

    public GetUserProfileHandler(IDapperContext dapper)
    {
        _dapper = dapper;
    }

    public async Task<UserProfileDto?> HandleAsync(GetUserProfileQuery query)
    {
        using var connection = _dapper.CreateConnection();
        const string sql = """
            SELECT 
                Id, 
                UserName, 
                Email, 
                PhoneNumber, 
                EmailConfirmed 
            FROM AspNetUsers 
            WHERE Id = @UserId COLLATE NOCASE
            """;

        return await connection.QuerySingleOrDefaultAsync<UserProfileDto>(sql, new { UserId = query.UserId.ToString() });
    }
}

using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using StackExchange.Redis;

[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    private readonly MySqlConnection _mysqlConnection;
    private readonly IConnectionMultiplexer _redis;

    public SampleController(MySqlConnection mysqlConnection, IConnectionMultiplexer redis)
    {
        _mysqlConnection = mysqlConnection;
        _redis = redis;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Message = "Backend API is working!" });
    }

    [HttpGet("mysql")]
    public async Task<IActionResult> GetFromMySql()
    {
        await _mysqlConnection.OpenAsync();
        var command = new MySqlCommand("SELECT NOW();", _mysqlConnection);
        var result = await command.ExecuteScalarAsync();
        await _mysqlConnection.CloseAsync();

        return Ok(new { MySqlServerTime = result?.ToString() });
    }

    [HttpGet("redis")]
    public async Task<IActionResult> GetFromRedis()
    {
        if (!_redis.IsConnected)
       {
         return StatusCode(500, "Redis is not connected.");
       }

        var db = _redis.GetDatabase();
        await db.StringSetAsync("test-key", "Hello from Redis!", TimeSpan.FromMinutes(5));
        var value = await db.StringGetAsync("test-key");
        string stringValue = value.HasValue ? value.ToString() : null;
        return Ok(new { RedisValue = stringValue });
    }

    [HttpGet("user/{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        string cacheKey = $"user:{id}";

        // Try to get from Redis cache
        var _redisdb = _redis.GetDatabase();
        var cachedUser = await _redisdb.StringGetAsync(cacheKey);
        if (cachedUser.HasValue)
        {
            return Ok(new { source = "redis", data = cachedUser.ToString() });
        }

        // If not in Redis, get from MySQL
        await _mysqlConnection.OpenAsync();
        var command = _mysqlConnection.CreateCommand();
        command.CommandText = "SELECT name FROM users WHERE id = @id";
        command.Parameters.AddWithValue("@id", id);

        var userName = await command.ExecuteScalarAsync();
        await _mysqlConnection.CloseAsync();
        if (userName == null)
        {
            return NotFound();
        }

        // Save into Redis
        await _redisdb.StringSetAsync(cacheKey, userName.ToString(), TimeSpan.FromMinutes(5));

        return Ok(new { source = "mysql", data = userName.ToString() });
    }

}

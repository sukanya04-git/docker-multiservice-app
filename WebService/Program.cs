using MySql.Data.MySqlClient;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

var mysqlConnStr = builder.Configuration.GetConnectionString("DefaultConnection");
var redisConnStr = builder.Configuration.GetConnectionString("RedisConnection");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddTransient<MySqlConnection>(_ => new MySqlConnection(mysqlConnStr));
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConnStr));

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();
app.Run();

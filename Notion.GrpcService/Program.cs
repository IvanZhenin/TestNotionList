using Microsoft.EntityFrameworkCore;
using Notion.BaseModule.Interfaces;
using Notion.BusinessLogic.Services;
using Notion.DataAccess.Data;
using Notion.GrpcServer.Services;
using StackExchange.Redis;

namespace Notion.GrpcService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            // Add services to the container.
            builder.Services.AddGrpc();

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<INotionService, UserNotionService>();

            builder.Services.AddDbContext<NotionDb>(options =>
            {
                options.UseNpgsql(config["ConnectionStrings:DbConnect"]);
            });

            builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
            {
                var configuration = config["Redis:Config"];
                return ConnectionMultiplexer.Connect(configuration ?? throw new Exception());
            });

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config["Redis:Config"];
                options.InstanceName = config["Redis:Instance"];
            });

            builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
            builder.Services.AddSingleton<ICacheService, RedisCacheService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<INotionService, UserNotionService>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<NotionDb>();
                context.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.
            app.MapGrpcService<UserManagerService>();
            app.MapGrpcService<UserNotionManagerService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}
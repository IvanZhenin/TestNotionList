using Grpc.Net.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Notion.Protos;

namespace Notion.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            var grpcChannel = GrpcChannel.ForAddress("https://localhost:7226");
            builder.Services.AddSingleton(new UserManager.UserManagerClient(grpcChannel));
            builder.Services.AddSingleton(new UserNotionManager.UserNotionManagerClient(grpcChannel));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Authentication/Login";
                    options.AccessDeniedPath = "/Authentication/Login";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.MapGet("/", async context =>
            {
                context.Response.Redirect("/Authentication/Login");
            });

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
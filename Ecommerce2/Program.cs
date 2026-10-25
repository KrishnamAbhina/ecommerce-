using ecommerce_application.DATA;
using ecommerce_application;
using ecommerce_application.SERVICES;
using DAL.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Microsoft.OpenApi;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.File(
        path: "logs/lg-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} - {Message:lj}{NewLine}{Exception}",
        fileSizeLimitBytes: 104857600,
        retainedFileCountLimit: 30)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Ecommerce2")
    .CreateLogger();

try
{
    Log.Information("Application starting...");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

// Add services to the container.

    builder.Services.AddControllers();
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Configure JWT Authentication
    var jwtKey = builder.Configuration["Jwt:Key"] ?? "0wq4rpookie@0ebteabhinatewy#te0qup9rwqp7%hello9wqry&wq";
    var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Ecommerce2";
    var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "Ecommerce2Users";

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            };

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    Log.Information("OnTokenValidated event triggered");
                    var claimsIdentity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
                    if (claimsIdentity != null)
                    {
                        var allClaims = claimsIdentity.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
                        Log.Information($"All claims: {string.Join(", ", allClaims)}");

                        var roleClaim = claimsIdentity.FindFirst("role");
                        if (roleClaim != null)
                        {
                            Log.Information($"Found role claim: {roleClaim.Value}");
                            claimsIdentity.AddClaim(new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, roleClaim.Value));
                            Log.Information($"Added standard role claim: {roleClaim.Value}");
                        }
                        else
                        {
                            Log.Warning("No 'role' claim found in token");
                        }
                    }
                    else
                    {
                        Log.Warning("ClaimsIdentity is null");
                    }
                    return Task.CompletedTask;
                }
            };
        });



    builder.Services.AddAuthorization();

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<EcommerceDbContext>(options =>
        options.UseSqlServer(connectionString));

    // Register DAO
    builder.Services.AddScoped<CategoryDAO>();
    builder.Services.AddScoped<ProductDAO>();
    builder.Services.AddScoped<InventoryDAO>();
    builder.Services.AddScoped<AddressModelDAO>();
    builder.Services.AddScoped<OrdersDAO>();
    builder.Services.AddScoped<CartDAO>();
    builder.Services.AddScoped<AuthDAO>();

    // Register BLL
    builder.Services.AddScoped<IProductService, ProductService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IInventoryService, InventoryService>();
    builder.Services.AddScoped<IAddressService, AddressService>();
    builder.Services.AddScoped<IOrdersService, OrdersService>();
    builder.Services.AddScoped<ICartService, CartService>();
    builder.Services.AddScoped<IAuthService, AuthService>();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("ReactPolicy", policy =>
        {
            policy.WithOrigins("http://localhost:3002")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    var app = builder.Build();

    app.UseCors("ReactPolicy");

    app.MapSwagger();
    app.MapSwaggerUI();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("Application started successfully");
    app.Run();
}


catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

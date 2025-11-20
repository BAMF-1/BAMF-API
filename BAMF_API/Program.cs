/*
using Azure.AI.OpenAI;
using BAMF_API.Data;
using BAMF_API.Exceptions;
using BAMF_API.Interfaces;
using BAMF_API.Interfaces.AdminInterfaces;
using BAMF_API.Interfaces.AuthInterfaces;
using BAMF_API.Interfaces.InventoryInterfaces;
using BAMF_API.Interfaces.OrderInterfaces;
using BAMF_API.Interfaces.ProductInterfaces;
using BAMF_API.Interfaces.ReviewInterfaces;
using BAMF_API.Interfaces.UserInterfaces;
using BAMF_API.Repositories;
using BAMF_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;

namespace BAMF_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Dependency Injection for Repositories and Services
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IInventoryService, InventoryService>();

            builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
            builder.Services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
            builder.Services.AddScoped<IVariantRepository, VariantRepository>();
            builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();
            builder.Services.AddScoped<IAdminService, AdminService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Swagger with JWT Auth
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "BAMF API",
                    Version = "v1",
                    Description = "Backend API for BAMF with Auth, Reviews, Orders, and Users"
                });

                //  Enable JWT Auth support
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Enter 'Bearer' followed by your JWT token. Example: Bearer eyJhbGciOiJIUzI1...",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                };

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "bearer"
                            }
                        },
                        new string[] {}
                    }
                };

                options.AddSecurityDefinition("bearer", securityScheme);
                options.AddSecurityRequirement(securityRequirement);
            });

            // JWT configuration
            var jwt = builder.Configuration.GetSection("Jwt");
            builder.Services.AddAuthentication(options =>
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
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
                };
            });

            builder.Services.AddAuthorization();

            string endpoint = builder.Configuration["AZURE_OPENAI_ENDPOINT"];
            string apiKey = builder.Configuration["AZURE_OPENAI_KEY"];

            builder.Services.AddSingleton<AzureOpenAIClient>(sp =>
            {
                return new AzureOpenAIClient(
                    new Uri(endpoint),
                    new System.ClientModel.ApiKeyCredential(apiKey)
                );
            });


            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins",
                    policy =>
                    {
                        policy.WithOrigins(
                            "http://localhost:3000",
                            "https://localhost:7039"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowSpecificOrigins");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            // Global exception handler: map NotFoundException -> 404, InsufficientStockException -> 409
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    var feature = context.Features.Get<IExceptionHandlerFeature>();
                    var ex = feature?.Error;

                    if (ex is NotFoundException nfe)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        context.Response.ContentType = "application/json";
                        var payload = new
                        {
                            error = "NotFound",
                            message = nfe.Message,
                            resource = nfe.Resource,
                            identifier = nfe.Identifier
                        };
                        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
                        return;
                    }

                    if (ex is InsufficientStockException ise)
                    {
                        // Return 409 Conflict for out-of-stock situations (recommended)
                        context.Response.StatusCode = StatusCodes.Status409Conflict;
                        context.Response.ContentType = "application/json";
                        var payload = new
                        {
                            error = "InsufficientStock",
                            message = ise.Message,
                            skuOrVariant = ise.SkuOrVariantId,
                            requested = ise.Requested,
                            available = ise.Available
                        };
                        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
                        return;
                    }

                    // fallback: rethrow or handle as 500
                    throw ex!;
                });
            });

            // Seed database
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
                SeedData.EnsureSeeded(db);
            }

            app.Run();

        }
    }
}
*/

using Azure.AI.OpenAI;
using BAMF_API.Data;
using BAMF_API.Exceptions;
using BAMF_API.Interfaces;
using BAMF_API.Interfaces.AdminInterfaces;
using BAMF_API.Interfaces.AuthInterfaces;
using BAMF_API.Interfaces.InventoryInterfaces;
using BAMF_API.Interfaces.OrderInterfaces;
using BAMF_API.Interfaces.ProductInterfaces;
using BAMF_API.Interfaces.ReviewInterfaces;
using BAMF_API.Interfaces.UserInterfaces;
using BAMF_API.Repositories;
using BAMF_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;

namespace BAMF_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Log configuration for debugging
            Console.WriteLine("=== Starting BAMF API ===");
            Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");

            try
            {
                // Add DbContext
                var connString = builder.Configuration.GetConnectionString("DefaultConnection");

                builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connString));

                // Dependency Injection for Repositories and Services
                builder.Services.AddScoped<IOrderRepository, OrderRepository>();
                builder.Services.AddScoped<IOrderService, OrderService>();
                builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
                builder.Services.AddScoped<IReviewService, ReviewService>();
                builder.Services.AddScoped<IInventoryService, InventoryService>();

                builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
                builder.Services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
                builder.Services.AddScoped<IVariantRepository, VariantRepository>();
                builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
                builder.Services.AddScoped<IAuthRepository, AuthRepository>();
                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<IUserRepository, UserRepository>();
                builder.Services.AddScoped<IUserService, UserService>();
                builder.Services.AddScoped<IAdminRepository, AdminRepository>();
                builder.Services.AddScoped<IAdminService, AdminService>();

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();

                // Swagger with JWT Auth
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo
                    {
                        Title = "BAMF API",
                        Version = "v1",
                        Description = "Backend API for BAMF with Auth, Reviews, Orders, and Users"
                    });

                    var securityScheme = new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Description = "Enter 'Bearer' followed by your JWT token",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT"
                    };

                    var securityRequirement = new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "bearer"
                                }
                            },
                            new string[] {}
                        }
                    };

                    options.AddSecurityDefinition("bearer", securityScheme);
                    options.AddSecurityRequirement(securityRequirement);
                });

                // JWT configuration
                var jwt = builder.Configuration.GetSection("Jwt");
                var jwtKey = jwt["Key"];
                var jwtIssuer = jwt["Issuer"];
                var jwtAudience = jwt["Audience"];

                if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience))
                {
                    throw new InvalidOperationException("JWT configuration is missing!");
                }

                builder.Services.AddAuthentication(options =>
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
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

                builder.Services.AddAuthorization();

                // Azure OpenAI - Optional
                //string? endpoint = builder.Configuration["AZURE_OPENAI_ENDPOINT"];
                //string? apiKey = builder.Configuration["AZURE_OPENAI_KEY"];

                //if (!string.IsNullOrEmpty(endpoint) && !string.IsNullOrEmpty(apiKey))
                //{
                //    builder.Services.AddSingleton<AzureOpenAIClient>(sp =>
                //    {
                //        return new AzureOpenAIClient(
                //            new Uri(endpoint),
                //            new System.ClientModel.ApiKeyCredential(apiKey)
                //        );
                //    });
                //    Console.WriteLine("Azure OpenAI configured");
                //}
                //else
                //{
                //    Console.WriteLine("Azure OpenAI not configured - skipping");
                //}

                // Add CORS
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowSpecificOrigins",
                        policy =>
                        {
                            policy.WithOrigins(
                                "http://localhost:3000",
                                "https://localhost:7039",
                                "https://bamf-gear-h5a3f7dvc9ffbxhr.germanywestcentral-01.azurewebsites.net"
                            )
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                        });
                });

                var app = builder.Build();

                Console.WriteLine("=== App Built Successfully ===");

                // Enable Swagger
                app.UseSwagger();
                app.UseSwaggerUI();

                app.UseHttpsRedirection();
                app.UseCors("AllowSpecificOrigins");
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();

                // Health check endpoint
                app.MapGet("/health", () => Results.Ok(new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    environment = app.Environment.EnvironmentName
                }));

                app.MapGet("/", () => Results.Ok(new
                {
                    message = "BAMF API is running",
                    endpoints = new[] { "/api/products" }
                }));

                app.MapPost("/admin/reset-db", (IServiceProvider sp) =>
                {
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    db.Database.EnsureDeleted();
                    db.Database.Migrate();

                    return Results.Ok("Database was reset");
                });

                app.MapPost("/admin/seed-db", (IServiceProvider sp) =>
                {
                    using var scope = sp.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    SeedData.EnsureSeeded(db);
                    return Results.Ok("Database was seeded");
                });


                // Global exception handler
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async context =>
                    {
                        var feature = context.Features.Get<IExceptionHandlerFeature>();
                        var ex = feature?.Error;

                        if (ex is NotFoundException nfe)
                        {
                            context.Response.StatusCode = StatusCodes.Status404NotFound;
                            context.Response.ContentType = "application/json";
                            var payload = new
                            {
                                error = "NotFound",
                                message = nfe.Message,
                                resource = nfe.Resource,
                                identifier = nfe.Identifier
                            };
                            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
                            return;
                        }

                        if (ex is InsufficientStockException ise)
                        {
                            context.Response.StatusCode = StatusCodes.Status409Conflict;
                            context.Response.ContentType = "application/json";
                            var payload = new
                            {
                                error = "InsufficientStock",
                                message = ise.Message,
                                skuOrVariant = ise.SkuOrVariantId,
                                requested = ise.Requested,
                                available = ise.Available
                            };
                            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
                            return;
                        }

                        throw ex!;
                    });
                });

                Console.WriteLine("=== Starting App ===");
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL ERROR: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
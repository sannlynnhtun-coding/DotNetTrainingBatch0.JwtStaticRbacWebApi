using System.Text;
using DotNetTrainingBatch0.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using DotNetTrainingBatch0.JwtWebApi.Features.Auth;
using DotNetTrainingBatch0.JwtWebApi.Features.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "JwtWebApi", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProductService>();

// Register AppDbContext with In-Memory Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("JwtWebApiDb"));

var jwtKey = builder.Configuration["Jwt:Key"]!;

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
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey)
        )
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProductView", policy =>
        policy.RequireClaim("permission", "Product.View"));

    options.AddPolicy("ProductCreate", policy =>
        policy.RequireClaim("permission", "Product.Create"));

    options.AddPolicy("ProductUpdate", policy =>
        policy.RequireClaim("permission", "Product.Update"));

    options.AddPolicy("ProductDelete", policy =>
        policy.RequireClaim("permission", "Product.Delete"));
});

var app = builder.Build();

// Seed initial data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    if (!context.TblUsers.Any())
    {
        context.TblUsers.AddRange(
            new DotNetTrainingBatch0.Database.AppDbContextModels.AppUser
            {
                Id = 1,
                Username = "admin",
                Password = "123",
                Role = "Admin",
                Permissions = new List<string> { "Product.View", "Product.Create", "Product.Update", "Product.Delete" }
            },
            new DotNetTrainingBatch0.Database.AppDbContextModels.AppUser
            {
                Id = 2,
                Username = "staff",
                Password = "123",
                Role = "Staff",
                Permissions = new List<string> { "Product.View" }
            }
        );
        context.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

using System.Text;
using EraXP_Back.Models.Domain;
using EraXP_Back.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var scheme = new OpenApiSecurityScheme()
    {
        In = ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' following by space and JWT",
        Name = "Authorization", 
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
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    };
    
   c.SwaggerDoc("v1", new OpenApiInfo() { Title = "You api title", Version = "v1" });
   c.AddSecurityDefinition("Bearer", scheme);
   c.AddSecurityRequirement(securityRequirement);

});

builder.Services.AddControllers();

IConfigurationSection userUtils = builder.Configuration.GetSection("userUtils");

if (!userUtils.Exists())
    throw new NullReferenceException("userUtils section was missing from the configuration!");

string? key = userUtils["key"];

if (key == null)
    throw new NullReferenceException("The key for the JWT Signature is missing from the configuration!");

string jwtIssuer = userUtils["issuer"] ?? "EraXP_Service";

builder.Services.AddSingleton<UserClaimUtils>(it =>
{
    string? encoding = userUtils["encoding"] ?? "UTF-8";

    return new UserClaimUtils(key, jwtIssuer, encoding);
});

builder.Services
    .AddAuthentication(options =>
    {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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
                ValidAudience = jwtIssuer,
                IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(key))
            };
            options.MapInboundClaims = false;
        }
    );

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(options =>
    {
    });
    app.UseSwaggerUI(options =>
    {
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

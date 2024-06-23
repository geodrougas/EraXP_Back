using EraXP_Back.Persistence;
using EraXP_Back.PostgresQL;
using EraXP_Back.Utils;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

#region UserClaims

{
    IConfigurationSection userClaimUtilsSection = builder.Configuration.GetSection("userClaimUtils");

    if (!userClaimUtilsSection.Exists())
        throw new NullReferenceException("userUtils section was missing from the configuration!");

    string? key = userClaimUtilsSection["key"];

    if (key == null)
        throw new NullReferenceException("The key for the JWT Signature is missing from the configuration!");

    string jwtIssuer = userClaimUtilsSection["issuer"] ?? "EraXP_Service";

    int jwtLifeSpan;

    if (!int.TryParse(userClaimUtilsSection["jwtLifeSpan"], out jwtLifeSpan))
        jwtLifeSpan = 1440;

    builder.Services.AddSingleton<ClaimUtils>(it =>
    {
        string? encoding = userClaimUtilsSection["encoding"] ?? "UTF-8";

        return new ClaimUtils(key, jwtIssuer, encoding, TimeSpan.FromMinutes(jwtLifeSpan));
    });
    
    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "Multiple_schemes";
            options.DefaultChallengeScheme = "Multiple_schemes";
            options.DefaultScheme = "Multiple_schemes";
        })
        .AddPolicyScheme("Multiple_schemes", "Multiple Schemes", options =>
        {
            options.ForwardDefaultSelector = context =>
            {
                string?[]? authorizationHeader = context.Request.Headers.Authorization;

                if (authorizationHeader != null
                    && authorizationHeader.Any(m => m != null && m.TrimStart().StartsWith("bearer", StringComparison.CurrentCultureIgnoreCase)))
                    return JwtBearerDefaults.AuthenticationScheme;

                return CookieAuthenticationDefaults.AuthenticationScheme;
            };
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
            }
        )
        .AddCookie();
}
#endregion UserClaims

#region db
{
    IConfigurationSection dbSection = builder.Configuration.GetSection("db");
    string connectionString = dbSection["connectionString"]
        ?? throw new NullReferenceException("You must provide a connectionstring for the database!");
    builder.Services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>(m =>
        new DbConnectionFactory(connectionString));
}
#endregion

#region CypherUtil

{
    IConfigurationSection cypherSection = builder.Configuration.GetSection("cypher");

    if (!cypherSection.Exists())
        throw new NullReferenceException("cypher section was missing from the configuration!");

    string? key = cypherSection["key"];

    if (key == null)
        throw new NullReferenceException("The key field from the cypher section is missing from the configuration!");

    builder.Services.AddSingleton<CypherUtil>(it => new CypherUtil(key));
}

#endregion

builder.Services.AddSingleton<AuthorityUtils>();

builder.Services.AddSingleton<UserUtils>();


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

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

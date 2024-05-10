using EraXP_Back.Models.Domain;
using EraXP_Back.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddSingleton<UserClaimUtils>(it =>
{
    IConfigurationSection section = builder.Configuration.GetSection("userUtils");

    if (!section.Exists())
        throw new NullReferenceException("userUtils section was missing from the configuration!");
    string? key = section["key"];
    string? encoding = section["encoding"] ?? "UTF-8";

    if (key == null)
        throw new NullReferenceException("The key for the JWT Signature is missing from the configuration!");

    return new UserClaimUtils(key, encoding);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

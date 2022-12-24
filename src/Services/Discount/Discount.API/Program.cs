using Discount.API.Extensions;
using Discount.API.Repositories;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {Title = "Discount.API", Version = "v1"});
});
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Host.UseSerilog((_, lc) => lc.WriteTo.Console());




var app = builder.Build();
await app.MigrateDatabase();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Discount.API v1"));
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();
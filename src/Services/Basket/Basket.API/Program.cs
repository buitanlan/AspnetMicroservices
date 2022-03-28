using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder();


builder.Services.AddControllers();
builder.Services.AddStackExchangeRedisCache(option =>
{
    option.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Basket.API", Version = "v1" });
});
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddScoped<IDiscountGrpcService, DiscountGrpcService>();
builder.Host.UseSerilog((_, lc) => lc.WriteTo.Console());



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket.API v1"));
}

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();

using Discount.Grpc.Extentions;
using Discount.Grpc.Repositories;
using Discount.Grpc.Services;
using Serilog;

var builder = WebApplication.CreateBuilder();

builder.Services.AddGrpc();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Host.UseSerilog((_, lc) => lc.WriteTo.Console());


var app = builder.Build();

await app.MigrateDatabase<Program>();
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.MapGrpcService<DiscountService>();

app.Run();
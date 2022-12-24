﻿using System.Data;
using Dapper;
using Discount.Grpc.Entities;
using Npgsql;

namespace Discount.Grpc.Repositories;

public class DiscountRepository: IDiscountRepository
{
    private readonly string? _connectionString;
    public DiscountRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetValue<string>("DatabaseSettings:ConnectionString");
    }
        
        
    public async Task<Coupon> GetDiscount(string productName)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        if (connection.State == ConnectionState.Closed)
        {
            await connection.OpenAsync();
        }

        const string query = @"select * from Coupon where ProductName = @ProductName";
        var coupon = await connection.QueryFirstOrDefaultAsync<Coupon>(query, new {ProductName = productName});

        if (coupon is null)
        {
            return new Coupon
            {
                ProductName = "No Discount",
                Amount = 0,
                Description = "No Discount Desc",
            };
        }

        return coupon;
    }

    public async Task<bool> CreateDiscount(Coupon coupon)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        if (connection.State == ConnectionState.Closed)
        {
            await connection.OpenAsync();
        }

        const string query =
            @"insert into Coupon  (ProductName, Description, Amount) values (@ProductName, @Description,  @Amount)";
        var couponQuery = new
        {
            coupon.ProductName,
            coupon.Description,
            coupon.Amount
        };

        var affectedRow = await connection.ExecuteAsync(query, couponQuery);
        return affectedRow != 0;
    }

        
    public async Task<bool> UpdateDiscount(Coupon coupon)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        if (connection.State == ConnectionState.Closed)
        {
            await connection.OpenAsync();
        }

        const string query =
            @"update Coupon set ProductName = @ProductName, Description = @Description, Amount = @Amount where Id = @Id";
        var couponQuery = new
        {
            coupon.ProductName,
            coupon.Description,
            coupon.Amount
        };

        var affectedRow = await connection.ExecuteAsync(query, couponQuery);
        return affectedRow != 0;
    }

    public async Task<bool> DeleteDiscount(string productName)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        if (connection.State == ConnectionState.Closed)
        {
            await connection.OpenAsync();
        }

        const string query = @"delete from Coupon where ProductName = @ProductName";
           

        var affectedRow = await connection.ExecuteAsync(query, new {ProductName = productName});
        return affectedRow != 0;
    }
}
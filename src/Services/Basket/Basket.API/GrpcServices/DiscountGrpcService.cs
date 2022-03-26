using Discount.Grpc.Protos;
using Grpc.Net.Client;

namespace Basket.API.GrpcServices;

public class DiscountGrpcService: IDiscountGrpcService
{
    private readonly DiscountProtoService.DiscountProtoServiceClient _client;
    public DiscountGrpcService(IConfiguration configuration)
    {
        var channel = GrpcChannel.ForAddress(configuration.GetValue<string>("GrpcSettings:DiscountUrl"));
        _client = new DiscountProtoService.DiscountProtoServiceClient(channel);
    }

    public async Task<CouponModel> GetDiscount(string productName)
    {
        var discountRequest = new GetDiscountRequest
        {
            ProductName = productName
        };

        return await _client.GetDiscountAsync(discountRequest);
    }
}
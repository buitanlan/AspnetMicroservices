using Discount.Grpc.Protos;

namespace Basket.API.GrpcServices;

public interface IDiscountGrpcService
{
    public Task<CouponModel> GetDiscount(string productName);

}
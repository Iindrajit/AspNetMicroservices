using Discount.Grpc.Protos;
using System.Threading.Tasks;

namespace Basket.API.GrpcServices
{
    //This class will encapsulate the genereated client class by VS. Its a wrapper over grpc service.
    public class DiscountGrpcService
    {
        private readonly DiscountProvider.DiscountProviderClient _discountProviderClient;

        public DiscountGrpcService(DiscountProvider.DiscountProviderClient discountProviderClient)
        {
            _discountProviderClient = discountProviderClient;
        }

        public async Task<CouponDto> GetDiscount(string productName)
        {
            var request = new GetDiscountRequest{ ProductName = productName };

            return await _discountProviderClient.GetDiscountAsync(request);
        }

    }
}

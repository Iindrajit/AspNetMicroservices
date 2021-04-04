using AutoMapper;
using Discount.API.Dtos;
using Discount.API.Entities;

namespace Discount.API.Utilities
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Coupon, CouponDto>()
                     .ReverseMap();
        }
    }
}

﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Discount.Grpc.Services
{
    //Inherit from the class generated by the proto service class that was auto generated
    public class DiscountService : DiscountProvider.DiscountProviderBase
    {
        private readonly IDiscountRepository _repository;
        private readonly ILogger<DiscountService> _logger;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository repository, ILogger<DiscountService> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public override async Task<CouponDto> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.ProductName))
                throw new RpcException(new Status(StatusCode.InvalidArgument,"Product name is required."));

            var coupon = await _repository.GetDiscount(request.ProductName);
            if(coupon == null)
            {
                throw new RpcException(
                    new Status(StatusCode.NotFound, $"Discount with Product name: {request.ProductName}, not found"));
            }
            _logger.LogInformation($"Discount details - Product name: {coupon.ProductName}, Amount: {coupon.Amount}");

            var couponDto = _mapper.Map<CouponDto>(coupon);
            return couponDto;
        }

        public override async Task<CouponDto> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.Coupon);
            var isCreated = await _repository.CreateDiscount(coupon);

            if (isCreated)
            {
                _logger.LogInformation($"Discount is successfully created for product: {coupon.ProductName}");

                var couponDto = _mapper.Map<CouponDto>(coupon);
                return couponDto;
            }
            else
            {
                _logger.LogInformation($"Failed to create discount: Product {coupon.ProductName}, " +
                    $"Description {coupon.Description}, Amount {coupon.Amount}");

                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, $"Failed to create discount for the product: {coupon.ProductName}."));
            }
        }

        public override async Task<CouponDto> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var prodName = request.Coupon.ProductName;
            var couponFromRepo = await _repository.GetDiscount(prodName);

            if (couponFromRepo.Id == 0) // not available in db 
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Product name is required."));

            _mapper.Map(request.Coupon, couponFromRepo);

            bool isUpdated = await _repository.UpdateDiscount(couponFromRepo);
            if (isUpdated)
                _logger.LogInformation($"Discount is succesfully updated for product: {prodName}");
            else
                _logger.LogInformation($"Discount details of Product: {prodName}, not found.");

            var couponDto = _mapper.Map<CouponDto>(couponFromRepo);

            return couponDto;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            if (string.IsNullOrWhiteSpace(request.ProductName))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Product name is required."));

            var isDeleted = await _repository.DeleteDiscount(request.ProductName);

            if (isDeleted)
                _logger.LogInformation($"Discount deleted for product: {request.ProductName}");
            else
                _logger.LogInformation($"Discount details of Product: {request.ProductName}, not found.");

            var response = new DeleteDiscountResponse
            {
                Success = isDeleted
            };

            return response;
        }
    }
}
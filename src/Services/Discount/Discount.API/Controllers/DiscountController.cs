using AutoMapper;
using Discount.API.Dtos;
using Discount.API.Entities;
using Discount.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Discount.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountRepository _repository;
        private readonly IMapper _mapper;

        public DiscountController(IDiscountRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("{productName}", Name = "GetDiscount")]
        [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CouponDto>> GetDiscount(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return BadRequest("Product name is required.");

            var coupon = await _repository.GetDiscount(productName);
            var couponDto = _mapper.Map<CouponDto>(coupon);
            return Ok(couponDto);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CouponDto>> CreateDiscount([FromBody] CouponDto couponDto)
        {
            var coupon = _mapper.Map<Coupon>(couponDto);
            var isCreated = await _repository.CreateDiscount(coupon);

            if(isCreated)
                return CreatedAtRoute("GetDiscount", new { productName = coupon.ProductName }, couponDto);
            else 
                return BadRequest("Failed to create discount. Please check the parameters passed.");
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDiscount([FromBody] CouponDto couponDto)
        {
            var couponFromRepo = await _repository.GetDiscount(couponDto.ProductName);

            if (couponFromRepo.Id == 0) // not available in db 
                return NotFound($"Product: {couponDto.ProductName}, could not be found.");
            
            _mapper.Map(couponDto, couponFromRepo);

            if (await _repository.UpdateDiscount(couponFromRepo))
            {
                return NoContent();
            }

            return BadRequest($"Product:{couponDto.ProductName} could not be updated. Please check the parameters passed.");
        }

        [HttpDelete("{productName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteDiscount(string productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return BadRequest("Product name is required.");

            if (await _repository.DeleteDiscount(productName))
                return Ok();
            else
                return NotFound($"Product:{productName} could not be found.");
        }
    }
}
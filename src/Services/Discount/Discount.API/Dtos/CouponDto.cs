using System.ComponentModel.DataAnnotations;

namespace Discount.API.Dtos
{
    public class CouponDto
    {
        [Required(ErrorMessage = "Product name is required.")]
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; } = 0;
    }
}

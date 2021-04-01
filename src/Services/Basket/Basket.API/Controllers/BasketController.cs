using Basket.API.Entities;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;

        public BasketController(IBasketRepository repository)
        {
            _repository = repository;
        }
        
        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return BadRequest($"Username is required.");

            var basket = await _repository.GetBasket(userName);
            return Ok(basket);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
        {
            if (string.IsNullOrWhiteSpace(basket.UserName))
                return BadRequest($"Username is required.");

            var updatedBasket = await _repository.UpdateBasket(basket);
            return Ok(updatedBasket);
        }

        [HttpDelete("{userName}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
                return BadRequest($"Username is required.");

            await _repository.DeleteBasket(userName);
            return Ok();
        }

    }
}
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(IProductRepository repository, ILogger<CatalogController> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _repository.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id:length(24)}", Name = "GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await _repository.GetProductById(id);

            if (product == null)
            {
                _logger.LogError($"Product with id: {id}, not found.");
                return NotFound();
            }

            return Ok(product);
        }

        [Route("[action]/{category}")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetProductByCategory(string category)
        {
            var products = await _repository.GetProductByCategory(category);

            if (products == null)
            {
                _logger.LogError($"Products by category: {category}, not found.");
                return NotFound();
            }

            return Ok(products);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> CreateProduct(Product product)
        {
            await _repository.CreateProduct(product);
            return CreatedAtRoute("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut]
        //[ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            //check if the object is available for updation.
            var productToUpdate = await _repository.GetProductById(product.Id);

            if (productToUpdate == null)
                return NotFound($"Product with id: {product.Id} not found to update.");

            bool isUpdated = await _repository.UpdateProduct(product);
            if (isUpdated)
                return NoContent();

            _logger.LogError($"Error while trying to update product with id: {product.Id}");
            return StatusCode(500);
        }

        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProductById(string id)
        {
            // get the object to delete
            var productToDelete = await _repository.GetProductById(id);

            if (productToDelete == null)
                return NotFound($"Product with id: {id} not found to delete.");

            bool isDeleted = await _repository.DeleteProduct(id);

            if (isDeleted)
                return Ok();

            _logger.LogError($"Error while trying to delete product with id: {id}");
            return StatusCode(500);
        }
    }
}
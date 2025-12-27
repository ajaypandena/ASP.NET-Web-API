using Microsoft.AspNetCore.Mvc;
using DemoAPI.Data;
using DemoAPI.Models;
using System;
using Microsoft.Extensions.Logging;

namespace DemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
       
        // GET: api/products
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(ProductRepository.Products);
        }

        // GET: api/products/1
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {

            var product = ProductRepository.Products.FirstOrDefault(p => p.Id == id);
            if (product == null)
                return NotFound();
            return Ok(product);
            //var product = ProductRepository.Products.FirstOrDefault(p => p.Id == id);
            //if (product == null) return NotFound();
            //return Ok(product);
        }

        // POST: api/products

        [HttpPost("batch")]
        public IActionResult CreateMultiple([FromBody] List<ProductsDto> products)
        {
            if (products == null || !products.Any())
                return BadRequest("No products provided.");

            var existingIds = ProductRepository.Products
                .Select(p => p.Id)
                .OrderBy(id => id)
                .ToList();

            int nextId = 1;
            var createdProducts = new List<Product>();

            foreach (var dto in products)
            {
                while (existingIds.Contains(nextId))
                    nextId++;

                var product = new Product
                {
                    Id = nextId,
                    Name = dto.Name,
                    Price = (double)dto.Price,
                    Quantity = dto.Quantity,
                    CreatedAt = DateTime.UtcNow
                };

                existingIds.Add(nextId);
                ProductRepository.Products.Add(product);
                Log.Logs($"Created product Id={product.Id}, Name={product.Name}");

                createdProducts.Add(product);
            }

            ProductRepository.SaveChanges();

            return Created("", createdProducts);
        }




        //[HttpPost("batch")]
        //public IActionResult CreateMultiple([FromBody] List<Product> products)
        //{
        //    if (products == null || !products.Any())
        //        return BadRequest("No products provided.");

        //    foreach (var product in products)
        //    {
        //        product.Id = ProductRepository.Products.Any()
        //            ? ProductRepository.Products.Max(p => p.Id) + 1
        //            : 1;

        //        ProductRepository.Products.Add(product);
        //    }

        //    ProductRepository.SaveChanges();

        //    return Created("", products); // 201 Created with the list
        //}


        // PUT: api/products/1
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ProductsDto updatedProduct)
        {
            var product = ProductRepository.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
           
            product.Name = updatedProduct.Name;
            product.Price = (double)updatedProduct.Price;
            product.Quantity = updatedProduct.Quantity;
            product.UpdatedAt = DateTime.UtcNow;
            Log.Logs($"Updated product Id={product.Id}, Name={product.Name}");
            return NoContent();
        }

        // DELETE: api/products/1
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = ProductRepository.Products.FirstOrDefault(p => p.Id == id);
            if (product == null) return NotFound();
            ProductRepository.Products.Remove(product);
            Log.Logs($"Deleted product Id={product.Id}, Name={product.Name}");

            return NoContent();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Northwind.EntityModels;

namespace Northwind.WebApi.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private int pageSize = 10;
        private readonly ILogger<ProductsController> _logger;
        private readonly NorthwindContext _db;
        public ProductsController(ILogger<ProductsController> logger,
        NorthwindContext context)
        {
            _logger = logger;
            _db = context;
        }
        // GET: api/products
        [HttpGet]
        [Produces(typeof(Product[]))]
        public IEnumerable<Product> Get(int? page)
        {
            return _db.Products
            .Where(p => p.UnitsInStock > 0 && !p.Discontinued)
            .OrderBy(product => product.ProductId)
            .Skip(((page ?? 1) - 1) * pageSize)
            .Take(pageSize);
        }
        // GET: api/products/outofstock
        [HttpGet]
        [Route("outofstock")]
        [Produces(typeof(Product[]))]
        public IEnumerable<Product> GetOutOfStockProducts()
        {
            return _db.Products
            .Where(p => p.UnitsInStock == 0 && !p.Discontinued);
        }
        // GET: api/products/discontinued
        [HttpGet]
        [Route("discontinued")]
        [Produces(typeof(Product[]))]
        public IEnumerable<Product> GetDiscontinuedProducts()
        {
            return _db.Products
            .Where(product => product.Discontinued);
        }
        // GET api/products/5
        [HttpGet("{id:int}")]
        public async ValueTask<Product?> Get(int id)
        {
            return await _db.Products.FindAsync(id);
        }
        // GET api/products/cha
        [HttpGet("{name}")]
        public IEnumerable<Product> Get(string name)
        {
            return _db.Products.Where(p => p.ProductName.Contains(name));
        }
        // POST api/products
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            return Created($"api/products/{product.ProductId}", product);
        }
        // PUT api/products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Product product)
        {
            Product? foundProduct = await _db.Products.FindAsync(id);
            if (foundProduct is null) return NotFound();
            foundProduct.ProductName = product.ProductName;
            foundProduct.CategoryId = product.CategoryId;
            foundProduct.SupplierId = product.SupplierId;
            foundProduct.QuantityPerUnit = product.QuantityPerUnit;
            foundProduct.UnitsInStock = product.UnitsInStock;
            foundProduct.UnitsOnOrder = product.UnitsOnOrder;
            foundProduct.ReorderLevel = product.ReorderLevel;
            foundProduct.UnitPrice = product.UnitPrice;
            foundProduct.Discontinued = product.Discontinued;
            await _db.SaveChangesAsync();
            return NoContent();
        }
        // DELETE api/products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (await _db.Products.FindAsync(id) is Product product)
            {
                _db.Products.Remove(product);
                await _db.SaveChangesAsync();
                return NoContent();
            }
            return NotFound();
        }
    }
}

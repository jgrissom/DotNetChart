using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Northwind.Models;

namespace Northwind.Controllers
{
    public class APIController : Controller
    {
        // this controller depends on the NorthwindRepository
        private INorthwindRepository repository;
        public APIController(INorthwindRepository repo) => repository = repo;

        [HttpGet, Route("api/product")]
        // returns all products
        public IEnumerable<Product> Get() => repository.Products.OrderBy(p => p.ProductName);
        [HttpGet, Route("api/product/{id}")]
        // returns specific product
        public Product Get(int id) => repository.Products.FirstOrDefault(p => p.ProductId == id);
        [HttpGet, Route("api/product/discontinued/{discontinued}")]
        // returns all products where discontinued = true/false
        public IEnumerable<Product> GetDiscontinued(bool discontinued) => repository.Products.Where(p => p.Discontinued == discontinued).OrderBy(p => p.ProductName);
        [HttpGet, Route("api/category/{CategoryId}/product")]
        // returns all products in a specific category
        public IEnumerable<Product> GetByCategory(int CategoryId) => repository.Products.Where(p => p.CategoryId == CategoryId).OrderBy(p => p.ProductName);
        [HttpGet, Route("api/category/{CategoryId}/product/discontinued/{discontinued}")]
        // returns all products in a specific category where discontinued = true/false
        public IEnumerable<Product> GetByCategoryDiscontinued(int CategoryId, bool discontinued) => repository.Products.Where(p => p.CategoryId == CategoryId && p.Discontinued == discontinued).OrderBy(p => p.ProductName);
        [HttpPost, Route("api/addtocart")]
        // adds a row to the cartitem table
        public CartItem Post([FromBody] CartItemJSON cartItem) => repository.AddToCart(cartItem);

        [HttpGet, Route("api/category/{CategoryId}/revenuebycountry")]
        public IEnumerable<SalesByCountry> GetRevenueByCountry(int CategoryId)=> repository.OrderDetails.Where(od => od.Product.CategoryId == CategoryId).GroupBy(od => od.Order.Customer.Country).Select(grp => new SalesByCountry
            {
                country = grp.Key,
                revenue = grp.Sum(x => x.Quantity * x.UnitPrice * (1 - x.Discount))
            }).OrderByDescending(p => p.revenue);

        [HttpGet, Route("api/category/{CategoryId}/revenuebyproduct")]
        public IEnumerable<RevByProduct> GetRevenueByProduct(int CategoryId) => repository.OrderDetails.Where(od => od.Product.CategoryId == CategoryId).GroupBy(od => od.Product.ProductName).Select(grp => new RevByProduct
        {
            product = grp.Key,
            revenue = grp.Sum(x => x.Quantity * x.UnitPrice * (1 - x.Discount))
        }).OrderBy(p => p.product);

        [HttpGet, Route("api/category/{CategoryId}/revenuebyyear")]
        public IEnumerable<RevByYear> GetRevenueByYear(int CategoryId) => repository.OrderDetails.Where(od => od.Product.CategoryId == CategoryId).GroupBy(od => od.Order.OrderDate.Value.Year).Select(grp => new RevByYear 
        {
            year = grp.Key,
            revenue = grp.Sum(x => x.Quantity * x.UnitPrice * (1 - x.Discount))
        }).OrderBy(x => x.year);
    }
}

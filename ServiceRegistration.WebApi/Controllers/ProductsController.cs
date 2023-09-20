using Microsoft.AspNetCore.Mvc;
using ServiceRegistration.WebApi.Model;
using ServiceRegistration.WebApi.Repositories;

namespace ServiceRegistration.WebApi.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private IRepository<Product> Repository { get; }
    public ProductsController(IRepository<Product> repository)
    {
        Repository = repository;
    }

    [HttpGet(Name = "GetProducts")]
    public Task<IEnumerable<Product>> Get()
    {
        var products = Repository.GetAllAsync();
        return products;
    }
}
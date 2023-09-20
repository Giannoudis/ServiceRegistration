using Microsoft.AspNetCore.Mvc;
using ServiceRegistration.WebApi.Model;
using ServiceRegistration.WebApi.Repositories;

namespace ServiceRegistration.WebApi.Controllers;

[ApiController]
[Route("tenants")]
public class TenantsController : ControllerBase
{
    private IRepository<Tenant> Repository { get; }
    public TenantsController(IRepository<Tenant> repository)
    {
        Repository = repository;
    }

    [HttpGet(Name = "GetTenants")]
    public Task<IEnumerable<Tenant>> Get()
    {
        var tenants = Repository.GetAllAsync();
        return tenants;
    }
}
using Microsoft.AspNetCore.Mvc;
using ServiceRegistration.WebApi.Model;
using ServiceRegistration.WebApi.Repositories;

namespace ServiceRegistration.WebApi.Controllers;

[ApiController]
[Route("employees")]
public class EmployeesController : ControllerBase
{
    private IRepository<Employee> Repository { get; }
    public EmployeesController(IRepository<Employee> repository)
    {
        Repository = repository;
    }

    [HttpGet(Name = "GetEmployees")]
    public Task<IEnumerable<Employee>> Get()
    {
        var employees = Repository.GetAllAsync();
        return employees;
    }
}
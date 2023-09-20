using System.Reflection;
using System.Text.Json;
using ServiceRegistration.WebApi.Model;

namespace ServiceRegistration.WebApi.Repositories;

[ServiceTransient]
public class ProductRepository : IRepository<Product>
{
    private string FileName { get; }
    private readonly JsonSerializerOptions serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ProductRepository()
    {
        var fileName = "Data\\Products.json";

        // file is located on the executable folder
        var assembly = Assembly.GetEntryAssembly();
        var directory = assembly != null
            ? Path.GetDirectoryName(assembly.Location)
            : Path.GetDirectoryName(fileName);
        FileName = directory != null
            ? Path.Combine(directory, fileName)
            : fileName;
    }

    /// <summary>Get employees</summary>
    public Task<IEnumerable<Product>> GetAllAsync()
    {
        if (File.Exists(FileName))
        {
            var caseFields = JsonSerializer.Deserialize<List<Product>>(
                File.ReadAllText(FileName),
                serializerOptions);
            if (caseFields != null)
            {
                return Task.FromResult(caseFields.AsEnumerable());
            }
        }
        return Task.FromResult(new List<Product>().AsEnumerable());
    }
}
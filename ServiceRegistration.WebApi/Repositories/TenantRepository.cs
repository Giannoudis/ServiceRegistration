using System.Reflection;
using System.Text.Json;
using ServiceRegistration.WebApi.Model;

namespace ServiceRegistration.WebApi.Repositories;

[ServiceTransient]
public class TenantRepository : IRepository<Tenant>
{
    private string FileName { get; }
    private readonly JsonSerializerOptions serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public TenantRepository()
    {
        var fileName = "Data\\Tenants.json";

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
    public Task<IEnumerable<Tenant>> GetAllAsync()
    {
        if (File.Exists(FileName))
        {
            var caseFields = JsonSerializer.Deserialize<List<Tenant>>(
                File.ReadAllText(FileName),
                serializerOptions);
            if (caseFields != null)
            {
                return Task.FromResult(caseFields.AsEnumerable());
            }
        }
        return Task.FromResult(new List<Tenant>().AsEnumerable());
    }
}
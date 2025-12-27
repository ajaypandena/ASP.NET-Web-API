using System.Text.Json;

using DemoAPI.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;


namespace DemoAPI.Data
{
    public static class ProductRepository
{
    private static string _filePath;
    private static List<Product> _products;

    // Initialize repository with configuration
    public static void Initialize(IConfiguration configuration)
    {
        _filePath = configuration["ProductFilePath"];
            _filePath = Environment.ExpandEnvironmentVariables(_filePath);

           // Console.WriteLine($"Products JSON file path: {_filePath}");


            if (string.IsNullOrWhiteSpace(_filePath))
            throw new Exception("ProductFilePath is not set in configuration.");

        // Ensure folder exists
        var folder = Path.GetDirectoryName(_filePath);
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        // Initialize empty file if missing
        if (!File.Exists(_filePath))
            File.WriteAllText(_filePath, "[]");
    }

    public static List<Product> Products
    {
        get
        {
            if (_products == null)
            {
                try
                {
                    var json = File.ReadAllText(_filePath);
                    _products = string.IsNullOrWhiteSpace(json)
                        ? new List<Product>()
                        : JsonSerializer.Deserialize<List<Product>>(json) ?? new List<Product>();
                }
                catch (JsonException)
                {
                    _products = new List<Product>();
                }
            }
            return _products;
        }
    }

        public static void SaveChanges()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };

            var json = JsonSerializer.Serialize(_products, options);
            File.WriteAllText(_filePath, json);
        }

    }

}

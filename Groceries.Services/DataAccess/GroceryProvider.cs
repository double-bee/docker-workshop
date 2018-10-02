using System.Collections.Generic;
using System.IO;
using Groceries.Services;
using Microsoft.Extensions.Options;

public class GroceryProvider : IGroceryProvider
{
    private GroceryProviderOptions _options;

    public GroceryProvider(GroceryProviderOptions options)
    {
        _options = options;
    }

    public IEnumerable<Grocery> GetAll()
    {
        using (StreamReader reader = File.OpenText(_options.DataPath))
        {
            Newtonsoft.Json.JsonSerializer jsonSerializer = new Newtonsoft.Json.JsonSerializer();
            IEnumerable<Grocery> result = (IEnumerable<Grocery>)jsonSerializer.Deserialize(reader, typeof(IEnumerable<Grocery>));

            return result;
        }
    }
}
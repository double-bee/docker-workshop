using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Groceries.Services
{
    public class GroceryRepository : IGroceryRepository
    {
        private GroceryRepositoryOptions _options;

        public GroceryRepository(IOptions<GroceryRepositoryOptions> options)
        {
            _options = options.Value;
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

        public void Add(Grocery grocery)
        {
            IEnumerable<Grocery> items = GetAll();
            items = items.Concat(new [] {grocery});
            Save(items);
        }

        public void Remove(Grocery grocery)
        {
            IEnumerable<Grocery> items = GetAll();
            items = items.Except(new [] {grocery});
            Save(items);
        }

        private void Save(IEnumerable<Grocery> groceries)
        {
            using (TextWriter textWriter = new StreamWriter(_options.DataPath, false))
            {
                Newtonsoft.Json.JsonSerializer jsonSerializer = new Newtonsoft.Json.JsonSerializer();
                jsonSerializer.Serialize(textWriter, groceries);
            }
        }
    }
}
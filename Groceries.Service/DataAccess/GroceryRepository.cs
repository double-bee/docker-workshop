using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;

namespace Groceries.Service
{
    public class GroceryRepository : IGroceryRepository
    {
        private const string DataPath = "/data/groceries.json";

        public GroceryRepository()
        {
            CreateDummyDataIfNotFound();
        }

        public IEnumerable<Grocery> GetAll()
        {
            using (StreamReader reader = File.OpenText(DataPath))
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
            using (TextWriter textWriter = new StreamWriter(DataPath, false))
            {
                Newtonsoft.Json.JsonSerializer jsonSerializer = new Newtonsoft.Json.JsonSerializer();
                jsonSerializer.Serialize(textWriter, groceries);
            }
        }

        private void CreateDummyDataIfNotFound()
        {
            string directory = Path.GetDirectoryName(DataPath);
            if (!String.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(DataPath))
            {
                var groceries = new List<Grocery>
                {
                    new Grocery { Name = "Chocolade", Created = new DateTime(2018, 10, 4, 14, 0, 0) },
                    new Grocery { Name = "Pizza", Created = new DateTime(2018, 10, 4, 14, 0, 1) },
                };
                Save(groceries);
            }
        }
    }
}
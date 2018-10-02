using System.Collections.Generic;

namespace Groceries.Services
{
    public interface IGroceryProvider
    {
        IEnumerable<Grocery> GetAll();
    }
}
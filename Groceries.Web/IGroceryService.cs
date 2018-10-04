using System.Collections.Generic;
using System.Threading.Tasks;
using Groceries.Web.Models;

namespace Groceries.Web
{
    public interface IGroceryService
    {
         Task<IEnumerable<Grocery>> GetAll();
    }
}
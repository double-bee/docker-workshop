using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Groceries.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroceriesController : ControllerBase
    {
        private readonly IGroceryRepository _groceryRepository;

        public GroceriesController(IGroceryRepository groceryRepository)
        {
            _groceryRepository = groceryRepository;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Grocery>> Get() => Ok(_groceryRepository.GetAll());

        // POST api/values
        [HttpPost]
        public void Post([FromBody] Grocery value)
        {
            _groceryRepository.Add(value);
        }

        // DELETE api/values/5
        [HttpDelete("{name}")]
        public void Delete(string name)
        {
            Grocery grocery = _groceryRepository.GetAll().FirstOrDefault(g => g.Name == name);
            _groceryRepository.Remove(grocery);
        }
    }
}

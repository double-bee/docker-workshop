using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Groceries.Services.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroceriesController : ControllerBase
    {
        private readonly IGroceryProvider _groceryProvider;

        public GroceriesController(IGroceryProvider groceryProvider)
        {
            _groceryProvider = groceryProvider;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<Grocery>> Get()
        {
            return Ok(_groceryProvider.GetAll());
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

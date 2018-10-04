using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Groceries.Web.Models;
using Newtonsoft.Json;

namespace Groceries.Web
{
    public class GroceryService : IGroceryService
    {
        private readonly HttpClient _httpClient;

        public GroceryService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Grocery>> GetAll()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("/");
            response.EnsureSuccessStatusCode();
            IEnumerable<Grocery> result = await response.Content.ReadAsAsync<IEnumerable<Grocery>>();
            return result;
        }

        public async Task<Uri> Add(Grocery grocery)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/", grocery);
            response.EnsureSuccessStatusCode();
            return response.Headers.Location;
        }

        public async Task<HttpStatusCode> DeleteProductAsync(string name)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync(name);
            return response.StatusCode;
        }
    }
}
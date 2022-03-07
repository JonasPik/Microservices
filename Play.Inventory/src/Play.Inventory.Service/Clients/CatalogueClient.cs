using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Clients
{
    public class CatalogueClient
    {
        private readonly HttpClient httpClient;

        public CatalogueClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IReadOnlyCollection<CatalogueItemDto>> GetCatalogueItemsAsync()
        {
            var items = await httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogueItemDto>>("/items");
            return items;
        }
    }
}
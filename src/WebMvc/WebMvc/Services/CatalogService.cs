namespace ShoesOnContainers.WebMvc.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using ShoesOnContainers.WebMvc.Infastructure;
    using ShoesOnContainers.WebMvc.Models;

    public class CatalogService : ICatalogService
    {
        private readonly IOptionsSnapshot<AppSettings> _settings;
        private readonly IHttpClient _apiClient;
        private readonly ILogger<CatalogService> _logger;
        private readonly string _remoteServiceBaseUrl;

        public CatalogService(IOptionsSnapshot<AppSettings> settings, IHttpClient httpClient, ILogger<CatalogService> logger)
        {
            this._settings = settings;
            this._apiClient = httpClient;
            this._logger = logger;
            this._remoteServiceBaseUrl = $"{this._settings.Value.CatalogUrl}/api/catalog/";
        }

        public async Task<Catalog> GetCatalogItems(int page, int take, int? brand, int? type)
        {
            var allcatalogItemsUri = ApiPaths.Catalog.GetAllCatalogItems(this._remoteServiceBaseUrl, page, take, brand, type);
            var dataString = await this._apiClient.GetStringAsync(allcatalogItemsUri);
            var response = JsonConvert.DeserializeObject<Catalog>(dataString);
            return response;
        }

        public async Task<IEnumerable<SelectListItem>> GetBrands()
        {
            var getBrandsUri = ApiPaths.Catalog.GetAllBrands(this._remoteServiceBaseUrl);
            var dataString = await this._apiClient.GetStringAsync(getBrandsUri);
            var items = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All", Selected = true }
            };
            var brands = JArray.Parse(dataString);
            foreach (var brand in brands.Children<JObject>())
            {
                items.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("brand")
                });
            }

            return items;
        }

        public async Task<IEnumerable<SelectListItem>> GetTypes()
        {
            var getTypesUri = ApiPaths.Catalog.GetAllTypes(this._remoteServiceBaseUrl);
            var dataString = await this._apiClient.GetStringAsync(getTypesUri);
            var items = new List<SelectListItem>
            {
                new SelectListItem() { Value = null, Text = "All", Selected = true }
            };
            var brands = JArray.Parse(dataString);
            foreach (var brand in brands.Children<JObject>())
            {
                items.Add(new SelectListItem()
                {
                    Value = brand.Value<string>("id"),
                    Text = brand.Value<string>("type")
                });
            }

            return items;
        }
    }
}

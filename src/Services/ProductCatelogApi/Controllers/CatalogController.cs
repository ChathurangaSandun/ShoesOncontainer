using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ProductCatelogApi.Controllers
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;

    using ProductCatelogApi.Data;
    using ProductCatelogApi.Domain;
    using ProductCatelogApi.ViewModels;

    [Produces("application/json")]
    [Route("api/catalog")]
    public class CatalogController : Controller
    {
        private readonly CatalogContext _context;
        private readonly IOptionsSnapshot<CatalogSettings> _settings;

        public CatalogController(CatalogContext context,
                                 IOptionsSnapshot<CatalogSettings> settings)
        {
            this._context = context;
            this._settings = settings;
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogTypes()
        {
            var typeList = await this._context.CatalogTypes.ToListAsync<CatalogType>();
            return this.Ok(typeList);
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> CatalogBrands()
        {
            var brandList = await this._context.CatalogBrands.ToListAsync<CatalogBrand>();
            return this.Ok(brandList);
        }

        [HttpGet]
        [Route("items/{id:int}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            if (id <= 0)
            {
                return this.BadRequest();
            }

            var item = await this._context.CatalogItems.SingleOrDefaultAsync<CatalogItem>(o => o.Id == id);
            if (item != null)
            {
                item.PictureUrl = item.PictureUrl.Replace("http://externalcatalogbaseurltobereplaced", this._settings.Value.ExternalCatalogBaseUrl);
                return this.Ok(item);
            }

            return this.NotFound();
        }

        [HttpGet]
        [Route("[action]")]
        public async Task<IActionResult> Items([FromQuery] int pageSize = 5, [FromQuery] int pageIndex = 0)
        {
            var totalCount = await this._context.CatalogItems.LongCountAsync();
            var pageOnItems = await this._context.CatalogItems
                                  .OrderBy(o => o.Name)
                                  .Skip(pageSize * pageIndex)
                                  .Take(pageSize)
                                  .ToListAsync<CatalogItem>();
            pageOnItems = this.SetPicImage(pageOnItems);
            var viewModel = new PagenatedItemsViewModel<CatalogItem>(pageSize, pageIndex, totalCount, pageOnItems);
            return this.Ok(viewModel);
        }

        //// GET api/catalog/items/withname/pri?pagesize=5&pageIndex=0
        [HttpGet]
        [Route("[action]/withname/{name:minlength(1)}")]
        public async Task<IActionResult> Items(string name, [FromQuery] int pageSize = 5, [FromQuery] int pageIndex = 0)
        {
            var totalCount = await this._context.CatalogItems
                             .Where(o => o.Name.StartsWith(name)).LongCountAsync();
            var pageOnItems = await this._context.CatalogItems
                                  .Where(o => o.Name.StartsWith(name))
                                  .OrderBy(o => o.Name)
                                  .Skip(pageSize * pageIndex)
                                  .Take(pageSize)
                                  .ToListAsync<CatalogItem>();
            pageOnItems = this.SetPicImage(pageOnItems);
            var viewModel = new PagenatedItemsViewModel<CatalogItem>(pageSize, pageIndex, totalCount, pageOnItems);
            return this.Ok(viewModel);
        }

        //// GET api/catalog/items/type/1/brand/1?pagesize=5&pageIndex=0
        [HttpGet]
        [Route("[action]/type/{typeId}/brand/{brandId}")]
        public async Task<IActionResult> Items(int? typeId, int? brandId, [FromQuery] int pageSize = 5, [FromQuery] int pageIndex = 0)
        {
            var itemList = this._context.CatalogItems.AsQueryable<CatalogItem>();
            if (typeId.HasValue)
            {
                itemList = itemList.Where(x => x.CatalogTypeId.Equals(typeId));
            }

            if (brandId.HasValue)
            {
                itemList = itemList.Where(x => x.CatalogBrandId.Equals(brandId));
            }

            var totalCount = await itemList.LongCountAsync();
            var pageOnItems = await itemList
                                  .OrderBy(o => o.Name)
                                  .Skip(pageSize * pageIndex)
                                  .Take(pageSize)
                                  .ToListAsync<CatalogItem>();
            pageOnItems = this.SetPicImage(pageOnItems);
            var viewModel = new PagenatedItemsViewModel<CatalogItem>(pageSize, pageIndex, totalCount, pageOnItems);
            return this.Ok(viewModel);
        }

        [HttpPost]
        [Route("items")]
        public async Task<IActionResult> CreateProduct([FromBody] CatalogItem item)
        {
            this._context.CatalogItems.Add(item);
            await this._context.SaveChangesAsync();
            return this.CreatedAtAction("GetItemById", new { id = item.Id });
        }

        [HttpPut]
        [Route("items")]
        public async Task<IActionResult> UpdateProduct([FromBody] CatalogItem updatedItem)
        {
            var item = await this._context.CatalogItems.SingleOrDefaultAsync(o => o.Id == updatedItem.Id);
            if (item == null)
            {
                return this.NotFound(new { Message = $"Item with id {updatedItem.Id} not found" });
            }

            item = updatedItem;
            this._context.CatalogItems.Update(item);
            await this._context.SaveChangesAsync();
            return this.CreatedAtAction("GetItemById", new { id = item.Id });
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var item = await this._context.CatalogItems.SingleOrDefaultAsync(o => o.Id == id);
            if (item == null)
            {
                return this.NotFound(new { Message = $"Item with id= {id} not found" });
            }

            this._context.CatalogItems.Remove(item);
            await this._context.SaveChangesAsync();
            return this.NoContent();
        }

        private List<CatalogItem> SetPicImage(List<CatalogItem> pageOnItems)
        {
            pageOnItems.ForEach(x => x.PictureUrl = x.PictureUrl.Replace("http://externalcatalogbaseurltobereplaced", this._settings.Value.ExternalCatalogBaseUrl));
            return pageOnItems;
        }
    }
}
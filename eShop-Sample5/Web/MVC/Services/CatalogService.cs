using Microsoft.AspNetCore.Mvc.RazorPages;
using MVC.Dtos;
using MVC.Models.Enums;
using MVC.Services.Interfaces;
using MVC.ViewModels;

namespace MVC.Services;

public class CatalogService : ICatalogService
{
    private readonly IOptions<AppSettings> _settings;
    private readonly IHttpClientService _httpClient;
    private readonly ILogger<CatalogService> _logger;

    public CatalogService(IHttpClientService httpClient, ILogger<CatalogService> logger, IOptions<AppSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings;
        _logger = logger;
    }

    public async Task<Catalog> GetCatalogItems(int page, int take, int? brand, int? type)
    {
        var filters = new Dictionary<CatalogTypeFilter, int>();

        if (brand.HasValue)
        {
            filters.Add(CatalogTypeFilter.Brand, brand.Value);
        }
        
        if (type.HasValue)
        {
            filters.Add(CatalogTypeFilter.Type, type.Value);
        }
        
        var result = await _httpClient.SendAsync<Catalog, PaginatedItemsRequest<CatalogTypeFilter>>($"{_settings.Value.CatalogUrl}/items",
           HttpMethod.Post, 
           new PaginatedItemsRequest<CatalogTypeFilter>()
            {
                PageIndex = page,
                PageSize = take,
                Filters = filters
            });

        return result;
    }

    public async Task<IEnumerable<SelectListItem>> GetBrands()
    {
        var response = await _httpClient.GetAsync<List<CatalogBrand>>($"{_settings.Value.CatalogUrl}/GetBrands",HttpMethod.Get);
        if (response != null)
        {
            var catalogBrands = response;

            var list = catalogBrands.Select(item => new SelectListItem
            {
                Value = item.Id.ToString(),
                Text = item.Brand 
            }).ToList();

            return list;
        }
        else
        {

            return new List<SelectListItem>();
        }
    }

    public async Task<IEnumerable<SelectListItem>> GetTypes()
    {
        var response = await _httpClient.GetAsync<List<CatalogType>>($"{_settings.Value.CatalogUrl}/GetTypes", HttpMethod.Get);
        if (response != null)
        {
            var catalogTypes = response;

            var list = catalogTypes.Select(item => new SelectListItem
            {
                Value = item.Id.ToString(),
                Text = item.Type
            }).ToList();

            return list;
        }
        else
        {

            return new List<SelectListItem>();
        }
    }
}

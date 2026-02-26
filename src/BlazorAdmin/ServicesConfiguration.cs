using BlazorAdmin.Services;
using BlazorShared.Interfaces;
using BlazorShared.Models;
using Blazored.LocalStorage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BlazorAdmin;

public static class ServicesConfiguration
{
    public static IServiceCollection AddBlazorServices(this IServiceCollection services)
    {
        services.AddScoped<ICatalogLookupDataService<CatalogBrand>>(sp =>
            new CachedCatalogLookupDataServiceDecorator<CatalogBrand, CatalogBrandResponse>(
                sp.GetRequiredService<ILocalStorageService>(),
                sp.GetRequiredService<CatalogLookupDataService<CatalogBrand, CatalogBrandResponse>>(),
                sp.GetRequiredService<ILogger<CachedCatalogLookupDataServiceDecorator<CatalogBrand, CatalogBrandResponse>>>()));
        services.AddScoped<CatalogLookupDataService<CatalogBrand, CatalogBrandResponse>>();

        services.AddScoped<ICatalogLookupDataService<CatalogType>>(sp =>
            new CachedCatalogLookupDataServiceDecorator<CatalogType, CatalogTypeResponse>(
                sp.GetRequiredService<ILocalStorageService>(),
                sp.GetRequiredService<CatalogLookupDataService<CatalogType, CatalogTypeResponse>>(),
                sp.GetRequiredService<ILogger<CachedCatalogLookupDataServiceDecorator<CatalogType, CatalogTypeResponse>>>()));
        services.AddScoped<CatalogLookupDataService<CatalogType, CatalogTypeResponse>>();

        services.AddScoped<ICatalogItemService>(sp =>
            new CachedCatalogItemServiceDecorator(
                sp.GetRequiredService<ILocalStorageService>(),
                sp.GetRequiredService<CatalogItemService>(),
                sp.GetRequiredService<ILogger<CachedCatalogItemServiceDecorator>>()));
        services.AddScoped<CatalogItemService>();

        return services;
    }
}

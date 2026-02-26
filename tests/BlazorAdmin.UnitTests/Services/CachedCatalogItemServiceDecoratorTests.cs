using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using BlazorAdmin.Services;
using BlazorShared.Interfaces;
using BlazorShared.Models;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace MyApp.Tests
{
    public class CachedCatalogItemServiceDecoratorTests
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ICatalogItemService _inner;
        private readonly ILogger<CachedCatalogItemServiceDecorator> _logger;
        private readonly CachedCatalogItemServiceDecorator _sut;

        public CachedCatalogItemServiceDecoratorTests()
        {
            _localStorage = Substitute.For<ILocalStorageService>();
            _inner = Substitute.For<ICatalogItemService>();
            _logger = Substitute.For<ILogger<CachedCatalogItemServiceDecorator>>();
            _sut = new CachedCatalogItemServiceDecorator(_localStorage, _inner, _logger);
        }

        [Fact]
        public async Task List_ReturnsCachedItems_WhenCacheIsFresh()
        {
            var cached = new List<CatalogItem> { new CatalogItem { Id = 1, Name = "A" } };
            var entry = new CacheEntry<List<CatalogItem>>(cached) { DateCreated = DateTime.UtcNow };
            _localStorage.GetItemAsync<CacheEntry<List<CatalogItem>>>("items", Arg.Any<CancellationToken>())
                .Returns(new ValueTask<CacheEntry<List<CatalogItem>>>(entry));

            var result = await _sut.List();

            Assert.Single(result);
            Assert.Equal(1, result[0].Id);
            await _inner.DidNotReceive().List();
        }

        [Fact]
        public async Task ListPaged_FetchesAndCaches_WhenNoCacheEntry()
        {
            _localStorage.GetItemAsync<CacheEntry<List<CatalogItem>>>("items", Arg.Any<CancellationToken>())
                .Returns(new ValueTask<CacheEntry<List<CatalogItem>>>((CacheEntry<List<CatalogItem>>)null));
            var items = new List<CatalogItem> { new CatalogItem { Id = 2, Name = "B" } };
            _inner.ListPaged(10).Returns(Task.FromResult(items));

            var result = await _sut.ListPaged(10);

            Assert.Single(result);
            await _localStorage.Received(1).SetItemAsync("items", Arg.Any<CacheEntry<List<CatalogItem>>>(), Arg.Any<CancellationToken>());
        }
    }
}

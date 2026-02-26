using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Services;
using Microsoft.eShopWeb.ApplicationCore.Specifications;

namespace Microsoft.eShopWeb.UnitTests
{
    public class BasketServiceTests
    {
        // ── Mock declarations ──────────────────────────────────────────
        private readonly Mock<IRepository<Basket>> _mockRepository;
        private readonly Mock<IAppLogger<BasketService>> _mockAppLogger;

        // ── System Under Test ──────────────────────────────────────────
        private readonly BasketService _sut;

        // ── Constructor (xUnit creates new instance per test) ──────────
        public BasketServiceTests()
        {
            _mockRepository = new Mock<IRepository<Basket>>();
            _mockAppLogger = new Mock<IAppLogger<BasketService>>();

            _sut = new BasketService(
                _mockRepository.Object,
                _mockAppLogger.Object
            );
        }

        // ── Exception tests ────────────────────────────────────────────
        [Fact]
        public async Task AddItemToBasket_WhenUsernameIsNull_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.AddItemToBasket(null!, 1, 10.0m));
        }

        [Fact]
        public async Task AddItemToBasket_WhenCatalogItemIdIsZero_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.AddItemToBasket("username", 0, 10.0m));
        }

        [Fact]
        public async Task AddItemToBasket_WhenPriceIsNegative_ThrowsArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => _sut.AddItemToBasket("username", 1, -10.0m));
        }

        // ── Happy path ─────────────────────────────────────────────────
        [Fact]
        public async Task AddItemToBasket_WithValidInput_AddsItemToBasket()
        {
            // Arrange
            var username = "username";
            var catalogItemId = 1;
            var price = 10.0m;
            var quantity = 1;

            _mockRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(), default))
                .ReturnsAsync((Basket)null);
            _mockRepository.Setup(r => r.AddAsync(It.IsAny<Basket>(), default))
                .ReturnsAsync(new Basket(username));

            // Act
            var result = await _sut.AddItemToBasket(username, catalogItemId, price, quantity);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result.BuyerId);
            _mockRepository.Verify(r => r.FirstOrDefaultAsync(It.IsAny<BasketWithItemsSpecification>(), default), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Basket>(), default), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(result, default), Times.Once);
        }

        // ── Additional Exception tests ─────────────────────────────────
        [Fact]
        public async Task DeleteBasketAsync_WhenBasketIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            int basketId = 1;
            _mockRepository.Setup(r => r.GetByIdAsync(basketId, default))
                .ReturnsAsync((Basket)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.DeleteBasketAsync(basketId));
        }
    }
}

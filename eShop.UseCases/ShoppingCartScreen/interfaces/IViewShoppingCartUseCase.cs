using eShop.CoreBusiness.Models;

namespace eShop.UseCases.ShoppingCartScreen.interfaces
{
    public interface IViewShoppingCartUseCase
    {
        Task<Order> Execute();
    }
}
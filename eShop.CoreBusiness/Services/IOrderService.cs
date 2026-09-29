using eShop.CoreBusiness.Models;

namespace eShop.CoreBusiness.Services
{
    public interface IOrderService
    {
        bool ValidateCustomerInformation(string name, string address, string city, string province, string country);
        bool ValidateCreateOrder(Order order);
        bool ValidateUpdateOrder(Order order);
        bool ValidateProcessOrder(Order order);
    }
}

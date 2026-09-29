using eShop.CoreBusiness.Models;
using eShop.UseCases.PluginInterfaces.DataStore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eShop.DataStore.HardCode
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Dictionary<int, Order> orders;

        public OrderRepository()
        {
            orders = new Dictionary<int, Order>();
        }

        public int CreateOrder(Order order)
        {
            order.OrderId = orders.Count + 1;
            order.UniqueId = Guid.NewGuid().ToString();
            orders.Add(order.OrderId.Value, order);
            return order.OrderId.Value;
        }

        public IEnumerable<OrderLineItem> GetLineItemsByOrderId(int orderId)
        {
            throw new NotImplementedException();
        }

        public Order? GetOrder(int id)
        {
            return orders.Values.FirstOrDefault(x => x.OrderId == id);
        }

        public Order? GetOrderByUniqueId(string uniqueId)
        {
            return orders.Values.FirstOrDefault(x => x.UniqueId == uniqueId);
        }

        public IEnumerable<Order> GetOrders()
        {
            return orders.Values;
        }

        public IEnumerable<Order> GetOutstandingOrders()
        {
            return orders.Values.Where(x => x.DateProcessed == null || x.DateProcessed.HasValue == false);
        }

        public IEnumerable<Order> GetProcessedOrders()
        {
            return orders.Values.Where(x => x.DateProcessed.HasValue == true);
        }

        public void UpdateOrder(Order order)
        {
            if (order == null || order.OrderId == null || order.OrderId.HasValue == false) return;
            orders[order.OrderId.Value] = order;
        }
    }
}
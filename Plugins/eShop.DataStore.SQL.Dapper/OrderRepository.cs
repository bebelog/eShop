using System;
using System.Collections.Generic;
using System.Linq;
using eShop.CoreBusiness.Models;
using eShop.UseCases.PluginInterfaces.DataStore;
using eShop.DataStore.SQL.Dapper.Helpers;

namespace eShop.DataStore.SQL.Dapper
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDataAccess dataAccess;

        public OrderRepository(IDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public int CreateOrder(Order order)
        {
            string sql = @"
                INSERT INTO [dbo].[Order] (
                    [DatePlaced], [DateProcessing], [DateProcessed],
                    [CustomerName], [CustomerAddress], [CustomerCity],
                    [CustomerStateProvince], [CustomerCountry], [AdminUser], [UniqueId]
                )
                OUTPUT INSERTED.OrderId
                VALUES (
                    @DatePlaced, @DateProcessing, @DateProcessed,
                    @CustomerName, @CustomerAddress, @CustomerCity,
                    @CustomerStateProvince, @CustomerCountry, @AdminUser, @UniqueId
                )";

            int orderId = dataAccess.QuerySingle<int, Order>(sql, order);
            order.OrderId = orderId;

            if (order.LineItems != null)
            {
                foreach (var lineItem in order.LineItems)
                {
                    lineItem.OrderId = orderId;
                    sql = @"
                        INSERT INTO [dbo].[OrderLineItem] (
                            [ProductId], [OrderId], [Quantity], [Price]
                        )
                        VALUES (
                            @ProductId, @OrderId, @Quantity, @Price
                        )";
                    dataAccess.ExecuteCommand(sql, lineItem);
                }
            }

            return orderId;
        }

        public IEnumerable<OrderLineItem> GetLineItemsByOrderId(int orderId)
        {
            string sql = "SELECT * FROM [dbo].[OrderLineItem] WHERE OrderId = @OrderId";
            var lineItems = dataAccess.Query<OrderLineItem, dynamic>(sql, new { OrderId = orderId });
            foreach (var item in lineItems)
            {
                sql = "SELECT * FROM [dbo].[Product] WHERE ProductId = @ProductId";
                item.Product = dataAccess.QuerySingle<Product, dynamic>(sql, new { item.ProductId });
            }
            return lineItems;
        }

        public Order? GetOrder(int id)
        {
            string sql = "SELECT * FROM [dbo].[Order] WHERE OrderId = @OrderId";
            var order = dataAccess.QuerySingle<Order, dynamic>(sql, new { OrderId = id });

            if (order != null && order.OrderId.HasValue)
            {
                order.LineItems = GetLineItemsByOrderId(order.OrderId.Value).ToList();
            }

            return order;
        }

        public Order? GetOrderByUniqueId(string uniqueId)
        {
            string sql = "SELECT * FROM [dbo].[Order] WHERE UniqueId = @UniqueId";
            var order = dataAccess.QuerySingle<Order, dynamic>(sql, new { UniqueId = uniqueId });

            if (order != null && order.OrderId.HasValue)
            {
                order.LineItems = GetLineItemsByOrderId(order.OrderId.Value).ToList();
            }

            return order;
        }

        public IEnumerable<Order> GetOrders()
        {
            string sql = "SELECT * FROM [dbo].[Order]";
            return dataAccess.Query<Order, dynamic>(sql, new { });
        }

        public IEnumerable<Order> GetOutstandingOrders()
        {
            string sql = "SELECT * FROM [dbo].[Order] WHERE DateProcessed IS NULL";
            return dataAccess.Query<Order, dynamic>(sql, new { });
        }

        public IEnumerable<Order> GetProcessedOrders()
        {
            string sql = "SELECT * FROM [dbo].[Order] WHERE DateProcessed IS NOT NULL";
            return dataAccess.Query<Order, dynamic>(sql, new { });
        }

        public void UpdateOrder(Order order)
        {
            string sql = @"
                UPDATE [dbo].[Order]
                SET
                    [DatePlaced] = @DatePlaced,
                    [DateProcessing] = @DateProcessing,
                    [DateProcessed] = @DateProcessed,
                    [CustomerName] = @CustomerName,
                    [CustomerAddress] = @CustomerAddress,
                    [CustomerCity] = @CustomerCity,
                    [CustomerStateProvince] = @CustomerStateProvince,
                    [CustomerCountry] = @CustomerCountry,
                    [AdminUser] = @AdminUser,
                    [UniqueId] = @UniqueId
                WHERE OrderId = @OrderId";

            dataAccess.ExecuteCommand(sql, order);

            if (order.LineItems != null)
            {
                foreach (var lineItem in order.LineItems)
                {
                    if (lineItem.LineItemId.HasValue)
                    {
                        sql = @"
                            UPDATE [dbo].[OrderLineItem]
                            SET
                                [ProductId] = @ProductId,
                                [OrderId] = @OrderId,
                                [Quantity] = @Quantity,
                                [Price] = @Price
                            WHERE LineItemId = @LineItemId";
                    }
                    else
                    {
                        lineItem.OrderId = order.OrderId;
                        sql = @"
                            INSERT INTO [dbo].[OrderLineItem] (
                                [ProductId], [OrderId], [Quantity], [Price]
                            )
                            VALUES (
                                @ProductId, @OrderId, @Quantity, @Price
                            )";
                    }
                    dataAccess.ExecuteCommand(sql, lineItem);
                }
            }
        }
    }
}

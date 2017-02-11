using DataLayer.DBLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.ServiceModel;
using System.Transactions;

namespace ServiceLibrary
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    class OrderContract : IOrderContract
    {
        public IEnumerable<BusinessOrder> GetOrders()
        {
            ShopContext context = new ShopContext();
            context.Configuration.ProxyCreationEnabled = false;
            try
            {
                List<BusinessOrder> orders = new List<BusinessOrder>();
                foreach (var order in context.Orders)
                {
                    BusinessOrder newOrder = new BusinessOrder();
                    newOrder.OrderId = order.OrderId;
                    
                    newOrder.OrderDate = order.OrderDate.ToShortDateString();
                    newOrder.OrderSum = order.OrderSum;
                    newOrder.OrderItems = new List<BusinessOrderItem>();
                    foreach (var orderItem in context.OrderItems.Where(oi => oi.OrderId == order.OrderId))
                    {
                        BusinessOrderItem newOrderItem = new BusinessOrderItem();
                        newOrderItem.GoodId = (int)orderItem.GoodId;
                        newOrderItem.GoodName = context.Goods.Where(g => g.GoodId == orderItem.GoodId).FirstOrDefault().GoodName;
                        newOrderItem.GoodCount = orderItem.GoodCount;
                        newOrderItem.OrderItemSum = orderItem.OrderItemSum;
                        newOrder.OrderItems.Add(newOrderItem);
                    }
                    orders.Add(newOrder);
                }
                return orders;
            }
            catch (Exception exc)
            {
                throw new FaultException("Error Server FaultException: " + exc.Message);
            }
        }


        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public string AddOrder(BusinessOrderItem[] businessOrder)
        {
            List<BusinessOrderItem> orderList = new List<BusinessOrderItem>();
            for (int i = 0; i < businessOrder.Length; i++)
            {
                orderList.Add(businessOrder[i]);
            }
            try
            {
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.RequiresNew))
                {
                    int orderId = CreateOrder();
                    CreateOrderItems(orderList, orderId);
                    scope.Complete();
                }
                return null;
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return exc.Message;
            }
        }

        private int CreateOrder()
        {
            ShopContext context = new ShopContext();
            Order order = new Order() { OrderId = 0, OrderDate = DateTime.Now, OrderSum = 0 };
            context.Orders.AddOrUpdate(order);
            context.SaveChanges();
            return order.OrderId;

        }

        private void CreateOrderItems(List<BusinessOrderItem> businessOrder, int orderId)
        {
            ShopContext context = new ShopContext();
            foreach (var businessItem in businessOrder)
            {
                OrderItem orderItem = new OrderItem();
                orderItem.OrderItemId = 0;
                orderItem.OrderId = orderId;
                orderItem.GoodId = businessItem.GoodId;
                orderItem.GoodCount = businessItem.GoodCount;
                orderItem.OrderItemSum = businessItem.OrderItemSum;
                context.Goods.Where(g => g.GoodId == businessItem.GoodId).FirstOrDefault().GoodCount -= businessItem.GoodCount;
                context.Orders.Where(o => o.OrderId == orderId).FirstOrDefault().OrderSum += businessItem.OrderItemSum;
                context.OrderItems.AddOrUpdate(orderItem);
            }
            context.SaveChanges();
        }
    }
}

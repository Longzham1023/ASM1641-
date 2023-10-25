using System;
using ASM1641_.Data;
using ASM1641_.Dtos;
using ASM1641_.IService;
using ASM1641_.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ASM1641_.Service
{
    public class OrderService : IOrderService
    {

        private readonly IMongoCollection<Orders> _orderCollection;
        public OrderService(IOptions<DatabaseSetting> dbSetting)
        {
            var mongoClient = new MongoClient(dbSetting.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(dbSetting.Value.DatabaseName);

            _orderCollection = mongoDatabase.GetCollection<Orders>(dbSetting.Value.OrderCollection);
        }
        public async Task DeleteOrderAsync(string orderId)
        {
            var order = Builders<Orders>.Filter.Eq("Id", orderId);
            if(order != null)
            {
                await _orderCollection.DeleteOneAsync(order);
            }
            else
            {
                throw new Exception("Order Id not found!");
            }
        }

        public async Task<OrderResult> GetAllOrdersAsync(int page)
        {
            if (page <= 0)
            {
                throw new Exception("pageNumber must be greater than zero.");
            }

            int skip = (page - 1) * 10;

            var totalOrders = await _orderCollection.CountDocumentsAsync(_ => true);
            var totalPages = (int)Math.Ceiling((double)totalOrders / 10);
            var orders = await _orderCollection.Find(e => true).Skip(skip).Limit(10).ToListAsync();

            return new OrderResult()
            {
                totalOrders = totalOrders,
                totalPages = totalPages,
                orders = orders,
                page = page
            };
        }

        public async Task<Orders> GetOrderByIdAsync(string orderId)
        {
           

            var orderFilter = Builders<Orders>.Filter.Eq(e => e.Id, orderId);
            var order = await _orderCollection.Find(orderFilter).FirstOrDefaultAsync();

            if (order == null)
            {
                throw new Exception("Order not found.");
            }

            return order;
        }


        public async Task<OrderResult> GetOrdersByCustomerIdAsync(int page, string customerId)
        {
            if (page <= 0)
            {
                throw new Exception("Page number must be greater than zero.");
            }

            // Assuming your page size is 10, change it accordingly if it's different
            int pageSize = 10;
            int skip = (page - 1) * pageSize;

            var orderFilter = Builders<Orders>.Filter.Eq(e => e.customerId, customerId);
            var orders = await _orderCollection.Find(orderFilter).Skip(skip).Limit(pageSize).ToListAsync();

            // Count the total number of orders for the given customer
            long totalOrders = await _orderCollection.CountDocumentsAsync(orderFilter);

            // Calculate the total number of pages
            int totalPages = (int)Math.Ceiling((double)totalOrders / pageSize);

            return new OrderResult()
            {
                totalOrders = totalOrders,
                totalPages = totalPages,
                orders = orders,
                page = page
            };
        }


        public async Task updateStatusOrder(string orderId, string status)
        {
            var orderFilter = Builders<Orders>.Filter.Eq("Id", orderId);
            var order = await _orderCollection.Find(orderFilter).FirstOrDefaultAsync();
            if (order != null)
            {
                order.status = status;
                await _orderCollection.ReplaceOneAsync(orderFilter, order);
            }
            else
            {
                throw new Exception("Order Id not found!");
            }
        }
    }
}


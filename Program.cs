var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();

var app = builder.Build();

app.MapGet("/orders", () =>
{
    using (var session = DocumentStoreHolder.Store.OpenSession())
    {
        var orders = session.Query<Order>().ToList();

        return orders;
    }
});

app.MapGet("/orders/redis", async (IDistributedCache distributedCache) =>
{
    try
    {
        using (var session = DocumentStoreHolder.Store.OpenSession())
        {        
            var orders = session.Query<Order>().ToList();

            var cacheKey = "orderList";

            string serializedOrders;

            var orderList = new List<Order>();

            var redisOrders = await distributedCache.GetAsync(cacheKey);

            if (redisOrders != null)
            {
                serializedOrders = Encoding.UTF8.GetString(redisOrders);
                orderList = JsonConvert.DeserializeObject<List<Order>>(serializedOrders);
            }
            else
            {
                orderList = orders.ToList();

                serializedOrders = JsonConvert.SerializeObject(orderList);

                redisOrders = Encoding.UTF8.GetBytes(serializedOrders);

                var options = new DistributedCacheEntryOptions()
                      .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                      .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                distributedCache.SetAsync(cacheKey, redisOrders, options);
            }
            return orderList;
        }
    }
    catch (global::System.Exception ex)
    {
        throw;
    }
});

app.Run();

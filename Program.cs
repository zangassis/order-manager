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

            string serializedOrderList;

            var orderList = new List<Order>();

            var redisOrderList = await distributedCache.GetAsync(cacheKey);

            if (redisOrderList != null)
            {
                serializedOrderList = Encoding.UTF8.GetString(redisOrderList);
                orderList = JsonConvert.DeserializeObject<List<Order>>(serializedOrderList);
            }
            else
            {
                orderList = orders.ToList();

                serializedOrderList = JsonConvert.SerializeObject(orderList);

                redisOrderList = Encoding.UTF8.GetBytes(serializedOrderList);

                var options = new DistributedCacheEntryOptions()
                      .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
                      .SetSlidingExpiration(TimeSpan.FromMinutes(2));

                distributedCache.SetAsync(cacheKey, redisOrderList, options);
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

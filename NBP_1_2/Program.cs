


using Neo4jClient;
using Neo4j;
using NBP_1_2.Neo4J;
using NBP_1_2.Redis;
using StackExchange.Redis;
using NBP_1_2.Hubs;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("cors", builder => {
        builder.AllowAnyOrigin().AllowAnyMethod();
    });
});

builder.Services.AddDistributedMemoryCache();
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton(RedisLogic.GetClient);
builder.Services.AddSingleton(Neo4JLogic.GetClient);

builder.Services.AddSignalR().AddStackExchangeRedis(Config.SingleHostConnectionString);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();



app.UseRouting();

app.UseCors("cors");

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<NotificationHub>("/notificationHub");

app.Run();


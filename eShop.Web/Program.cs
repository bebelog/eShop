using eShop.CoreBusiness.Services;
using eShop.DataStore.SQL.Dapper.Helpers;
using eShop.ShoppingCartLocalStorage;
using eShop.StateStore.DI;
using eShop.UseCases.AdminPortal.OrderDetailScreen;
using eShop.UseCases.AdminPortal.OutstandingOrdersScreen;
using eShop.UseCases.AdminPortal.ProcessedOrdersScreen;
using eShop.UseCases.OrderConfirmationScreen;
using eShop.UseCases.PluginInterfaces.DataStore;
using eShop.UseCases.PluginInterfaces.StateStore;
using eShop.UseCases.PluginInterfaces.UI;
using eShop.UseCases.SearchProductScreen;
using eShop.UseCases.ShoppingCartScreen;
using eShop.UseCases.ShoppingCartScreen.interfaces;
using eShop.UseCases.ViewProductScreen;
using eShop.UseCases.ViewProductScreen.interfaces;
using eShop.Web.Components;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace eShop.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddAuthentication("eShop.CookieAuth")
                .AddCookie("eShop.CookieAuth", config =>
                {
                    config.Cookie.Name = "eShop.CookieAuth";
                    config.LoginPath = "/login";
                    config.LogoutPath = "/logout";
                });
            builder.Services.AddAuthorization();
            builder.Services.AddCascadingAuthenticationState();

            builder.Services.AddRazorComponents().AddInteractiveServerComponents();

            // DataAccess & Repositories (Dapper SQL Server Plugin)
            builder.Services.AddTransient<IDataAccess, DataAccess>();
            builder.Services.AddTransient<IProductRepository, eShop.DataStore.SQL.Dapper.ProductRepository>();
            builder.Services.AddTransient<IOrderRepository, eShop.DataStore.SQL.Dapper.OrderRepository>();

            builder.Services.AddScoped<IShoppingCart, ShoppingCart>();
            builder.Services.AddScoped<IShoppingCartStateStore, ShoppingCartStateStore>();

            builder.Services.AddTransient<IOrderService, OrderServices>();

            // Customer Portal Use Cases
            builder.Services.AddTransient<ISearchProductUseCase, SearchProductUseCase>();
            builder.Services.AddTransient<IViewProductUseCase, ViewProductUseCase>();
            builder.Services.AddTransient<IAddProductToCartUseCase, AddProductToCartUseCase>();
            builder.Services.AddTransient<IViewShoppingCartUseCase, ViewShoppingCartUseCase>();
            builder.Services.AddTransient<IDeleteProductUseCase, DeleteProductUseCase>();
            builder.Services.AddTransient<IUpdateQuantityUseCase, UpdateQuantityUseCase>();
            builder.Services.AddTransient<IPlaceOrderUseCase, PlaceOrderUseCase>();
            builder.Services.AddTransient<IViewOrderConfirmationUseCase, ViewOrderConfirmationUseCase>();

            // Admin Portal Use Cases
            builder.Services.AddTransient<IViewOutstandingOrdersUseCase, ViewOutstandingOrdersUseCase>();
            builder.Services.AddTransient<IViewOrderDetailUseCase, ViewOrderDetailUseCase>();
            builder.Services.AddTransient<IProcessOrderUseCase, ProcessOrderUseCase>();
            builder.Services.AddTransient<IViewProcessedOrdersUseCase, ViewProcessedOrdersUseCase>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseAntiforgery();

            app.MapControllers();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode()
                .AddAdditionalAssemblies(
                    typeof(eShop.Web.CustomerPortal.Pages.ViewProductComponent).Assembly,
                    typeof(eShop.Web.AdminPortal.Pages.OutstandingOrders).Assembly,
                    typeof(eShop.Web.Common.Controls.LoginComponent).Assembly
                );

            app.Run();
        }
    }
}

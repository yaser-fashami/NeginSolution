using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Negin.Core.Domain.Entities;
using Negin.Core.Domain.Interfaces;
using Negin.Infra.Data.Sql.EFRepositories;
using Negin.Infrastructure;
using Negin.Services.Billing;
using Negin.Services.Operation;
using SmartBreadcrumbs.Extensions;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});
builder.Services.AddDbContext<NeginDbContext>(c => c.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>(option=> {
    option.Password.RequiredLength = 3;
    option.Password.RequireNonAlphanumeric = false;
    option.Password.RequireUppercase = false;
    option.Password.RequireLowercase = false;
    option.Password.RequireDigit= true;
    option.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<NeginDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.AccessDeniedPath = "/Account/AccessDenied";
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddBreadcrumbs(Assembly.GetExecutingAssembly(), options =>
{
	options.TagName = "nav";
	options.TagClasses = "";
	options.OlClasses = "breadcrumb breadcrumb-separatorless fw-semibold fs-7 my-0 pt-1";
	options.LiClasses = "breadcrumb-item text-muted";
	options.ActiveLiClasses = "breadcrumb-item active";
	options.SeparatorElement = "<li class=\"breadcrumb-item\">\r\n\t\t\t\t\t\t\t\t\t\t\t\t<span class=\"bullet bg-gray-400 w-5px h-2px\"></span>\r\n\t\t\t\t\t\t\t\t\t\t\t</li>";
    options.DefaultAction = "List";
});

#region IOC
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IVesselRepository, VesselRepository>();
builder.Services.AddScoped<IShippingLineCompanyRepository, ShippingLineCompanyRepository>();
builder.Services.AddScoped<IVoyageRepository, VoyageRepository>();
builder.Services.AddScoped<IVesselStoppageService, VesselStoppageService>();
builder.Services.AddScoped<IBasicInfoRepository, BasicInfoRepository>();
builder.Services.AddScoped<IInvoiceCalculator, InvoiceCalculator>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/BasicInfo/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=BasicInfo}/{action=List}/{id?}");

app.Run();
